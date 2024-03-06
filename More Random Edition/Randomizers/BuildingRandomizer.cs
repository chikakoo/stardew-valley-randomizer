﻿using StardewValley;
using StardewValley.GameData.Buildings;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Randomizes the building that Robin can build for you
    /// </summary>
    public class BuildingRandomizer
	{
        /// <summary>
        /// An item and how much to multiply the amount required by
        /// </summary>
        private class ItemAndMultiplier
        {
            public Item Item { get; set; }
            public int Multiplier { get; set; }

            public ItemAndMultiplier(Item item, int multiplier = 1)
            {
                Item = item;
                Multiplier = multiplier;
            }

            /// <summary>
            /// The number of items - note that this will NOT return the same value each time it's called!
            /// </summary>
            /// <param name="rng">The RNG to use</param>
            public int GenerateAmount(RNG rng)
            {
                return Item.GetAmountRequiredForCrafting(rng) * Multiplier;
            }
        }

		/// <summary>
		/// Tracks an item and the number of them needed for a recipe
		/// </summary>
		private class RequiredBuildingItem
		{
			public Item Item { get; set; }
			public int NumberOfItems { get; set; }

			public RequiredBuildingItem(Item item, int numberOfItems)
            {
                Item = item;
                NumberOfItems = numberOfItems;
            }
        }

        private static RNG Rng { get; set; }

        /// <summary>
        /// Randomize the buildings
        /// </summary>
        /// <returns>The dictionary to use to replace the assets</returns>
        public static Dictionary<string, BuildingData> Randomize()
		{
			Rng = RNG.GetFarmRNG(nameof(BuildingRandomizer));

            Dictionary<string, BuildingData> buildingData = DataLoader.Buildings(Game1.content);
			Dictionary<string, BuildingData> buildingChanges = new();

            List<int> idsToDisallowForAnimalBuildings = ItemList.GetAnimalProducts().Select(x => x.Id).ToList();
			idsToDisallowForAnimalBuildings.AddRange(new List<int>
			{
				(int)ObjectIndexes.GreenSlimeEgg,
				(int)ObjectIndexes.BlueSlimeEgg,
				(int)ObjectIndexes.RedSlimeEgg,
				(int)ObjectIndexes.PurpleSlimeEgg
			});

			Item resource1, resource2;
			ItemAndMultiplier itemChoice;
			List<ItemAndMultiplier> buildingMaterials;

            List<Buildings> buildings = Enum.GetValues(typeof(Buildings)).Cast<Buildings>().ToList();
            foreach (Buildings buildingType in buildings)
			{
				resource1 = ItemList.GetRandomResourceItem(Rng);
				resource2 = ItemList.GetRandomResourceItem(Rng, new int[] { resource1.Id });

				switch (buildingType)
				{
					case Buildings.Silo:
						buildingMaterials = new List<ItemAndMultiplier>
                        {
                            new(resource1, Rng.NextIntWithinRange(2, 3)),
                            new(resource2),
                            new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements))
                        };
						RandomizeAndAddBuilding("Silo", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.Mill:
						buildingMaterials = new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(resource2, 2),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements))
						};
                        RandomizeAndAddBuilding("Mill", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.ShippingBin:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements))
						});
                        buildingMaterials = new List<ItemAndMultiplier> { itemChoice };
                        RandomizeAndAddBuilding("Shipping Bin", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.Coop:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 5),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});
						buildingMaterials = new List<ItemAndMultiplier>
						{
							itemChoice,
							new(resource2, Rng.NextIntWithinRange(2, 3))
						};
                        RandomizeAndAddBuilding("Coop", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.BigCoop:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});
						buildingMaterials = new List<ItemAndMultiplier>
						{
							itemChoice,
							new(resource2, 7)
						};
                        RandomizeAndAddBuilding("Big Coop", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.DeluxeCoop:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 9),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.LargeTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});
						buildingMaterials = new List<ItemAndMultiplier>
						{
							itemChoice,
							new(resource2, 4)
						};
                        RandomizeAndAddBuilding("Deluxe Coop", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.Barn:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 5),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});
						buildingMaterials = new List<ItemAndMultiplier>
						{
							itemChoice,
							new(resource2, Rng.NextIntWithinRange(2, 3))
						};
                        RandomizeAndAddBuilding("Barn", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.BigBarn:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});
                        buildingMaterials = new List<ItemAndMultiplier>
						{
							itemChoice,
							new(resource2, 7)
						};
                        RandomizeAndAddBuilding("Big Barn", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.DeluxeBarn:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 9),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.LargeTimeRequirements, idsToDisallowForAnimalBuildings.ToArray()))
						});
                        buildingMaterials = new List<ItemAndMultiplier>
						{
							itemChoice,
							new(resource2, 4)
						};
                        RandomizeAndAddBuilding("Deluxe Barn", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.SlimeHutch:
						buildingMaterials = new List<ItemAndMultiplier>
						{
							new(resource1, 9),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements), 2),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.LargeTimeRequirements))
						};
                        RandomizeAndAddBuilding("Slime Hutch", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.Shed:
                        buildingMaterials = Rng.GetRandomValueFromList(new List<List<ItemAndMultiplier>> {
							new() { new ItemAndMultiplier(resource1, 5) },
							new() {
								new ItemAndMultiplier(resource1, 3),
								new ItemAndMultiplier(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements))
							}
						});
                        RandomizeAndAddBuilding("Shed", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.Cabin:
                        RandomizeAndAddBuilding("Cabin", GetRequiredItemsForCabin(), buildingData, buildingChanges);
                        break;
					case Buildings.Well:
						itemChoice = Rng.GetRandomValueFromList(new List<ItemAndMultiplier>
						{
							new(resource1, 3),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements))
						});
                        buildingMaterials = new List<ItemAndMultiplier> { itemChoice };
                        RandomizeAndAddBuilding("Well", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.FishPond:
                        buildingMaterials = new List<ItemAndMultiplier>
						{
							new(resource1, 2),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.SmallTimeRequirements), 2),
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.SmallTimeRequirements), 2)
						};
                        RandomizeAndAddBuilding("Fish Pond", buildingMaterials, buildingData, buildingChanges);
                        break;
					case Buildings.Stable:
                        buildingMaterials = new List<ItemAndMultiplier>
						{
							new(ItemList.GetRandomItemAtDifficulty(Rng, ObtainingDifficulties.MediumTimeRequirements), 2),
							new(resource1, 8),
						};
                        RandomizeAndAddBuilding("Stable", buildingMaterials, buildingData, buildingChanges);
                        break;
					default:
						Globals.ConsoleError($"Unhandled building: {buildingType}");
						continue;
				}
			}

			WriteToSpoilerLog(buildingChanges);
			return buildingChanges;
		}

		/// <summary>
		/// Randomizes the build cost and assigns the given build materials to the building
		/// </summary>
		/// <param name="buildingKey">The key of the building</param>
		/// <param name="buildMaterials">The materials to assign the building</param>
		/// <param name="buildingData">The original building data</param>
		/// <param name="buildingChanges">Our dictionary of building changes</param>
		private static void RandomizeAndAddBuilding(
			string buildingKey, 
			List<ItemAndMultiplier> buildMaterials,
            Dictionary<string, BuildingData> buildingData,
            Dictionary<string, BuildingData> buildingChanges)
		{
            BuildingData currentBuilding = buildingData[buildingKey];
            currentBuilding.BuildCost = ComputePrice(currentBuilding.BuildCost);
            currentBuilding.BuildMaterials = ComputeBuildMaterials(buildMaterials);

			// Cabins specifically have skins that need to be randomized as well
			if (buildingKey == "Cabin")
			{
				currentBuilding.Skins.ForEach(skin => 
					skin.BuildMaterials = ComputeBuildMaterials(GetRequiredItemsForCabin()));
			}

            buildingChanges.Add(buildingKey, currentBuilding);
        }

		/// <summary>
		/// Gets the required items for a cabin - applies to any cabin
		/// </summary>
		/// <returns />
		private static List<ItemAndMultiplier> GetRequiredItemsForCabin()
		{
			Item resource = ItemList.GetRandomResourceItem(Rng, new int[(int)ObjectIndexes.Hardwood]);
			Item easyItem = Rng.GetRandomValueFromList(
				ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.MediumTimeRequirements, new List<int> { resource.Id })
			);

			return Rng.GetRandomValueFromList(new List<List<ItemAndMultiplier>> {
				new() { new ItemAndMultiplier(resource, 2) },
				new() {
					new ItemAndMultiplier(resource),
					new ItemAndMultiplier(easyItem)
				}
			});
		}

        /// <summary>
        /// Computes the price based on the base money
        /// This is any value between the base money, plus or minus the money variable percentage
        /// </summary>
        /// <param name="baseMoneyRequired">The amount the building normally costs</param>
        /// <returns>The new price</returns>
        private static int ComputePrice(int baseMoneyRequired)
        {
            const double MoneyVariablePercentage = 0.25;
            int variableAmount = (int)(baseMoneyRequired * MoneyVariablePercentage);
            return Rng.NextIntWithinRange(baseMoneyRequired - variableAmount, baseMoneyRequired + variableAmount);
        }

        /// <summary>
        /// Returns the build materials based on the items and their multipliers
        /// Sums up the amounts of items if they have the same id to prevent errors
        /// </summary>
        /// <param name="itemsRequired">The items this building requires</param>
		/// <returns>The list of BuildingMaterials to be used by the building</returns>
        private static List<BuildingMaterial> ComputeBuildMaterials(List<ItemAndMultiplier> itemsRequired)
        {
            Dictionary<int, RequiredBuildingItem> requiredItemsDict = new();
            foreach (ItemAndMultiplier itemAndMultiplier in itemsRequired)
            {
                RequiredBuildingItem requiredItem = new(itemAndMultiplier.Item, itemAndMultiplier.GenerateAmount(Rng));
                int reqiredItemId = requiredItem.Item.Id;
                if (requiredItemsDict.ContainsKey(reqiredItemId))
                {
                    requiredItemsDict[reqiredItemId].NumberOfItems += requiredItem.NumberOfItems;
                }
                else
                {
                    requiredItemsDict.Add(reqiredItemId, requiredItem);
                }
            }

			List<BuildingMaterial> buildingMaterials = new();
			foreach(RequiredBuildingItem buildingItem in requiredItemsDict.Values)
			{
				buildingMaterials.Add(
					new BuildingMaterial
					{
						ItemId = buildingItem.Item.QualifiedId,
						Amount = buildingItem.NumberOfItems
					}
				);
			}

			return buildingMaterials;
        }

        /// <summary>
        /// Writes the buildings to the spoiler log
        /// </summary>
        /// <param name="buildingChanges">Info about the changed buildings</param>
        private static void WriteToSpoilerLog(Dictionary<string, BuildingData> buildingChanges)
		{
			if (!Globals.Config.RandomizeBuildingCosts) { return; }

			Globals.SpoilerWrite("==== BUILDINGS ====");
			foreach (var buildingData in buildingChanges)
			{
				string buildingName = buildingData.Key;
				BuildingData building = buildingData.Value;

				Globals.SpoilerWrite($"{buildingName} - {building.BuildCost}G");
				Globals.SpoilerWrite(GetRequiredItemsSpoilerString(building.BuildMaterials));
				Globals.SpoilerWrite("===");
			}
			Globals.SpoilerWrite("");
		}

        /// <summary>
        /// Gets the required items string for use in the spoiler log
        /// </summary>
        /// <param name="building">The building</param>
        /// <returns />
        private static string GetRequiredItemsSpoilerString(List<BuildingMaterial> buildingMaterials)
        {
			return string.Join(" - ", 
				buildingMaterials.Select(material =>
					$"{ItemList.GetItemFromStringId(material.Id).Name}: {material.Amount}"));
        }
    }
}
