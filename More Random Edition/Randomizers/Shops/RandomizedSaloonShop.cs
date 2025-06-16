using StardewValley.GameData.Shops;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    public class RandomizedSaloonShop : RandomizedShop
    {
        public RandomizedSaloonShop() : base("Saloon") { }

        public override bool ShouldModifyShop()
            => Globals.Config.Shops.RandomizeSaloonShop;

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
            // Stock will change every Monday
            RNG shopRNG = RNG.GetWeeklyRNG(nameof(RandomizedSaloonShop));
            CurrentShopData.Items.Clear();

            // Beer and coffee will always be available
            AddStock(ItemList.GetQualifiedId(ObjectIndexes.Beer), "BeerItem");
            AddStock(ItemList.GetQualifiedId(ObjectIndexes.Coffee), "CoffeeItem");

            // Random Cooked Items - pick 3-5 random dishes each week
            var numberOfCookedItems = shopRNG.NextIntWithinRange(3, 5);
            List<CookedItem> allCookedItems = ItemList.GetCookedItems()
                .Cast<CookedItem>()
                .ToList();
            shopRNG.GetRandomValuesFromList(
                    allCookedItems.Select(item => item.QualifiedId).ToList(), 
                    numberOfCookedItems)
                .ForEach(itemId => AddStock(itemId, $"FoodItem-{itemId}"));

            /// Adds random cooking recipes
            /// - Pick 3-5 random recipes each week
            /// - Note that the game will not include these if they are already learned
            var numberOfRecipes = shopRNG.NextIntWithinRange(3, 5);
            shopRNG.GetRandomValuesFromList(allCookedItems, numberOfRecipes)
                .ForEach(cookedItem =>
                    AddStock(
                        cookedItem.QualifiedId, 
                        $"RecipeItem-{cookedItem.QualifiedId}", 
                        isRecipe: true,
                        recipeName: cookedItem.RecipeName)
                    );
        }
    }
}
