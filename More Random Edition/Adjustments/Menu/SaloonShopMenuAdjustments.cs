﻿using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    internal class SaloonShopMenuAdjustments : ShopMenuAdjustments
    {
        /// <summary>
        /// The saloon shop will be mostly random now - cycling every Monday
        /// - Beer and Coffee will still be available
        /// - 3-5 random cooked foods will be sold
        /// - 3-5 random recipes will be sold (not shown if the player has them)
        /// </summary>
        /// <param name="menu">The shop menu</param>
        public static void AdjustStock(ShopMenu menu)
        {
            if (!Globals.Config.Shops.RandomizeSaloonShop)
            {
                return;
            }

            // Stock will change every Monday
            Random shopRNG = Globals.GetWeeklyRNG();
            EmptyStock(menu);

            // Beer and coffee will always be available
            SVObject beer = new((int)ObjectIndexes.Beer, 1);
            SVObject coffee = new((int)ObjectIndexes.Coffee, 1);

            AddStock(menu, beer);
            AddStock(menu, coffee);

            // Random Cooked Items - pick 3-5 random dishes each week
            var numberOfCookedItems = Range.GetRandomValue(3, 5, shopRNG);
            List<Item> gusFoodList = Globals.RNGGetRandomValuesFromList(ItemList.GetCookedItems(), numberOfCookedItems, shopRNG);
            AddStock(menu, gusFoodList);

            // Random Cooking Recipes - pick 3-5 random recipes each week
            var numberOfRecipes = Range.GetRandomValue(3, 5, shopRNG);
            List<Item> gusRecipeList = Globals.RNGGetRandomValuesFromList(ItemList.GetCookedItems(), numberOfRecipes, shopRNG);
            gusRecipeList.ForEach(recipeItem =>
            {
                ISalable recipe = recipeItem.GetSaliableObject(isRecipe: true);

                // Don't add if player already knows recipe
                string recipeName = recipe.Name[..(recipe.Name.IndexOf("Recipe") - 1)];
                if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
                {
                    AddStock(menu, recipe, stock: 1);
                }
            });
        }
    }
}
