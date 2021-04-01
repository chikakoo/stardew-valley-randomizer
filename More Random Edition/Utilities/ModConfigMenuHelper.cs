using StardewModdingAPI;
using System;

namespace Randomizer
{
	class ModConfigMenuHelper
	{
		public IGenericModConfigMenuAPI api;
		public IManifest ModManifest;

		public ModConfigMenuHelper(IGenericModConfigMenuAPI api, IManifest ModManifest)
		{
			this.api = api;
			this.ModManifest = ModManifest;
		}

		public void RegisterModOptions()
		{
			api.RegisterSimpleOption(ModManifest, "Create Spoiler Log", "Create a text file which contains all of the randomized elements when a new farm is created. Highly recommended to leave on.", () => Globals.Config.CreateSpoilerLog, (bool val) => Globals.Config.CreateSpoilerLog = val);

			AddLabel("---RANDOMIZATION OPTIONS---", "Toggle on or off the various aspects of the game which can be randomized.");

			AddLabel("Bundle Options");
			AddCheckBox("Community Center Bundles", "Generate new bundles for each room which select a random number of items from a themed pool.", () => Globals.Config.Bundles.Randomize, (bool val) => Globals.Config.Bundles.Randomize = val);
			AddCheckBox("> Show Helpful Tooltips", "When this option is enabled, mouse over the items in a bundle to get a helpful description of where to locate them.", () => Globals.Config.Bundles.ShowDescriptionsInBundleTooltips, (bool val) => Globals.Config.Bundles.ShowDescriptionsInBundleTooltips = val);

			AddLabel("Crafting Recipe Options");
			AddCheckBox("Crafting Recipes", "Create recipes using randomly selected items from a pool. Uses rules for balanced difficulty.", () => Globals.Config.CraftingRecipes.Randomize, (bool val) => Globals.Config.CraftingRecipes.Randomize = val);
			AddCheckBox("> Skill Level Requirements", "Randomize levels at which the recipes are learned. Recipe randomization must be turned on for this to take effect.", () => Globals.Config.CraftingRecipes.RandomizeLevels, (bool val) => Globals.Config.CraftingRecipes.RandomizeLevels = val);

			AddLabel("NPC Options");
			AddCheckBox("Swap NPC Skins", "Switch NPC's skins arounds. Does not change names or schedules.", () => Globals.Config.NPCs.RandomizeSkins, (bool val) => Globals.Config.NPCs.RandomizeSkins = val);
			AddCheckBox("NPC Birthdays", "Moves each NPC's birthday to a random day in the year.", () => Globals.Config.NPCs.RandomizeBirthdays, (bool val) => Globals.Config.NPCs.RandomizeBirthdays = val);
			AddCheckBox("Individual Item Preferences", "Generates a new set of loved items, hated items, and so on for each NPC.", () => Globals.Config.NPCs.RandomizeIndividualPreferences, (bool val) => Globals.Config.NPCs.RandomizeIndividualPreferences = val);
			AddCheckBox("Universal Item Preferences", "Generates new sets of universally loved, liked, disliked, hated, and neutral items.", () => Globals.Config.NPCs.RandomizeUniversalPreferences, (bool val) => Globals.Config.NPCs.RandomizeUniversalPreferences = val);

			AddLabel("Shop Options");
			AddCheckBox("Randomize Main Shops", "Randomizes the stock in Krobus' and Sandy's shops, and adds an Item of the Week to Pierre's shop.", () => Globals.Config.Shops.RandomizeMainShops, (bool val) => Globals.Config.Shops.RandomizeMainShops = val);
			AddCheckBox("Randomize Misc Shops", "Randomizes the stock in minor shops such as Gus' and Willy's shops.", () => Globals.Config.Shops.RandomizeMiscShops, (bool val) => Globals.Config.Shops.RandomizeMiscShops = val);

			AddLabel("Crop Options");
			AddCheckBox("Crops", "Randomize crop names, growing schedules, and attributes (trellis, scythe needed, etc.).", () => Globals.Config.Crops.Randomize, (bool val) => Globals.Config.Crops.Randomize = val);
			AddCheckBox("> Use Custom Crop Images", "Use custom images for seeds and crops at each growth stage.", () => Globals.Config.Crops.UseCustomImages, (bool val) => Globals.Config.Crops.UseCustomImages = val);
			AddCheckBox("Fruit Trees", "Generates Item saplings that grow a random item. Prices are loosely balanced based on the item grown.", () => Globals.Config.RandomizeFruitTrees, (bool val) => Globals.Config.RandomizeFruitTrees = val);

			AddLabel("Fish Options");
			AddCheckBox("Fish", "Randomize fish names, difficulty and behaviors, as well as locations, times of days and seasons.", () => Globals.Config.Fish.Randomize, (bool val) => Globals.Config.Fish.Randomize = val);
			AddCheckBox("> Use Custom Fish Images", "Use custom images for the fish.", () => Globals.Config.Fish.UseCustomImages, (bool val) => Globals.Config.Fish.UseCustomImages = val);

			AddLabel("Monster Options");
			AddCheckBox("Monster Stats", "Randomize monster stats, behaviors, and non-unique item drops.", () => Globals.Config.Monsters.Randomize, (bool val) => Globals.Config.Monsters.Randomize = val);
			AddCheckBox("Shuffle Monster Drops", "Shuffle unique monster drops between all monsters.", () => Globals.Config.Monsters.SwapUniqueDrops, (bool val) => Globals.Config.Monsters.SwapUniqueDrops = val);

			AddLabel("Weapon Options");
			AddCheckBox("Weapons", "Randomize weapon stats, types, and drop locations.", () => Globals.Config.Weapons.Randomize, (bool val) => Globals.Config.Weapons.Randomize = val);
			AddCheckBox("> Use Custom Weapon Images", "Use custom images for weapons.", () => Globals.Config.Weapons.UseCustomImages, (bool val) => Globals.Config.Weapons.UseCustomImages = val);
			AddCheckBox("Galaxy Sword Name", "Disable to have the Galaxy Sword keep its name. There is a hard-coded check to spawn a high level bat on Wilderness Farm at night if the player has a Galaxy Sword in their inventory.", () => Globals.Config.Weapons.RandomizeGalaxySwordName, (bool val) => Globals.Config.Weapons.RandomizeGalaxySwordName = val);

			AddLabel("Boot Options");
			AddCheckBox("Boots", "Randomize boots stats, names, descriptions.", () => Globals.Config.Boots.Randomize, (bool val) => Globals.Config.Boots.Randomize = val);
			AddCheckBox("> Use Custom Boot Images", "Use custom images for boots.", () => Globals.Config.Boots.UseCustomImages, (bool val) => Globals.Config.Boots.UseCustomImages = val);

			AddLabel("Music Options");
			AddCheckBox("Music", "Shuffle most songs and ambience.", () => Globals.Config.Music.Randomize, (bool val) => Globals.Config.Music.Randomize = val);
			AddCheckBox("Random Song on Area Change", "Plays a new song each time the loaded area changes.", () => Globals.Config.Music.RandomSongEachTransition, (bool val) => Globals.Config.Music.RandomSongEachTransition = val);

			AddLabel("Misc Options");
			AddCheckBox("Building Costs", "Farm buildings that Robin can build for the player choose from a random pool of resources.", () => Globals.Config.RandomizeBuildingCosts, (bool val) => Globals.Config.RandomizeBuildingCosts = val);
			AddCheckBox("Animal Skins", "You might get a surprise from Marnie.", () => Globals.Config.RandomizeAnimalSkins, (bool val) => Globals.Config.RandomizeAnimalSkins = val);
			AddCheckBox("Forageables", "Forageables for every season and location are now randomly selected. Every forageable appears at least once per year.", () => Globals.Config.RandomizeForagables, (bool val) => Globals.Config.RandomizeForagables = val);
			AddCheckBox("Intro Text", "Replace portions of the intro cutscene with Mad Libs style text.", () => Globals.Config.RandomizeIntroStory, (bool val) => Globals.Config.RandomizeIntroStory = val);
			AddCheckBox("Quests", "Randomly select quest givers, required items, and rewards.", () => Globals.Config.RandomizeQuests, (bool val) => Globals.Config.RandomizeQuests = val);
			AddCheckBox("Rain", "Replace rain with a variant (Skulls/Junimos/Cats and Dogs/etc).", () => Globals.Config.RandomizeRain, (bool val) => Globals.Config.RandomizeRain = val);
		}

		private void AddLabel(string name, string desc = "")
		{
			api.RegisterLabel(ModManifest, name, desc);
		}

		private void AddCheckBox(string name, string desc, Func<bool> get, Action<bool> set)
		{
			api.RegisterSimpleOption(ModManifest, name, desc, get, set);
		}
	}

}
