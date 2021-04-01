using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Randomizer
{
	class ShopMenuAdjustments
	{
		static readonly int maxValue = int.MaxValue;


		/// <summary>
		/// Fixes sale price for saplings so that they are based on the randomly generated item they produce. Not governed by config.
		/// Additionally, adds a random "Item of the Week" which is obtainable in early game. Governed by RandomizeMainShops config.
		/// </summary>
		/// <param name="menu">The ShopMenu passed in from the MenuChanged event fired when opening Pierre's shop</param>
		public static void AdjustSeedShopStock(ShopMenu menu)
		{
			Random ShopRNG = GetRNG(Game1.Date.TotalDays + 1 / 7);

			// Fix sapling prices
			foreach (KeyValuePair<ISalable, int[]> sapling in menu.itemPriceAndStock.Where(keyValuePair => keyValuePair.Key.Name.Contains("Sapling")).ToList())
			{
				menu.itemPriceAndStock[sapling.Key] = new[] { sapling.Key.salePrice(), maxValue};
			}

			// Stop early if RandomizeMainShops is turned off
			if (!Globals.Config.Shops.RandomizeMainShops) return;

			// Build list of possible items
			List<int> excludedItems = new List<int> { (int)ObjectIndexes.Clay, (int)ObjectIndexes.Oil, (int)ObjectIndexes.Sugar, (int)ObjectIndexes.Vinegar, (int)ObjectIndexes.Hay, (int)ObjectIndexes.WheatFlour };
			List<Item> validItems = ItemList.GetItemsInCraftableCategory(CraftableCategories.Easy)
				.Concat(ItemList.GetItemsInCraftableCategory(CraftableCategories.EasyAndNeedMany))
				.Concat(ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.MediumTimeRequirements, excludedItems)) // Filter out stuff Pierre already sells or that someone else will sell
				.Where(x => x.Id >= 0).Distinct().ToList(); // Filter out duplicate entries and BigCraftables (negative IDs)

			// Select and add item
			Item item = validItems[ShopRNG.Next(validItems.Count)];
			int numToSell;

			// If item is in a 'NeedMany' category, increase the number to sell
			if (item.IsCraftable && ((item as CraftableItem).Category == CraftableCategories.EasyAndNeedMany))
			{
				numToSell = Range.GetRandomValue(20, 30);
			}
			else
			{
				numToSell = Range.GetRandomValue(3, 15);
			}

			StardewValley.Object itemOfTheWeek = new StardewValley.Object(Vector2.Zero, item.Id, numToSell);

			// Certain items don't have a salePrice or it is too low
			int salePrice = Math.Min(20, (int)(itemOfTheWeek.salePrice() * Game1.MasterPlayer.difficultyModifier));

			menu.itemPriceAndStock.Add(itemOfTheWeek, new[] { salePrice, numToSell });
			menu.forSale.Insert(0, itemOfTheWeek);

		}

		/// <summary>
		/// Fixes sale prices for randomized gear so that nothing sells for more than it's worth.
		/// Not governed by config options.
		/// </summary>
		/// <param name="menu">The ShopMenu passed in from the MenuChanged event fired when opening Marlon's shop.</param>
		public static void AdjustAdventureShopStock(ShopMenu menu)
		{
			menu.itemPriceAndStock = menu.itemPriceAndStock.ToDictionary(
						item => item.Key,
						item => new[] { item.Key.salePrice(), maxValue }
				);
		}

		/// <summary>
		/// Adds Clay to Robin's shop in case it is needed and the player is having trouble getting enough.
		/// Not governed by config options.
		/// </summary>
		/// <param name="menu">The ShopMenu passed in from the MenuChanged event fired when opening Robin's shop.</param>
		public static void AdjustCarpenterShopStock(ShopMenu menu)
		{
			StardewValley.Object clay = new StardewValley.Object(Vector2.Zero, (int)ObjectIndexes.Clay, maxValue);
			menu.itemPriceAndStock.Add(clay, new[] { 40, maxValue });
			menu.forSale.Insert(2, clay);
		}

		/// <summary>
		/// Modifies Krobus' stock to be a random assortment of mid-game items
		/// Governed by RandomizeMainShops option.
		/// </summary>
		/// <param name="menu">The ShopMenu passed in from the MenuChanged event fired when opening Krobus' shop.</param>
		public static void AdjustSewerShopStock(ShopMenu menu)
		{
			if (!Globals.Config.Shops.RandomizeMainShops) return;

			// Stock rotates on a weekly basis
			Random ShopRNG = GetRNG(Game1.Date.TotalDays + 1 / 7);

			// Clean out existing shop stock
			EmptyStock(menu);

			// Items to leave out
			List<int> excludedItems = new List<int>
				{   
					(int)ObjectIndexes.Clam, (int)ObjectIndexes.Cockle, (int)ObjectIndexes.Mussel, (int)ObjectIndexes.Oyster, (int)ObjectIndexes.Rice,
					(int)ObjectIndexes.Keg, (int)ObjectIndexes.GoldOre, (int)ObjectIndexes.IronOre, (int)ObjectIndexes.CopperOre 
				};

			// Sort out BigCraftables until I know what to do with them
			List<Item> validItems = ItemList.GetItemsAtDifficulty(ObtainingDifficulties.MediumTimeRequirements, excludedItems)
				.Concat(ItemList.GetItemsInCraftableCategory(CraftableCategories.Moderate, excludedItems))
				.Concat(ItemList.GetItemsInCraftableCategory(CraftableCategories.ModerateAndNeedMany, excludedItems))
				.Where(x => x.Id >= 0).ToList();

			// List<Item> bigCraftables = validItems.Where(x => x.Id < 0).ToList();
			// validItems = validItems.Where(x => x.Id >= 0).ToList();

			int numItemsToSell = Range.GetRandomValue(7, 12);

			for (int i = 0; i < numItemsToSell; i++)
			{

				Item item = validItems[ShopRNG.Next(validItems.Count)];
				validItems.Remove(item);

				ISalable obj;

				int numToSell;

				// Rings will not function if not created with the Ring constructor
				if (item.IsRing)
				{
					numToSell = maxValue;
					obj = new StardewValley.Objects.Ring(item.Id) { Stack = numToSell };
				}
				// If item is in a 'NeedMany' category, increase the number to sell
				else if (item.IsCraftable && (item as CraftableItem).Category == CraftableCategories.ModerateAndNeedMany)
				{
					numToSell = Range.GetRandomValue(20, 30);
					obj = new StardewValley.Object(item.Id, numToSell);
				}
				else
				{
					numToSell = Range.GetRandomValue(3, 15);
					obj = new StardewValley.Object(item.Id, numToSell);
				}

				//Globals.ConsoleTrace($"Item ID: {item.Id} | Expected Item: {Enum.GetName(typeof(ObjectIndexes), item.Id)} | Display Name: {obj.DisplayName}");

				menu.itemPriceAndStock.Add(obj, new[] { obj.salePrice(), numToSell });
				menu.forSale.Add(obj);
			}

			// currently not working - how to instantiate big craftables?
			//foreach (Item item in bigCraftables)
			//{
			//	ISalable obj;
			//	if (item.Id > 0 && !(item.Id == 12))
			//	{
			//		obj = new StardewValley.Object(Vector2.Zero, item.Id, maxValue);
			//		menu.itemPriceAndStock.Add(obj, new[] { 1, 1 });
			//		menu.forSale.Add(obj);
			//	}
			//	else
			//	{
			//		obj = new StardewValley.Object(Vector2.Zero, item.Id) { Stack = maxValue };
			//		menu.itemPriceAndStock.Add(obj, new[] { 1, 1 });
			//		menu.forSale.Add(obj);
			//	}
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="menu">The ShopMenu passed in from the MenuChanged event fired when opening Sandy's shop.</param>
		public static void AdjustOasisShopStock(ShopMenu menu)
		{
			if (!Globals.Config.Shops.RandomizeMainShops) return;

			// Stock rotates on a weekly basis
			Random ShopRNG = GetRNG(Game1.Date.TotalDays + 1 / 7);

			// Clear out existing stock
			EmptyStock(menu);

			// Exclude fish and crops
			List<int> excludedItems = ItemList.GetFish()
				.Concat(ItemList.GetCrops())
				.Concat(ItemList.GetForagables())
				.Concat(ItemList.GetSeeds())
				.Concat(ItemList.GetCookedItems())
				.Select(x => x.Id)
				.Concat(new List<int> { (int)ObjectIndexes.MermaidsPendant, (int)ObjectIndexes.JackOLantern })
				.ToList();

			// Filter out BigCraftables until I know what to do with them
			List<Item> validItems = ItemList.GetItemsAtDifficulty(ObtainingDifficulties.LargeTimeRequirements, excludedItems)
				.Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.RareItem, excludedItems))
				.Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.EndgameItem, excludedItems))
				.Concat(ItemList.GetItemsInCraftableCategory(CraftableCategories.Difficult, excludedItems))
				.Concat(ItemList.GetItemsInCraftableCategory(CraftableCategories.DifficultAndNeedMany, excludedItems))
				// filter out bigcraftables
				.Where(x => x.Id >= 0).ToList();

			int numItemsToSell = Range.GetRandomValue(7, 12);

			for (int i = 0; i < numItemsToSell; i++)
			{
				Item item = validItems[ShopRNG.Next(validItems.Count)];
				validItems.Remove(item);

				int numToSell = Range.GetRandomValue(3, 15);

				// If item is in a 'NeedMany' category, increase the number to sell
				if (item.IsCraftable && (item as CraftableItem).Category == CraftableCategories.DifficultAndNeedMany)
				{
					numToSell = Range.GetRandomValue(20, 30);
				}

				ISalable obj = new StardewValley.Object(item.Id, numToSell);

				menu.itemPriceAndStock.Add(obj, new[] { obj.salePrice(), numToSell });
				menu.forSale.Add(obj);
			}
		}

		/// <summary>
		/// Modifies Gus' stock to be a random assortment of cooked items and recipes.
		/// Governed by RandomizeMiscShops option.
		/// </summary>
		/// <param name="menu">The ShopMenu passed in from the MenuChanged event fired when opening Gus' shop.</returns>
		public static void AdjustSaloonShopStock(ShopMenu menu)
		{
			if (!Globals.Config.Shops.RandomizeMiscShops) return;

			// Stock rotates on a weekly basis
			Random ShopRNG = GetRNG(Game1.Date.TotalDays + 1 / 7);

			// Clean out existing shop stock
			EmptyStock(menu);

			// Add beer - the one constant to Gus' stock
			StardewValley.Object beer = new StardewValley.Object(Vector2.Zero, (int)ObjectIndexes.Beer, maxValue);
			menu.itemPriceAndStock.Add(beer, new[] { beer.salePrice(), maxValue });
			menu.forSale.Add(beer);

			// Random Cooked Items - pick 3-5 random dishes each week
			List<Item> gusFoodList = ItemList.GetCookedItems();
			for (int i = 0; i < ShopRNG.Next(3, 6); i++)
			{
				int selectedIndex = ShopRNG.Next(gusFoodList.Count);
				Item selectedItem = gusFoodList[selectedIndex];
				gusFoodList.RemoveAt(selectedIndex);

				StardewValley.Object obj = new StardewValley.Object(Vector2.Zero, selectedItem.Id, maxValue);
				menu.itemPriceAndStock.Add(obj, new[] { obj.salePrice(), maxValue });
				menu.forSale.Add(obj);
			}

			// Random Cooking Recipes - pick 3-5 random recipes each week
			List<Item> gusRecipeList = ItemList.GetCookedItems();
			for (int i = 0; i < ShopRNG.Next(3, 6); i++)
			{
				int selectedIndex = ShopRNG.Next(gusRecipeList.Count);
				Item selectedItemRecipe = gusRecipeList[selectedIndex];
				gusRecipeList.RemoveAt(selectedIndex);

				StardewValley.Object recipe = new StardewValley.Object(selectedItemRecipe.Id, 1, isRecipe: true);

				// Don't add if player already knows recipe
				string recipeName = recipe.Name.Substring(0, recipe.Name.IndexOf("Recipe") - 1);
				if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
				{

					menu.itemPriceAndStock.Add(recipe, new[] { recipe.salePrice(), 1 });
					menu.forSale.Add(recipe);
				}
			}
		}

		/// <summary>
		/// Creates the RNG used by shops. Seeded by farm name and time interval - shops can change stock on a daily or weekly basis
		/// </summary>
		/// <returns></returns>
		private static Random GetRNG(int time)
		{
			byte[] seedvar = (new SHA1Managed()).ComputeHash(System.Text.Encoding.UTF8.GetBytes(Game1.player.farmName.Value));
			int seed = BitConverter.ToInt32(seedvar, 0) + time;

			return new Random(seed);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="menu"></param>
		private static void EmptyStock(ShopMenu menu)
		{
			while (menu.itemPriceAndStock.Any())
			{
				ISalable obj = menu.itemPriceAndStock.Keys.ElementAt(0);
				menu.itemPriceAndStock.Remove(obj);
				menu.forSale.Remove(obj);
			}
		}
	}
}