﻿using StardewValley.GameData.Shops;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    public class RandomizedSaloonShop : RandomizedShop
    {
        public RandomizedSaloonShop() : base("Saloon") { }

        /// <summary>
        /// Modifies the shop stock - see AdjustStock for details
        /// </summary>
        /// <returns>The modified shop data</returns>
        public override ShopData ModifyShop()
        {
            AdjustStock();

            return CurrentShopData;
        }

        /// <summary>
        /// The saloon shop will be mostly random now - cycling every Monday
        /// - Beer and Coffee will still be available
        /// - 3-5 random cooked foods will be sold
        /// - 3-5 random recipes will be sold (not shown if the player has them)
        /// </summary>
        private void AdjustStock()
        {
            if (!Globals.Config.Shops.RandomizeSaloonShop)
            {
                return;
            }

            // Stock will change every Monday
            Random shopRNG = Globals.GetWeeklyRNG(nameof(RandomizedSaloonShop));
            CurrentShopData.Items.Clear();

            // Beer and coffee will always be available
            AddStock(ItemList.GetQualifiedId(ObjectIndexes.Beer), "BeerItem");
            AddStock(ItemList.GetQualifiedId(ObjectIndexes.Coffee), "CoffeeItem");

            // Random Cooked Items - pick 3-5 random dishes each week
            var numberOfCookedItems = Range.GetRandomValue(3, 5, shopRNG);
            List<string> allCookedItemIds = ItemList.GetCookedItems()
                .Select(item => item.QualifiedId)
                .ToList();
            List<string> gusFoodList = 
                Globals.RNGGetRandomValuesFromList(allCookedItemIds, numberOfCookedItems, shopRNG);
            gusFoodList.ForEach(itemId => AddStock(itemId, $"FoodItem-{itemId}"));

            // Random Cooking Recipes - pick 3-5 random recipes each week
            // Note that the game will not include these if they are already learned
            var numberOfRecipes = Range.GetRandomValue(3, 5, shopRNG);
            List<string> gusRecipeList = 
                Globals.RNGGetRandomValuesFromList(allCookedItemIds, numberOfRecipes, shopRNG);
            gusRecipeList.ForEach(itemId =>
                AddStock(itemId, $"RecipeItem-{itemId}", isRecipe: true));
        }
    }
}
