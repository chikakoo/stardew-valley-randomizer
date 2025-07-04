using StardewModdingAPI;
using System;

namespace Randomizer
{
    public class ModConfigMenuHelper
	{
		public IGenericModConfigMenuApi Api;
		public IManifest ModManifest;

		public ModConfigMenuHelper(IGenericModConfigMenuApi api, IManifest modManifest)
		{
			Api = api;
			ModManifest = modManifest;
		}

		public void RegisterModOptions()
		{
			AddCheckbox("Create Spoiler Log", "Create a text file which contains all of the randomized elements when a new farm is created. Highly recommended to leave on.", () => Globals.Config.CreateSpoilerLog, (bool val) => Globals.Config.CreateSpoilerLog = val);
			AddCheckbox("Create Bundle Log", "Creates a text file with most community center bundle required items and shorthand versions of seasons/locations for certain items. Does not spoil rewards.", () => Globals.Config.CreateBundleLog, (bool val) => Globals.Config.CreateBundleLog = val);
			AddCheckbox("Save Randomized Images", "Saves most of the randomized images under a file called \"randomizedImages.png\". Used for debugging purposes - will slow down load times if on.", () => Globals.Config.SaveRandomizedImages, (bool val) => Globals.Config.SaveRandomizedImages = val);

			AddSectionTitle("---RANDOMIZATION OPTIONS---", "Toggle on or off the various aspects of the game which can be randomized. Note that toggling random images from ON to OFF will require a game restart to reload the default image.");

			AddSectionTitle("Bundle Options");
			AddCheckbox("Community Center Bundles", "Generate new bundles for each room which select a random number of items from a themed pool.", () => Globals.Config.Bundles.Randomize, (bool val) => Globals.Config.Bundles.Randomize = val);
			AddCheckbox("Show Helpful Tooltips", "When this option is enabled, mouse over the items in a bundle to get a helpful description of where to locate them.", () => Globals.Config.Bundles.ShowDescriptionsInBundleTooltips, (bool val) => Globals.Config.Bundles.ShowDescriptionsInBundleTooltips = val);

			AddSectionTitle("Crafting Recipe Options");
			AddCheckbox("Crafting Recipes", "Create recipes using randomly selected items from a pool. Uses rules for balanced difficulty.", () => Globals.Config.CraftingRecipes.Randomize, (bool val) => Globals.Config.CraftingRecipes.Randomize = val);
			AddCheckbox("Skill Level Requirements", "Randomize levels at which the recipes are learned. Recipe randomization must be turned on for this to take effect.", () => Globals.Config.CraftingRecipes.RandomizeLevels, (bool val) => Globals.Config.CraftingRecipes.RandomizeLevels = val);

			AddSectionTitle("NPC Options");
			AddCheckbox("NPC Birthdays", "Moves each NPC's birthday to a random day in the year.", () => Globals.Config.NPCs.RandomizeBirthdays, (bool val) => Globals.Config.NPCs.RandomizeBirthdays = val);
			AddCheckbox("Individual Item Preferences", "Generates a new set of loved items, hated items, and so on for each NPC.", () => Globals.Config.NPCs.RandomizeIndividualPreferences, (bool val) => Globals.Config.NPCs.RandomizeIndividualPreferences = val);
			AddCheckbox("Universal Item Preferences", "Generates new sets of universally loved, liked, disliked, hated, and neutral items.", () => Globals.Config.NPCs.RandomizeUniversalPreferences, (bool val) => Globals.Config.NPCs.RandomizeUniversalPreferences = val);
            AddCheckbox("Sprite Shuffle (Glitchy)", "Shuffles NPCs sprites among each other. Use at your own risk - can result in glitchy/invisible graphics.", () => Globals.Config.NPCs.SpriteShuffle, (bool val) => Globals.Config.NPCs.SpriteShuffle = val);

            AddSectionTitle("Crop Options");
			AddCheckbox("Randomize Stats", "Shuffles and randomizes what crops grow, their growing schedules, and their attributes (trellis, scythe needed, etc.).", () => Globals.Config.Crops.RandomizeStats, (bool val) => Globals.Config.Crops.RandomizeStats = val);
            AddCheckbox("Randomize Names", "Randomize crop names - recommeded to turn this on when randomizing stats so you know what crops each seed will grow.", () => Globals.Config.Crops.RandomizeNames, (bool val) => Globals.Config.Crops.RandomizeNames = val);
            AddCheckbox("Use Custom Crop Images", "Use custom images for seeds and crops at each growth stage.", () => Globals.Config.Crops.UseCustomImages, (bool val) => Globals.Config.Crops.UseCustomImages = val);
			AddHueShiftOption("Hue Shift Max", "The maxmium value that crop images will be hue-shifted. Set to 0 for no effect.", () => Globals.Config.Crops.HueShiftMax, (int val) => Globals.Config.Crops.HueShiftMax = val);

			AddSectionTitle("Fruit Tree Options");
            AddCheckbox("Randomize Fruit", "Generates Item saplings that grow a random item. Prices are loosely balanced based on the item grown.", () => Globals.Config.FruitTrees.RandomizeFruit, (bool val) => Globals.Config.FruitTrees.RandomizeFruit = val);
            AddCheckbox("Use Custom Fruit Tree Image", "Uses the custom fruit tree image, which by default has no specific fruit pictures.", () => Globals.Config.FruitTrees.UseCustomImages, (bool val) => Globals.Config.FruitTrees.UseCustomImages = val);

            AddSectionTitle("Fish Options");
			AddCheckbox("Shuffle Locations", "Shuffle all fish locations and corresponding seasons.", () => Globals.Config.Fish.ShuffleSeasonsAndLocations, (bool val) => Globals.Config.Fish.ShuffleSeasonsAndLocations = val);
            AddCheckbox("Randomize Behaviors", "Randomize fish difficulties and behaviors.", () => Globals.Config.Fish.RandomizeBehaviors, (bool val) => Globals.Config.Fish.RandomizeBehaviors = val);
            AddCheckbox("Randomize Names", "Randomize fish names.", () => Globals.Config.Fish.RandomizeNames, (bool val) => Globals.Config.Fish.RandomizeNames = val);
            AddCheckbox("Use Custom Fish Images", "Use custom images for the fish.", () => Globals.Config.Fish.UseCustomImages, (bool val) => Globals.Config.Fish.UseCustomImages = val);
			AddHueShiftOption("Hue Shift Max", "The maxmium value that fish images will be hue-shifted. Set to 0 for no effect.", () => Globals.Config.Fish.HueShiftMax, (int val) => Globals.Config.Fish.HueShiftMax = val);
			AddPercentageOption("Golden Tag %", "During the Trout Derby, the chance that a fish caught while in the town, forest, or mountains will award a Golden Tag.", () => Globals.Config.Fish.GoldenTagChance, (int val) => Globals.Config.Fish.GoldenTagChance = val);

			AddSectionTitle("Monster Options");
			AddCheckbox("Monster Stats", "Randomize monster stats, behaviors, and non-unique item drops.", () => Globals.Config.Monsters.Randomize, (bool val) => Globals.Config.Monsters.Randomize = val);
			AddCheckbox("Shuffle Monster Drops", "Shuffle unique monster drops between all monsters.", () => Globals.Config.Monsters.SwapUniqueDrops, (bool val) => Globals.Config.Monsters.SwapUniqueDrops = val);
            AddHueShiftOption("Hue Shift Max", "The maxmium value that monster images will be hue shifted by (excludes slimes). Set to 0 for no effect.", () => Globals.Config.Monsters.HueShiftMax, (int val) => Globals.Config.Monsters.HueShiftMax = val);

            AddSectionTitle("Weapon Options");
			AddCheckbox("Randomize Stats", "Randomize weapon stats, types, and drop locations.", () => Globals.Config.Weapons.RandomizeStats, (bool val) => Globals.Config.Weapons.RandomizeStats = val);
            AddCheckbox("Randomize Names", "Randomize weapon names.", () => Globals.Config.Weapons.RandomizeNames, (bool val) => Globals.Config.Weapons.RandomizeNames = val);
            AddCheckbox("Use Custom Weapon Images", "Use custom images for weapons.", () => Globals.Config.Weapons.UseCustomImages, (bool val) => Globals.Config.Weapons.UseCustomImages = val);

			AddSectionTitle("Boot Options");
			AddCheckbox("Randomize Stats", "Randomize boots stats.", () => Globals.Config.Boots.RandomizeStats, (bool val) => Globals.Config.Boots.RandomizeStats = val);
            AddCheckbox("Randomize Names", "Randomize boot names and descriptions.", () => Globals.Config.Boots.RandomizeNames, (bool val) => Globals.Config.Boots.RandomizeNames = val);
            AddCheckbox("Use Custom Boot Images", "Use custom images for boots.", () => Globals.Config.Boots.UseCustomImages, (bool val) => Globals.Config.Boots.UseCustomImages = val);
			AddHueShiftOption("Hue Shift Max", "The maxmium value that boot images will be hue-shifted. Set to 0 for no effect.", () => Globals.Config.Boots.HueShiftMax, (int val) => Globals.Config.Boots.HueShiftMax = val);

			AddSectionTitle("Animal Options");
			AddCheckbox("Randomize Horse Images", "Use custom images for horses.", () => Globals.Config.Animals.RandomizeHorses, (bool val) => Globals.Config.Animals.RandomizeHorses = val);
			AddCheckbox("Randomize Pet Images",
				"Use custom images for pet - you must choose the first pet option for this to work.",
				() => Globals.Config.Animals.RandomizePets,
				(bool val) =>
				{ 
					Globals.Config.Animals.RandomizePets = val;
                    Globals.ModRef.Helper.GameContent.InvalidateCache(AnimalIconPatcher.StardewAssetPath);
                });
            AddCheckbox("RandomizeCritters", "Use custom images for critters using replacements in their respective directories.", () => Globals.Config.Animals.RandomizeCritters, (bool val) => Globals.Config.Animals.RandomizeCritters = val);
            AddHueShiftOption("Critter Hue Shift Max", "The maxmium value that critter images will be hue shifted by. Set to 0 for no effect.", () => Globals.Config.Animals.CritterHueShiftMax, (int val) => Globals.Config.Animals.CritterHueShiftMax = val);

            AddSectionTitle("Music Options");
			AddCheckbox("Music", "Shuffle most songs and ambience.", () => Globals.Config.Music.Randomize, (bool val) => Globals.Config.Music.Randomize = val);
			AddCheckbox("Random Song on Each Change", "Plays a random song each time the song would change.", () => Globals.Config.Music.RandomSongEachChange, (bool val) => Globals.Config.Music.RandomSongEachChange = val);

			AddSectionTitle("Shop Options");
            AddCheckbox("Seed Shop Item of the Week", "Adds an item to Pierre's shop that changes every Monday.", () => Globals.Config.Shops.AddSeedShopItemOfTheWeek, (bool val) => Globals.Config.Shops.AddSeedShopItemOfTheWeek = val);
            AddCheckbox("Joja Mart Item of the week", "Adds an item to the Joja Mart that changes every Monday.", () => Globals.Config.Shops.AddJojaMartItemOfTheWeek, (bool val) => Globals.Config.Shops.AddJojaMartItemOfTheWeek = val);
            AddCheckbox("Add Clay to Robin's", "Adds clay to Robin's shop, costing between 25-75 coins each day.", () => Globals.Config.Shops.AddClayToRobinsShop, (bool val) => Globals.Config.Shops.AddClayToRobinsShop = val);
            AddCheckbox("Add Tapper Materials to Robin's", "Adds a random tapper material item to Robin's shop each day at an inflated price. This is to make crafting a tapper easier.", () => Globals.Config.Shops.AddTapperCraftItemsToRobinsShop, (bool val) => Globals.Config.Shops.AddTapperCraftItemsToRobinsShop = val);
            AddCheckbox("Randomize Blacksmith Shop", "Adds a random chance of a discount, or a mining-related item to appear in the blacksmith shop.", () => Globals.Config.Shops.RandomizeBlacksmithShop, (bool val) => Globals.Config.Shops.RandomizeBlacksmithShop = val);
			AddCheckbox("Randomize Saloon Shop", "Randomizes cooked food and recipes in Gus' Saloon Shop. Beer and Coffee are always available.", () => Globals.Config.Shops.RandomizeSaloonShop, (bool val) => Globals.Config.Shops.RandomizeSaloonShop = val);
            AddCheckbox("Willy's Catch of the Day", "Adds 1-3 stock of any random fish from the current season.", () => Globals.Config.Shops.AddFishingShopCatchOfTheDay, (bool val) => Globals.Config.Shops.AddFishingShopCatchOfTheDay = val);
            AddCheckbox("Randomize Oasis Shop", "Randomizes Sandy's shop by replacing the crop/foragable she sometimes sells. Also includes a couple random useful items. The randomization changes every Monday", () => Globals.Config.Shops.RandomizeOasisShop, (bool val) => Globals.Config.Shops.RandomizeOasisShop = val);
            AddCheckbox("Hat of the Week", "Adds a hat of the week to the hat shop (you must have unlocked at least one hat to unlock the shop first).", () => Globals.Config.Shops.AddHatShopHatOfTheWeek, (bool val) => Globals.Config.Shops.AddHatShopHatOfTheWeek = val);
			AddCheckbox("Randomize Sewer Shop", "Randomizes the two decorative items to two random items daily.", () => Globals.Config.Shops.RandomizerSewerShop, (bool val) => Globals.Config.Shops.RandomizerSewerShop = val);
            AddCheckbox("Randomize Club Shop", "Randomizes Qi's Club (the casino).", () => Globals.Config.Shops.RandomizeClubShop, (bool val) => Globals.Config.Shops.RandomizeClubShop = val);

			AddSectionTitle("Lightning Rod Options");
			AddCheckbox("Randomize", "Enables randomizing lightning rod output to the weights below.", () => Globals.Config.LightningRod.Randomize, (bool val) => Globals.Config.LightningRod.Randomize = val);
            AddPositiveNumberOption("Battery Weight", "(Requires Randomize to be on) The relative weight of a battery to be received from a lightning rod.", () => Globals.Config.LightningRod.BatteryWeight, (int val) => Globals.Config.LightningRod.BatteryWeight = val);
            AddPositiveNumberOption("Item Weight", "(Requires Randomize to be on) The relative weight of a normal item to be received from a lightning rod.", () => Globals.Config.LightningRod.ObjectWeight, (int val) => Globals.Config.LightningRod.ObjectWeight = val);
            AddPositiveNumberOption("Big Craftable Weight", "(Requires Randomize to be on) The relative weight of a big craftable (these are mostly machines) to be received from a lightning rod.", () => Globals.Config.LightningRod.BigCraftableWeight, (int val) => Globals.Config.LightningRod.BigCraftableWeight = val);
            AddPositiveNumberOption("Furniture Weight", "(Requires Randomize to be on) The relative weight of a furniture item to be received from a lightning rod.", () => Globals.Config.LightningRod.FurnitureWeight, (int val) => Globals.Config.LightningRod.FurnitureWeight = val);

            AddSectionTitle("Recycling Machine Options");
            AddCheckbox("Randomize", "Enables recycling any normal item into a random item within the cost allowance. Does not change items that can be recycled normally.", () => Globals.Config.RecyclingMachine.Randomize, (bool val) => Globals.Config.RecyclingMachine.Randomize = val);
            AddNumberOption("Max Price (% Above)", "The % above the input item's price that can be outputted. Negative input will match any item above the input's price.", () => Globals.Config.RecyclingMachine.PercentAbovePrice, (int val) => Globals.Config.RecyclingMachine.PercentAbovePrice = val);
            AddNumberOption("Min Price (% Below)", "The % below the input item's price that can be outputted. Negative input will match any item below the input's price.", () => Globals.Config.RecyclingMachine.PercentBelowPrice, (int val) => Globals.Config.RecyclingMachine.PercentBelowPrice = val);
            AddPercentageOption("Percent Any Random Item", "The % chance that any item that's worth more than the input can be received.", () => Globals.Config.RecyclingMachine.PercentAnyRandomItem, (int val) => Globals.Config.RecyclingMachine.PercentAnyRandomItem = val);

            AddSectionTitle("Misc Options");
			AddCheckbox("Building Costs", "Farm buildings that Robin can build for the player choose from a random pool of resources.", () => Globals.Config.RandomizeBuildingCosts, (bool val) => Globals.Config.RandomizeBuildingCosts = val);
			AddCheckbox("Forageables", "Forageables for every season and location are now randomly selected. Every forageable appears at least once per year.", () => Globals.Config.RandomizeForagables, (bool val) => Globals.Config.RandomizeForagables = val);
			AddCheckbox("Randomize Museum Rewards", "Changes museum rewards to similiar items. Does not affect the Dwarven Translation Manual, Ancient Fruit, or the Stardrop rewards.", () => Globals.Config.RandomizeMuseumRewards, (bool val) => Globals.Config.RandomizeMuseumRewards = val);
            AddCheckbox("Randomize Garbage Cans", "Random chance of NPC's dislikes/hates to be obtained from garbage cans.", () => Globals.Config.RandomizeGarbageCans, (bool val) => Globals.Config.RandomizeGarbageCans = val);
            AddCheckbox("Intro Text", "Replace portions of the intro cutscene with Mad Libs style text.", () => Globals.Config.RandomizeIntroStory, (bool val) => Globals.Config.RandomizeIntroStory = val);
			AddCheckbox("Quests", "Randomly select quest givers, required items, and rewards.", () => Globals.Config.RandomizeQuests, (bool val) => Globals.Config.RandomizeQuests = val);
			AddCheckbox("Rain", "Replace rain with a variant (Skulls/Junimos/Cats and Dogs/etc).", () => Globals.Config.RandomizeRain, (bool val) => Globals.Config.RandomizeRain = val);
		}

