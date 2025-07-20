using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Randomizes crops
    /// </summary>
    public class CropRandomizer
	{
        /// <summary>
        /// Helper class to simplify randomizing crops
        /// </summary>
        private class CropRandomizationInfoObject
        {
            public List<string> RegrowableSeedIdsToRandomize { get; set; }
            public List<string> NonRegrowableSeedIdsToRandomize { get; set; }
            public List<string> StaticSeedsToRandomize { get; set; }

            public List<CropItem> RandomizedCrops { get; set; }
            public List<CropItem> RandomizedVegetables { get; set; }
            public List<CropItem> RandomizedFlowers { get; set; }

            public CropRandomizationInfoObject()
            {
                RegrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
                    .Where(x => x.Randomize && x.RegrowsAfterHarvest && x.ShuffleBetweenSeeds)
                    .Select(x => x.Id)
                    .ToList();
                NonRegrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
                    .Where(x => x.Randomize && !x.RegrowsAfterHarvest && x.ShuffleBetweenSeeds)
                    .Select(x => x.Id)
                    .ToList();
                StaticSeedsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
                    .Where(x => x.Randomize && !x.ShuffleBetweenSeeds)
                    .Select(x => x.Id)
                    .ToList();

                RandomizedCrops = ItemList.GetCrops(true).Cast<CropItem>()
                    .Where(x => NonRegrowableSeedIdsToRandomize
                    .Union(RegrowableSeedIdsToRandomize)
                    .Union(StaticSeedsToRandomize).Contains(x.MatchingSeedItem.Id))
                    .ToList();
                RandomizedVegetables = RandomizedCrops.Where(x => !x.IsFlower).ToList();
                RandomizedFlowers = RandomizedCrops.Where(x => x.IsFlower).ToList();
            }
        }

        private static RNG Rng { get; set; }

        public static void Randomize(EditedObjects editedObjectInfo)
		{
			// If we're not randomizing names OR stats, no need to do anything
			if (!Globals.Config.Crops.RandomizeNames &&
				!Globals.Config.Crops.RandomizeStats)
			{
				// Currently this also handles fish dishes... so call this here
				// so we don't accidently skip any renames here
                SetUpCookedFood(editedObjectInfo);
				return;
			}

            Rng = RNG.GetFarmRNG(nameof(CropRandomizer));
			RandomizeCrops(editedObjectInfo);

			WriteToSpoilerLog();
		}

		/// <summary>
		/// Randomizes the crops - currently only does prices, and only for seasonal crops
		/// </summary>
		/// <param name="editedObjectInfo">The edited object information</param>
		private static void RandomizeCrops(EditedObjects editedObjectInfo)
		{
			var cropInfoObject = new CropRandomizationInfoObject();

			RandomizeCropStats(cropInfoObject);

			List<string> vegetableNames = NameAndDescriptionRandomizer
				.GenerateVegetableNames(cropInfoObject.RandomizedVegetables.Count + 1);
			List<string> cropDescriptions = NameAndDescriptionRandomizer
				.GenerateCropDescriptions(cropInfoObject.RandomizedCrops.Count);
			SetCropAndSeedInformation(
				editedObjectInfo,
				cropInfoObject.RandomizedVegetables,
				vegetableNames,
				cropDescriptions); // Note: It removes the descriptions it uses from the list after assigning them- may want to edit later

			SetUpCoffee(editedObjectInfo, vegetableNames[^1]);
			SetUpRice(editedObjectInfo);

			SetCropAndSeedInformation(
				editedObjectInfo,
				cropInfoObject.RandomizedFlowers,
				NameAndDescriptionRandomizer.GenerateFlowerNames(cropInfoObject.RandomizedFlowers.Count),
				cropDescriptions); // Note: It removes the descriptions it uses from the list after assigning them- may want to edit later

			SetUpCookedFood(editedObjectInfo);
		}

		private static void RandomizeCropStats(CropRandomizationInfoObject cropInfoObject)
		{
			if (!Globals.Config.Crops.RandomizeStats) { return; }

            List<string> regrowableSeedIdsToRandomizeCopy =
				new(cropInfoObject.RegrowableSeedIdsToRandomize);
            List<string> nonRegrowableSeedIdsToRandomizeCopy =
                new(cropInfoObject.NonRegrowableSeedIdsToRandomize);

            // Fill up a dictionary to remap the seed values
            Dictionary<string, string> seedMappings = new(); // Original value, new value

			cropInfoObject.RegrowableSeedIdsToRandomize.ForEach(seedId => 
				seedMappings.Add(seedId, Rng.GetAndRemoveRandomValueFromList(regrowableSeedIdsToRandomizeCopy)));
			cropInfoObject.NonRegrowableSeedIdsToRandomize.ForEach(seedId =>
				seedMappings.Add(seedId, Rng.GetAndRemoveRandomValueFromList(nonRegrowableSeedIdsToRandomizeCopy)));
            cropInfoObject.StaticSeedsToRandomize.ForEach(seedId =>
                seedMappings.Add(seedId, seedId));  // These are static, so map it to itself!

            // Loop through the dictionary and reassign the values, keeping the seasons the same as before
            foreach (KeyValuePair<string, string> seedMapping in seedMappings)
            {
                string originalValue = seedMapping.Key;
                string newValue = seedMapping.Value;

                CropData newCropGrowthInfo = DataLoader.Crops(Game1.content)[newValue.ToString()].DeepClone();
                newCropGrowthInfo.Seasons = DataLoader.Crops(Game1.content)[originalValue.ToString()].DeepClone().Seasons;
                newCropGrowthInfo.DaysInPhase = GetRandomGrowthStages(newCropGrowthInfo.DaysInPhase.Count);
                newCropGrowthInfo.HarvestMethod = Rng.NextBoolean(10)
                    ? HarvestMethod.Scythe
                    : HarvestMethod.Grab;
                newCropGrowthInfo.RegrowDays = newCropGrowthInfo.RegrowDays == -1
                    ? -1
                    : Rng.NextIntWithinRange(1, 7);

                var originalSeedItem = ItemList.Items[originalValue] as SeedItem;
                originalSeedItem.CropGrowthInfo = newCropGrowthInfo;
            }
        }

		/// <summary>
		/// Gets a list of randomly generated growth stages
		/// </summary>
		/// <param name="numberOfStages"></param>
		/// <returns>A list of integers, totaling up to a max of 12</returns>
		private static List<int> GetRandomGrowthStages(int numberOfStages)
		{
			if (numberOfStages <= 0)
			{
				Globals.ConsoleError("Tried to pass an invalid number of growth stages when randomizing crops.");
				return new List<int>();
			}

			int maxValuePerStage = 12 / numberOfStages;
			List<int> growthStages = new();

			for (int i = 0; i < numberOfStages; i++)
			{
				growthStages.Add(Rng.NextIntWithinRange(1, maxValuePerStage));
			}

			return growthStages;
		}

		/// <summary>
		/// Sets the crop and seed display name, price, and description
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="crops">The crops to set</param>
		/// <param name="randomNames">The random names to give the crops</param>
		private static void SetCropAndSeedInformation(
			EditedObjects editedObjectInfo,
			List<CropItem> crops,
			List<string> randomNames,
			List<string> randomDescriptions)
		{
			for (int i = 0; i < crops.Count; i++)
			{
                CropItem crop = crops[i];
				SeedItem seed = crop.MatchingSeedItem;

                ObjectData cropObject = Game1.objectData[crop.Id].DeepClone();
                ObjectData seedObject = Game1.objectData[seed.Id].DeepClone();

                // Set up new descriptions and prices if we are randomizing stats
                if (Globals.Config.Crops.RandomizeStats)
				{
                    string seasonsString = Globals.GetTranslation(
						"crop-tooltip-seasons",
						new { seasons = seed.GetSeasonsStringForDisplay() });
                    string description = Rng.GetAndRemoveRandomValueFromList(randomDescriptions);

                    seed.Price = GetRandomSeedPrice();
                    seedObject.Description = seed.Description;
                    seedObject.Price = seed.Price;

                    crop.Description = $"{description} {seasonsString}";
                    crop.Price = CalculateCropPrice(seed);
                    cropObject.Description = crop.Description;
                    cropObject.Price = crop.Price;
                }

				// Set up the override and display names if we are randomizing names
				if (Globals.Config.Crops.RandomizeNames)
				{
                    string newCropName = randomNames[i];

                    crop.OverrideName = newCropName;
                    cropObject.DisplayName = crop.Name;

                    seed.OverrideDisplayName = seed.IsTrellisCrop
						? Globals.GetTranslation("trellis-text", new { itemName = newCropName })
						: Globals.GetTranslation("seed-text", new { itemName = newCropName });
                    seed.OverrideName = seed.IsTrellisCrop
                        ? $"{newCropName} Starter"
                        : $"{newCropName} Seeds";
                    seedObject.DisplayName = seed.Name;
                }

                editedObjectInfo.ObjectsReplacements[crop.Id] = cropObject;
                editedObjectInfo.ObjectsReplacements[seed.Id] = seedObject;
            }
		}

		/// <summary>
		/// Sets up the coffee beans and coffee objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="coffeeItemName">The name of the coffee item</param>
		private static void SetUpCoffee(EditedObjects editedObjectInfo, string coffeeItemName)
		{
			if (!Globals.Config.Crops.RandomizeNames) { return; }

			Item coffee = ObjectIndexes.Coffee.GetItem();
			coffee.OverrideName = $"Hot {coffeeItemName}";
            coffee.CoffeeIngredient = coffeeItemName; // Used for the description of the coffee bean

			ObjectData coffeeObject = Game1.objectData[coffee.Id].DeepClone();
            coffeeObject.DisplayName = Globals.GetTranslation("item-coffee-name", new { itemName = coffeeItemName });
            editedObjectInfo.ObjectsReplacements[coffee.Id] = coffeeObject;

            SeedItem coffeeBean = ObjectIndexes.CoffeeBean.GetItem() as SeedItem;
			coffeeBean.OverrideName = $"{coffeeItemName} Bean";
			coffeeBean.OverrideDisplayName = Globals.GetTranslation("coffee-bean-name", new { itemName = coffeeItemName });

            ObjectData coffeeBeanObject = Game1.objectData[coffeeBean.Id].DeepClone();
			coffeeBeanObject.DisplayName = coffeeBean.Name;
			coffeeBeanObject.Description = coffeeBean.Description;
            editedObjectInfo.ObjectsReplacements[coffeeBean.Id] = coffeeBeanObject;
        }

		/// <summary>
		/// Sets up the rice objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpRice(EditedObjects editedObjectInfo)
		{
            if (!Globals.Config.Crops.RandomizeNames) { return; }

            CropItem unmilledRice = ObjectIndexes.UnmilledRice.GetItem() as CropItem;
			string riceName = unmilledRice.OverrideName;
			unmilledRice.OverrideName = $"Unmilled {riceName}";
			unmilledRice.OverrideDisplayName = Globals.GetTranslation("unmilled-rice-name", new { itemName = riceName });

            ObjectData unmilledRiceObject = Game1.objectData[unmilledRice.Id].DeepClone();
			unmilledRiceObject.DisplayName = unmilledRice.Name;
            editedObjectInfo.ObjectsReplacements[unmilledRice.Id] = unmilledRiceObject;

			Item rice = ObjectIndexes.Rice.GetItem();
            rice.OverrideName = riceName;

            ObjectData riceObject = Game1.objectData[rice.Id].DeepClone();
            riceObject.DisplayName = rice.Name;
            editedObjectInfo.ObjectsReplacements[rice.Id] = riceObject;
        }

		/// <summary>
		/// Changes the names of the cooked food to match those of the objects themselves
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpCookedFood(EditedObjects editedObjectInfo)
		{
			if (Globals.Config.Crops.RandomizeNames)
			{
				CookedItem.GetAllCropDishes().ForEach(cropDish =>
				{
					cropDish.CalculateOverrideName();

                    ObjectData cropDishObject = Game1.objectData[cropDish.Id].DeepClone();
                    cropDishObject.DisplayName = cropDish.Name;
                    editedObjectInfo.ObjectsReplacements[cropDish.Id] = cropDishObject;
                });
			}

			if (Globals.Config.Fish.RandomizeNames)
			{
                CookedItem.GetAllFishDishes().ForEach(fishDish =>
                {
					fishDish.CalculateOverrideName();

                    ObjectData fishDishObject = Game1.objectData[fishDish.Id].DeepClone();
                    fishDishObject.DisplayName = fishDish.Name;
                    editedObjectInfo.ObjectsReplacements[fishDish.Id] = fishDishObject;
                });
			}
		}

		/// <summary>
		/// Get a random seed price - the weighted values are as follows:
		/// 10 - 30: 40%
		/// 30 - 60: 30%
		/// 60 - 90: 15%
		/// 90 - 120: 10%
		/// 120 - 150: 5%
		/// </summary>
		/// <returns>
		/// The generated number - this will be an even number because the seed price that
		/// we need to report is actually the sell value, which is half of the price we will
		/// generate here
		/// </returns>
		private static int GetRandomSeedPrice()
		{
			int generatedValue = Rng.NextIntWithinRange(1, 100);
			int baseValue;
			if (generatedValue < 41)
			{
				baseValue = Rng.NextIntWithinRange(10, 30);
			}
			else if (generatedValue < 71)
			{
				baseValue = Rng.NextIntWithinRange(31, 60);
			}
			else if (generatedValue < 86)
			{
				baseValue = Rng.NextIntWithinRange(61, 90);
			}
			else if (generatedValue < 96)
			{
				baseValue = Rng.NextIntWithinRange(91, 120);
			}
			else
			{
				baseValue = Rng.NextIntWithinRange(121, 150);
			}

			return baseValue / 2; // We need to store the sell price, not the buy price
		}

		/// <summary>
		/// Calculates the seed price based on the seed growth info and price
		/// </summary>
		/// <param name="seed">The seed</param>
		/// <returns>
		/// Returns a value based on a random multiplier, regrowth days, and
		/// potential amount per harvest
		/// </returns>
		private static int CalculateCropPrice(SeedItem seed)
		{
			int seedPrice = seed.Price * 2; // The amount we store here is half of what we want to base this off of
			CropData growthInfo = seed.CropGrowthInfo;

			double multiplier;
			if (seedPrice < 31) { multiplier = Rng.NextIntWithinRange(15, 40) / (double)10; }
			else if (seedPrice < 61) { multiplier = Rng.NextIntWithinRange(15, 35) / (double)10; }
			else if (seedPrice < 91) { multiplier = Rng.NextIntWithinRange(15, 30) / (double)10; }
			else if (seedPrice < 121) { multiplier = Rng.NextIntWithinRange(15, 25) / (double)10; }
			else { multiplier = Rng.NextIntWithinRange(15, 20) / (double)10; }

			double regrowthDaysMultiplier;
			switch (growthInfo.RegrowDays)
			{
				case 1: regrowthDaysMultiplier = 0.3; break;
				case 2: regrowthDaysMultiplier = 0.4; break;
				case 3: regrowthDaysMultiplier = 0.5; break;
				case 4: regrowthDaysMultiplier = 0.6; break;
				case 5: regrowthDaysMultiplier = 0.7; break;
				case 6: regrowthDaysMultiplier = 0.8; break;
				case 7: regrowthDaysMultiplier = 0.9; break;
				default: regrowthDaysMultiplier = 1; break;
			}

			double amountPerHarvestMultiplier = 1;
			switch (growthInfo.HarvestMinStack)
			{
				case 0: break;
				case 1: break;
				case 2: amountPerHarvestMultiplier = 0.6; break;
				case 3: amountPerHarvestMultiplier = 0.45; break;
				case 4: amountPerHarvestMultiplier = 0.3; break;
				default:
					Globals.ConsoleError($"Unexpected seed with more than 4 minimum extra crops: {seed.Id}");
					break;
			}
			if (seed.CanGetExtraCrops && amountPerHarvestMultiplier == 1)
			{
				amountPerHarvestMultiplier = 0.9;
			}

			return (int)(seedPrice * multiplier * regrowthDaysMultiplier * amountPerHarvestMultiplier);
		}

		/// <summary>
		/// Writes relevant crop changes to the spoiler log
		/// </summary>
		private static void WriteToSpoilerLog()
		{
			Globals.SpoilerWrite("==== CROPS AND SEEDS ====");
            foreach (SeedItem seedItem in ItemList.GetSeeds().Cast<SeedItem>())
			{
				if (seedItem.ObjectIndex == ObjectIndexes.CoffeeBean || 
					seedItem.ObjectIndex == ObjectIndexes.AncientSeeds) 
				{ 
					continue; 
				}

				CropItem cropItem = (CropItem)ItemList.Items[seedItem.CropId];
				Globals.SpoilerWrite($"{cropItem.Id}: {cropItem.Name}");

				if (Globals.Config.Crops.RandomizeStats)
				{
                    Globals.SpoilerWrite($"- Seed Buy Price: {seedItem.Price * 2}G - Crop Sell Price: {cropItem.Price}G");
                    Globals.SpoilerWrite($"{seedItem.Id} ({seedItem.Name}): {seedItem.Description}");
                    Globals.SpoilerWrite("---");
                }
			}
			Globals.SpoilerWrite("");
		}
	}
}
