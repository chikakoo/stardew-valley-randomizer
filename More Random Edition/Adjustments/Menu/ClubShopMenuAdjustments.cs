﻿using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    internal class ClubShopMenuAdjustments : ShopMenuAdjustments
    {
        /// <summary>
        /// Sell the following:
        /// - 3-5 furniture
        /// - 1 hat or clothing item
        /// - 2-3 higher-tier misc items
        /// - 1 totem type
        /// </summary>
        /// <param name="menu">The shop menu</param>
        public static void AdjustStock(ShopMenu menu)
        {
            if (!Globals.Config.Shops.RandomizeClubShop)
            {
                return;
            }

            Random shopRNG = Globals.GetWeeklyRNG();
            EmptyStock(menu);

            AddFurniture(menu, shopRNG);
            AddHatOrClothing(menu, shopRNG);
            AddBigCraftable(menu, shopRNG);
            AddMiscItems(menu, shopRNG);
            AddTotem(menu, shopRNG);
        }

        /// <summary>
        /// Adds 3-5 random furniture items
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddFurniture(ShopMenu menu, Random shopRNG)
        {
            var numberOfFurniture = Range.GetRandomValue(3, 5, shopRNG);
            var furnitureToSell = ItemList.GetRandomFurnitureToSell(shopRNG, numberOfFurniture);
            furnitureToSell.ForEach(item => AddStock(menu, item, salePrice: GetSalePrice(item)));
        }

        /// <summary>
        /// Adds either a hat or a clothing item
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddHatOrClothing(ShopMenu menu, Random shopRNG)
        {
            var randomHat = ItemList.GetRandomHatsToSell(shopRNG, numberToGet: 1).First();
            var randomClothing = ItemList.GetRandomClothingToSell(shopRNG, numberToGet: 1).First();
            var hatOrClothingToSell = Globals.RNGGetNextBoolean(50, shopRNG) ? randomHat : randomClothing;
            AddStock(menu, hatOrClothingToSell, salePrice: GetSalePrice(hatOrClothingToSell));
        }

        /// <summary>
        /// Adds a BigCraftiable item
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddBigCraftable(ShopMenu menu, Random shopRNG)
        {
            var bigCraftableToSell = ItemList.GetRandomBigCraftablesToSell(shopRNG, numberToGet: 1).First();
            AddStock(menu, bigCraftableToSell, salePrice: GetSalePrice(bigCraftableToSell, multiplier: 2));
        }

        /// <summary>
        /// Adds 2-3 medium + misc items
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddMiscItems(ShopMenu menu, Random shopRNG)
        {
            var numberOfMiscItems = Range.GetRandomValue(2, 3, shopRNG);
            var poolOfMiscItemsToSell = ItemList.GetItemsAtDifficulty(ObtainingDifficulties.MediumTimeRequirements)
                .Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.LargeTimeRequirements))
                .Concat(ItemList.GetItemsAtDifficulty(ObtainingDifficulties.UncommonItem))
                .ToList();
            var miscItemsToSell = Globals.RNGGetRandomValuesFromList(poolOfMiscItemsToSell, numberOfMiscItems, shopRNG);
            miscItemsToSell.ForEach(item =>
                AddStock(menu, item, salePrice: GetSalePrice(item, minimumValue: 100)));
        }

        /// <summary>
        /// Adds a random totem type, alwas costing 500 Qi Coins
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="shopRNG"></param>
        private static void AddTotem(ShopMenu menu, Random shopRNG)
        {
            var totemList = new List<int>()
            {
                (int)ObjectIndexes.WarpTotemFarm,
                (int)ObjectIndexes.WarpTotemBeach,
                (int)ObjectIndexes.WarpTotemMountains,
                (int)ObjectIndexes.WarpTotemDesert,
                (int)ObjectIndexes.RainTotem
            };
            var totemToSell = new SVObject(Globals.RNGGetRandomValueFromList(totemList, shopRNG), 1);
            AddStock(menu, totemToSell, salePrice: 500);
        }

        /// <summary>
        /// Adjust the sale price of the item to be a factor of 10 since it doesn't cost coins;
        /// it costs the club currency!
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="minimumValue">The minimum value the item should be sold at</param>
        /// <param name="multiplier">The amount to multiply the sale price by</param>
        /// <returns>The computed sale price</returns>
        private static int GetSalePrice(Item item, int minimumValue = 1000, int multiplier = 1)
        {
            return GetSalePrice(item.GetSaliableObject(), minimumValue, multiplier);
        }

        /// <summary>
        /// Adjust the sale price of the item to be a factor of 10 since it doesn't cost coins;
        /// it costs the club currency!
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="minimumValue">The minimum value the item should be sold at</param>
        /// <param name="multiplier">The amount to multiply the sale price by</param>
        /// <returns>The computed sale price</returns>
        private static int GetSalePrice(ISalable item, int minimumValue = 1000, int multiplier = 1)
        {
            var baseSalePrice = Math.Max(minimumValue, item.salePrice() / 10);
            return GetAdjustedItemPrice(item, baseSalePrice, multiplier);
        }
    }
}