        /// <summary>
        /// A wrapper for the AddBoolOption functionality for readability
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="optionTooltip"></param>
        /// <param name="optionGet"></param>
        /// <param name="optionSet"></param>
        private void AddCheckbox(
			string optionName, 
			string optionTooltip, 
			Func<bool> optionGet, 
			Action<bool> optionSet)
		{
            Api.AddBoolOption(
				mod: ModManifest,
				name: () => optionName,
				tooltip: () => optionTooltip,
				getValue: optionGet,
				setValue: optionSet
            );
        }

		/// <summary>
		/// A wrapper for AddNumberOption catered toward hue-shift ranges
		/// </summary>
		/// <param name="labelText"></param>
		/// <param name="tooltip"></param>
		/// <param name="getValue"></param>
		/// <param name="setValue"></param>
        private void AddHueShiftOption(
            string labelText,
            string tooltip,
            Func<int> getValue, 
			Action<int> setValue)
		{
			Api.AddNumberOption(
				mod: ModManifest,
				getValue: getValue,
				setValue: setValue,
				name: () => labelText,
				tooltip: () => tooltip,
				min: 0,
				max: 359);
		}

		/// <summary>
		/// A wrapper for AddNumberOption catered toward percentage ranges
		/// </summary>
		/// <param name="labelText"></param>
		/// <param name="tooltip"></param>
		/// <param name="getValue"></param>
		/// <param name="setValue"></param>
		private void AddPercentageOption(
			string labelText,
			string tooltip,
			Func<int> getValue,
			Action<int> setValue)
		{
			Api.AddNumberOption(
				mod: ModManifest,
				getValue: getValue,
				setValue: setValue,
				name: () => labelText,
				tooltip: () => tooltip,
				min: 0,
				max: 100);
		}

        /// <summary>
        /// A wrapper for AddNumberOption catered toward any positive input
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="tooltip"></param>
        /// <param name="getValue"></param>
        /// <param name="setValue"></param>
        private void AddPositiveNumberOption(
            string labelText,
            string tooltip,
            Func<int> getValue,
            Action<int> setValue)
        {
            Api.AddNumberOption(
                mod: ModManifest,
                getValue: getValue,
                setValue: setValue,
                name: () => labelText,
                tooltip: () => tooltip,
				min: 0);
        }

        /// <summary>
        /// A wrapper for AddNumberOption catered toward any input
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="tooltip"></param>
        /// <param name="getValue"></param>
        /// <param name="setValue"></param>
        private void AddNumberOption(
            string labelText,
            string tooltip,
            Func<int> getValue,
            Action<int> setValue)
        {
            Api.AddNumberOption(
                mod: ModManifest,
                getValue: getValue,
                setValue: setValue,
                name: () => labelText,
                tooltip: () => tooltip);
        }

        /// <summary>
        /// A wrapper for AddSectionTitle
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tooltip"></param>
        private void AddSectionTitle(string text, string tooltip = "")
		{
            Api.AddSectionTitle(ModManifest, () => text, () => tooltip);
		}
	}
}
