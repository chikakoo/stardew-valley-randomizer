﻿using Force.DeepCloner;
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
		public static void Randomize(EditedObjects editedObjectInfo)
		{
			RandomizeCrops(editedObjectInfo);

			WriteToSpoilerLog();
		}

		/// <summary>
		/// Randomizes the crops - currently only does prices, and only for seasonal crops
		/// </summary>
		/// <param name="editedObjectInfo">The edited object information</param>
		private static void RandomizeCrops(EditedObjects editedObjectInfo)
		{
			List<int> regrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && x.RegrowsAfterHarvest)
				.Select(x => x.Id)
				.ToList();
			List<int> regrowableSeedIdsToRandomizeCopy = new(regrowableSeedIdsToRandomize);

			List<int> nonRegrowableSeedIdsToRandomize = ItemList.GetSeeds().Cast<SeedItem>()
				.Where(x => x.Randomize && !x.RegrowsAfterHarvest)
				.Select(x => x.Id)
				.ToList();
			List<int> nonRegrowableSeedIdsToRandomizeCopy = new(nonRegrowableSeedIdsToRandomize);

			// Fill up a dictionary to remap the seed values
			Dictionary<int, int> seedMappings = new(); // Original value, new value

			foreach (int originalRegrowableSeedId in regrowableSeedIdsToRandomize)
			{
				seedMappings.Add(originalRegrowableSeedId, Globals.RNGGetAndRemoveRandomValueFromList(regrowableSeedIdsToRandomizeCopy));
			}

			foreach (int originalNonRegrowableSeedId in nonRegrowableSeedIdsToRandomize)
			{
				seedMappings.Add(originalNonRegrowableSeedId, Globals.RNGGetAndRemoveRandomValueFromList(nonRegrowableSeedIdsToRandomizeCopy));
			}

			// Loop through the dictionary and reassign the values, keeping the seasons the same as before
			foreach (KeyValuePair<int, int> seedMapping in seedMappings)
			{
				int originalValue = seedMapping.Key;
				int newValue = seedMapping.Value;

                CropData newCropGrowthInfo = DataLoader.Crops(Game1.content)[newValue.ToString()].DeepClone();
                newCropGrowthInfo.Seasons = DataLoader.Crops(Game1.content)[originalValue.ToString()].DeepClone().Seasons;
				newCropGrowthInfo.DaysInPhase = GetRandomGrowthStages(newCropGrowthInfo.DaysInPhase.Count);
				newCropGrowthInfo.HarvestMethod = Globals.RNGGetNextBoolean(10)
					? HarvestMethod.Scythe
					: HarvestMethod.Grab;
				newCropGrowthInfo.RegrowDays = newCropGrowthInfo.RegrowDays == -1
					? -1
					: Range.GetRandomValue(1, 7);

                if (!Globals.Config.Crops.Randomize) { continue; } // Preserve the original seasons/etc

				var originalSeedItem = ItemList.GetItem((ObjectIndexes)originalValue) as SeedItem;
				originalSeedItem.CropGrowthInfo = newCropGrowthInfo;
			}

			// Set the object info
			List<CropItem> randomizedCrops = ItemList.GetCrops(true).Cast<CropItem>()
				.Where(x => nonRegrowableSeedIdsToRandomize.Union(regrowableSeedIdsToRandomize).Contains(x.MatchingSeedItem.Id))
				.ToList();
			List<CropItem> vegetables = randomizedCrops.Where(x => !x.IsFlower).ToList();
			List<CropItem> flowers = randomizedCrops.Where(x => x.IsFlower).ToList();

			List<string> vegetableNames = NameAndDescriptionRandomizer.GenerateVegetableNames(vegetables.Count + 1);
			List<string> cropDescriptions = NameAndDescriptionRandomizer.GenerateCropDescriptions(randomizedCrops.Count);
			SetCropAndSeedInformation(
				editedObjectInfo,
				vegetables,
				vegetableNames,
				cropDescriptions); // Note: It removes the descriptions it uses from the list after assigning them- may want to edit later

			SetUpCoffee(editedObjectInfo, vegetableNames[^1]);
			SetUpRice(editedObjectInfo);

			SetCropAndSeedInformation(
				editedObjectInfo,
				flowers,
				NameAndDescriptionRandomizer.GenerateFlowerNames(flowers.Count),
				cropDescriptions); // Note: It removes the descriptions it uses from the list after assigning them- may want to edit later

			SetUpCookedFood(editedObjectInfo);
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
				growthStages.Add(Range.GetRandomValue(1, maxValuePerStage));
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

				string name = randomNames[i];
                string seasonsString = Globals.GetTranslation(
					"crop-tooltip-seasons", 
					new { seasons = seed.GetSeasonsStringForDisplay() });
				string description = Globals.RNGGetAndRemoveRandomValueFromList(randomDescriptions);
				crop.OverrideName = name;
				crop.Description = $"{description} {seasonsString}";

				seed.OverrideDisplayName = seed.IsTrellisCrop
					? Globals.GetTranslation("trellis-text", new { itemName = name })
					: Globals.GetTranslation("seed-text", new { itemName = name });
				seed.OverrideName = seed.IsTrellisCrop
					? $"{name} Starter"
					: $"{name} Seeds";

				seed.Price = GetRandomSeedPrice();
				crop.Price = CalculateCropPrice(seed);

				if (!Globals.Config.Crops.Randomize) { continue; }

				ObjectData cropObject = Game1.objectData[crop.Id.ToString()].DeepClone();
				cropObject.DisplayName = crop.Name;
				cropObject.Description = crop.Description;
				cropObject.Price = crop.Price;

                ObjectData seedObject = Game1.objectData[seed.Id.ToString()].DeepClone();
                seedObject.DisplayName = seed.Name;
                seedObject.Description = seed.Description;
                seedObject.Price = seed.Price;

                editedObjectInfo.ObjectsReplacements[crop.Id.ToString()] = cropObject;
				editedObjectInfo.ObjectsReplacements[seed.Id.ToString()] = seedObject;
			}
		}

		/// <summary>
		/// Sets up the coffee beans and coffee objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		/// <param name="coffeeItemName">The name of the coffee item</param>
		private static void SetUpCoffee(EditedObjects editedObjectInfo, string coffeeItemName)
		{
			if (!Globals.Config.Crops.Randomize) { return; }

			Item coffee = ItemList.Items[ObjectIndexes.Coffee];
			coffee.OverrideName = $"Hot {coffeeItemName}";
            coffee.CoffeeIngredient = coffeeItemName; // Used for the description of the coffee bean

			ObjectData coffeeObject = Game1.objectData[coffee.Id.ToString()].DeepClone();
            coffeeObject.DisplayName = Globals.GetTranslation("item-coffee-name", new { itemName = coffeeItemName });
            editedObjectInfo.ObjectsReplacements[coffee.Id.ToString()] = coffeeObject;

            SeedItem coffeeBean = ItemList.Items[ObjectIndexes.CoffeeBean] as SeedItem;
			coffeeBean.OverrideName = $"{coffeeItemName} Bean";
			coffeeBean.OverrideDisplayName = Globals.GetTranslation("coffee-bean-name", new { itemName = coffeeItemName });

            ObjectData coffeeBeanObject = Game1.objectData[coffeeBean.Id.ToString()].DeepClone();
			coffeeBeanObject.DisplayName = coffeeBean.Name;
			coffeeBeanObject.Description = coffeeBean.Description;
            editedObjectInfo.ObjectsReplacements[coffeeBean.Id.ToString()] = coffeeBeanObject;
		}

		/// <summary>
		/// Sets up the rice objects
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		public static void SetUpRice(EditedObjects editedObjectInfo)
		{
			CropItem unmilledRice = ItemList.Items[ObjectIndexes.UnmilledRice] as CropItem;
			string riceName = unmilledRice.OverrideName;
			unmilledRice.OverrideName = $"Unmilled {riceName}";
			unmilledRice.OverrideDisplayName = Globals.GetTranslation("unmilled-rice-name", new { itemName = riceName });

            ObjectData unmilledRiceObject = Game1.objectData[unmilledRice.Id.ToString()].DeepClone();
			unmilledRiceObject.DisplayName = unmilledRice.Name;
            editedObjectInfo.ObjectsReplacements[unmilledRice.Id.ToString()] = unmilledRiceObject;

			Item rice = ItemList.Items[ObjectIndexes.Rice];
            rice.OverrideName = riceName;

            ObjectData riceObject = Game1.objectData[rice.Id.ToString()].DeepClone();
            riceObject.DisplayName = rice.Name;
            editedObjectInfo.ObjectsReplacements[rice.Id.ToString()] = riceObject;
        }

		/// <summary>
		/// Changes the names of the cooked food to match those of the objects themselves
		/// </summary>
		/// <param name="editedObjectInfo">The object info containing changes to apply</param>
		private static void SetUpCookedFood(EditedObjects editedObjectInfo)
		{
			if (Globals.Config.Crops.Randomize)
			{
				CookedItem.GetAllCropDishes().ForEach(cropDish =>
				{
					cropDish.CalculateOverrideName();

                    ObjectData cropDishObject = Game1.objectData[cropDish.Id.ToString()].DeepClone();
                    cropDishObject.DisplayName = cropDish.Name;
                    editedObjectInfo.ObjectsReplacements[cropDish.Id.ToString()] = cropDishObject;
                });
			}

			if (Globals.Config.Fish.Randomize)
			{
                CookedItem.GetAllFishDishes().ForEach(fishDish =>
                {
					fishDish.CalculateOverrideName();

                    ObjectData fishDishObject = Game1.objectData[fishDish.Id.ToString()].DeepClone();
                    fishDishObject.DisplayName = fishDish.Name;
                    editedObjectInfo.ObjectsReplacements[fishDish.Id.ToString()] = fishDishObject;
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
			int generatedValue = Range.GetRandomValue(1, 100);
			int baseValue;
			if (generatedValue < 41)
			{
				baseValue = Range.GetRandomValue(10, 30);
			}
			else if (generatedValue < 71)
			{
				baseValue = Range.GetRandomValue(31, 60);
			}
			else if (generatedValue < 86)
			{
				baseValue = Range.GetRandomValue(61, 90);
			}
			else if (generatedValue < 96)
			{
				baseValue = Range.GetRandomValue(91, 120);
			}
			else
			{
				baseValue = Range.GetRandomValue(121, 150);
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
			if (seedPrice < 31) { multiplier = Range.GetRandomValue(15, 40) / (double)10; }
			else if (seedPrice < 61) { multiplier = Range.GetRandomValue(15, 35) / (double)10; }
			else if (seedPrice < 91) { multiplier = Range.GetRandomValue(15, 30) / (double)10; }
			else if (seedPrice < 121) { multiplier = Range.GetRandomValue(15, 25) / (double)10; }
			else { multiplier = Range.GetRandomValue(15, 20) / (double)10; }

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
			if (Globals.Config.Crops.Randomize)
			{
				Globals.SpoilerWrite("==== CROPS AND SEEDS ====");
				foreach (SeedItem seedItem in ItemList.GetSeeds())
				{
					if (seedItem.Id == (int)ObjectIndexes.CoffeeBean || seedItem.Id == (int)ObjectIndexes.AncientSeeds) { continue; }
					CropItem cropItem = (CropItem)ItemList.Items[(ObjectIndexes)seedItem.CropId];
					Globals.SpoilerWrite($"{cropItem.Id}: {cropItem.Name} - Seed Buy Price: {seedItem.Price * 2}G - Crop Sell Price: {cropItem.Price}G");
					Globals.SpoilerWrite($"{seedItem.Id}: {seedItem.Description}");
					Globals.SpoilerWrite("---");
				}
				Globals.SpoilerWrite("");
			}

			if (Globals.Config.RandomizeFruitTrees)
			{
				Globals.SpoilerWrite("==== FRUIT TREES ====");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.CherrySapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.AppleSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.OrangeSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.PeachSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.PomegranateSapling)}");
				Globals.SpoilerWrite($"{ItemList.GetItemName(ObjectIndexes.ApricotSapling)}");
				Globals.SpoilerWrite("");
			}
		}
	}
}
