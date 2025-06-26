using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    public class BootRandomizer
	{
		private readonly static Dictionary<string, BootItem> Boots = new();
        private static RNG Rng { get; set; }

        /// <summary>
        /// The data from Data/Boots.xnb
        /// </summary>
        public static Dictionary<string, string> BootData { get; private set; }

        /// <summary>
        /// A map to keep track of boot names for the spoiler log
        /// </summary>
        private static Dictionary<string, string> OldBootNameMap { get; set; } = new();

        /// <summary>
        /// Randomizes boots - currently only changes defense and immunity
        /// </summary>
        /// <returns />
        public static Dictionary<string, string> Randomize()
		{
            // Initialize boot data here so that it's reloaded in case of a locale change
            // This is also used by ObjectImageBuilder, so do this here even if we aren't randomizing boots
            // Only include boots that have int ids - any others are from mods and won't work
            BootData = DataLoader.Boots(Game1.content)
				.Where(kv => int.TryParse(kv.Key, out int _))
				.ToDictionary(kv => kv.Key, kv => kv.Value);

            Dictionary<string, string> bootReplacements = new();
			if (!Globals.Config.Boots.ShouldSaveChanges())
			{
				return bootReplacements;
			}

            Rng = RNG.GetFarmRNG(nameof(BootRandomizer));
            Boots.Clear();
            OldBootNameMap.Clear();

			WeaponAndArmorNameRandomizer nameRandomizer = new(nameof(BootRandomizer));
			List<string> descriptions = 
				NameAndDescriptionRandomizer.GenerateBootDescriptions(BootData.Count);
			List<BootItem> bootsToUse = new();

			int index = 0;
			foreach (KeyValuePair<string, string> bootData in BootData)
			{
                string[] bootStringData = bootData.Value.Split("/");
                BootItem bootItem = CreateBootItemFromData(bootStringData, bootData.Key);

                bootsToUse.Add(bootItem);
                Boots.Add(bootItem.Id, bootItem);

                RandomizeNameAndDescription(bootItem, nameRandomizer, descriptions[index]);
                RandomizeStats(bootItem);

                // Fixes a crash that could occur with mods if boot display name data is missing
                if (bootStringData.Length == (int)BootIndexes.DisplayName)
                {
                    BootData[bootData.Key] += $"/{bootStringData[(int)BootIndexes.Name]}";
                }

                index++;
            }

			foreach (BootItem bootToAdd in bootsToUse)
            {
                bootReplacements.Add(bootToAdd.Id.ToString(), bootToAdd.ToString());
            }

            WriteToSpoilerLog(bootsToUse);
			return bootReplacements;
		}

        /// <summary>
        /// Creates a boot item from the given string data
        /// Grabs the original boot name if it exists on the array for later use as well
        /// </summary>
        /// <param name="bootStringData">The boot string data</param>
        /// <param name="bootDataKey">The key of the boot in the original dictionary</param>
        /// <returns></returns>
        private static BootItem CreateBootItemFromData(string[] bootStringData, string bootDataKey)
        {
            var bootName = bootStringData[(int)BootIndexes.Name];
            var originalBootName = bootStringData.Length > (int)BootIndexes.DisplayName
                ? bootStringData[(int)BootIndexes.DisplayName]
                : bootName;

            return new(
                originalBootName,
                bootDataKey,
                bootName,
                bootStringData[(int)BootIndexes.Description],
                int.Parse(bootStringData[(int)BootIndexes.Defense]),
                int.Parse(bootStringData[(int)BootIndexes.Immunity]));
        }

        /// <summary>
        /// Randomizes the name and description of the given boot
        /// </summary>
        /// <param name="bootItem">The boot to randomize</param>
        /// <param name="nameRandomizer">The name randomizer to use</param>
        /// <param name="description">The description to use</param>
		private static void RandomizeNameAndDescription(
            BootItem bootItem, 
            WeaponAndArmorNameRandomizer nameRandomizer,
            string description)
        {
            if (!Globals.Config.Boots.RandomizeNames) { return; }

            OldBootNameMap.Add(bootItem.Id, bootItem.Name);

            bootItem.OverrideName = nameRandomizer.GenerateRandomBootName();
            bootItem.Description = description;
        }

        /// <summary>
        /// Randomizes the stats of the given boot item
        /// - Sets it up to +/- 30% of the original stat pool, splitting randomly between defense and immunity
        /// - If both stats end up as 0, one is chosen at random to have a value of 1
        /// </summary>
        /// <param name="bootItem">The boot to randomize</param>
		private static void RandomizeStats(BootItem bootItem)
		{
            if (!Globals.Config.Boots.RandomizeStats) { return; }

            int statPool = Rng.NextIntWithinPercentage(bootItem.Defense + bootItem.Immunity, 30);
            int defense = Rng.NextIntWithinRange(0, statPool);
            int immunity = statPool - defense;

            if ((defense + immunity) == 0)
            {
                if (Rng.NextBoolean())
                {
                    defense = 1;
                }
                else
                {
                    immunity = 1;
                }
            }

            bootItem.Defense = defense;
            bootItem.Immunity = immunity;
        }

		/// <summary>
		/// Writes the boots to the spoiler log
		/// </summary>
		/// <param name="bootsToUse">The boot data that was used</param>
		private static void WriteToSpoilerLog(List<BootItem> bootsToUse)
		{
			Globals.SpoilerWrite("==== BOOTS ====");
			foreach (BootItem bootToAdd in bootsToUse)
			{
                var bootName = Globals.Config.Boots.RandomizeNames
                    ? $"{bootToAdd.OriginalName}: {bootToAdd.OverrideName}"
                    : bootToAdd.OriginalName;

                Globals.SpoilerWrite(bootName);

                if (Globals.Config.Boots.RandomizeStats)
                {
                    Globals.SpoilerWrite($"Defense: {bootToAdd.Defense}");
                    Globals.SpoilerWrite($"Immunity: {bootToAdd.Immunity}");
                    Globals.SpoilerWrite("---");
                }
			}
			Globals.SpoilerWrite("");
		}
	}
}
