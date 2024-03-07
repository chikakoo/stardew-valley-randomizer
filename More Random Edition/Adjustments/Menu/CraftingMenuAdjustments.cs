﻿using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    public class CraftingMenuAdjustments
	{
		/// <summary>
		/// Reduces the cost of the crab pot
		/// Intended to be used if the player has the Trapper profession
		/// </summary>
		/// <param name="gameMenu">The game menu that needs its cost adjusted</param>
		public static void ReduceCrabPotCost(GameMenu gameMenu)
		{
            const int TrapperProfession = 7;
            if (!Globals.Config.CraftingRecipes.Randomize || 
				!Game1.player.professions.Contains(TrapperProfession)) 
			{ 
				return; 
			}

			CraftingPage craftingPage = (CraftingPage)gameMenu.pages[GameMenu.craftingTab];
			foreach (var page in craftingPage.pagesOfCraftingRecipes)
			{
				foreach (ClickableTextureComponent key in page.Keys)
				{
					CraftingRecipe recipe = page[key];
					if (recipe.name == "Crab Pot")
					{
						CraftableItem crabPot = (CraftableItem)ItemList.Items[ObjectIndexes.CrabPot];
						Dictionary<ObjectIndexes, int> randomizedRecipe = crabPot.LastRecipeGenerated;
						ReduceRecipeCost(page[key], randomizedRecipe);
					}
				}
			}
		}

		/// <summary>
		/// Reduces a recipe's cost
		/// - if everything only needs one of each item, remove the cheapest item
		/// - otherwise - halve the amounts of all items, rounding down (with a min of 1 required)
		/// </summary>
		/// <param name="inGameRecipe">The recipe as stored by Stardew Valley</param>
		/// <param name="randomizedRecipe">The recipe as stored by this mod</param>
		private static void ReduceRecipeCost(
			CraftingRecipe inGameRecipe, 
			Dictionary<ObjectIndexes, int> randomizedRecipe)
		{
			Dictionary<int, int> modifiedRecipe = 
				CraftableItem.ConvertRecipeToUseCategories(randomizedRecipe);
            Dictionary<string, int> recipeList = inGameRecipe.recipeList;
			recipeList.Clear();
			if (modifiedRecipe.Values.All(x => x < 2))
			{
				int firstKeyOfEasiestItem = modifiedRecipe.Keys
					.Where(id => id > 0) // Removes categories - these will never be the cheapest item in this case
					.Select(id => ItemList.Items[(ObjectIndexes)id])
					.OrderBy(item => item.DifficultyToObtain)
					.Select(item => item.Id)
					.First();

				foreach (int id in modifiedRecipe.Keys.Where(x => x != firstKeyOfEasiestItem))
				{
					recipeList.Add(id.ToString(), 1);
				}
			}
			else
			{
				foreach (int id in modifiedRecipe.Keys)
				{
                    int numberRequired = modifiedRecipe[id];
					recipeList.Add(id.ToString(), Math.Max(numberRequired / 2, 1));
				}
			}
		}
	}
}