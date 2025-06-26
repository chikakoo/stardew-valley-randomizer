using StardewValley;
using System;

namespace Randomizer
{
    /// <summary>
    /// Represents a boot - doesn't use the same IDs as normal items, but we'll treat it like an item
    /// </summary>
    public class BootItem : Item
	{
		public string OriginalName { get; set; }
		public string Description { get; set; }
		public int Defense { get; set; }
		public int Immunity { get; set; }

        public BootItem(
            string originalName,
            string id,
			string name,
			int defense,
			int immunity) : base(id)
        {
            DifficultyToObtain = ObtainingDifficulties.NonCraftingItem;
            CanStack = false;
            ShouldBeForagable = false;

			OriginalName = originalName;
            OverrideName = name;
            Description = BootRandomizer.BootData[id.ToString()].Split("/")[(int)BootIndexes.Description];
            Defense = defense;
            Immunity = immunity;
        }

        public BootItem(
			string originalName,
			string id,
			string name,
			string description,
			int defense,
			int immunity) : this(originalName, id, name, defense, immunity)
		{
			if (Globals.ModRef.Helper.Translation.LocaleEnum == LocalizedContentManager.LanguageCode.en)
			{
				Description = description;
			}
		}

		/// <summary>
		/// Gets the sale price of the boot
		/// Equal to the sell price * 8
		/// </summary>
		/// <returns />
		public int GetBuyPrice()
		{
			return Math.Max(50 * 8 * (Defense + Immunity), 100);
		}

		/// <summary>
		/// The string to use for asset replacement
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			string[] originalData = BootRandomizer.BootData[Id.ToString()].Split("/");
            originalData[(int)BootIndexes.Name] = OverrideName;
            originalData[(int)BootIndexes.Description] = Description;
			originalData[(int)BootIndexes.Price] = GetBuyPrice().ToString();
            originalData[(int)BootIndexes.Defense] = Defense.ToString();
            originalData[(int)BootIndexes.Immunity] = Immunity.ToString();

			// If we are randomizing names, override it; otherwise, keep the old name
			originalData[(int)BootIndexes.DisplayName] = 
				Globals.Config.Boots.RandomizeNames
					? OverrideName
					: OriginalName;

            return string.Join("/", originalData);
		}
	}
}
