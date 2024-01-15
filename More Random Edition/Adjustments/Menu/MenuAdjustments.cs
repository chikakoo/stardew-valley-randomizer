﻿using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace Randomizer
{
    /// <summary>
    /// Makes menu adjustments to shops, etc
    /// </summary>
    public class MenuAdjustments
    {
        private static SeedShopMenuAdjustments SeedShop { get; } = new();
        private static AdventureShopMenuAdjustments AdventureShop { get; } = new();
        private static CarpenterShopMenuAdjustments CarpenterShop { get; } = new();
        private static SaloonShopMenuAdjustments SaloonShop { get; } = new();
        private static OasisShopMenuAdjustments OasisShop { get; } = new();
        private static SewerShopMenuAdjustments SewerShop { get; } = new();
        private static HatShopMenuAdjustments HatShop { get; } = new();
        private static ClubShopMenuAdjustments ClubShop { get; } = new();
        private static JojaMartMenuAdjustments JojaMart { get; } = new();
        private static FishingShopMenuAdjustments FishingShop { get; } = new();
        private static BlacksmithShopMenuAdjustments BlacksmithShop { get; } = new();

        /// <summary>
        /// Reset all the shop states
        /// Intended to be called at the end of every day so shops can reset
        /// </summary>
        public static void ResetShopStates()
        {
            SeedShop.ResetShopState();
            // Adventure shop is skipped as there's nothing to restore
            CarpenterShop.ResetShopState();
            SaloonShop.ResetShopState();
            OasisShop.ResetShopState();
            SewerShop.ResetShopState();
            // Hat shop is skipped as there's nothing to restore
            // Club shop is skipped as there's nothing to restore
            JojaMart.ResetShopState();
            FishingShop.ResetShopState();
            BlacksmithShop.ResetShopState();
        }

        /// <summary>
        /// Makes the actual menu adjustments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Contains the menu info - NewMenu means a menu was opened, OldMenu means one was closed</param>
        public static void AdjustMenus(object sender, MenuChangedEventArgs e)
        {
            // Bundle menu - fix ring deposit
            if (e.NewMenu is JunimoNoteMenu openedBundleMenu)
            {
                BundleMenuAdjustments.FixRingSelection(openedBundleMenu);
            }

            // Shops - adjust on open
            else if (e.NewMenu is ShopMenu openedShopMenu)
            {
                AdjustShopMenus(openedShopMenu, wasShopOpened: true);
            }

            // Shops - adjust on close
            else if (e.OldMenu is ShopMenu closedShopMenu)
            {
                AdjustShopMenus(closedShopMenu, wasShopOpened: false);
            }

            // Museum - TODO: can check if there's a better way to verify this
            //else if (e.NewMenu is ItemGrabMenu itemGrabMenu && Game1.currentLocation is LibraryMuseum)
            //{
            //    MuseumRewardMenuAdjustments.AdjustMenu(itemGrabMenu);
            //}
        }



        /// <summary>
        /// Adjust shops on menu open
        /// Modifies the stock if it was the first time they were open, or restores it from the state
        /// it was at when it was last closed
        /// </summary>
        /// <param name="shopMenu"></param>
        /// <param name="wasShopOpened">True if the shop was just opened, false if it was closed</param>
        private static void AdjustShopMenus(ShopMenu shopMenu, bool wasShopOpened)
        {
            switch (shopMenu.storeContext)
            {
                // Seed shop and Joja Mart - adds item of the week
                case "SeedShop":
                    SeedShop.OnChange(shopMenu, wasShopOpened);
                    break;
                case "JojaMart":
                    JojaMart.OnChange(shopMenu, wasShopOpened);
                    break;
                // Blacksmith shop - chance of mining-related random items/discounts
                case "Blacksmith":
                    BlacksmithShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Adventure shop - fix weapon prices so infinite money can't be made
                case "AdventureGuild":
                    AdventureShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Carpenter shop - add clay to prevent long grinds
                case "ScienceHouse":
                    CarpenterShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Saloon shop - will sell random foods/recipes each day
                case "Saloon":
                    SaloonShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Oasis shop - randomizes its foragable/crop/furniture stock each week
                case "SandyHouse":
                    OasisShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Sewer shop - randomizes the furniture and big craftable items daily
                case "Sewer":
                    SewerShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Fishing shop - adds a catch of the day
                case "FishShop":
                    FishingShop.OnChange(shopMenu, wasShopOpened);
                    break;
                // Hat shop - will sell a random hat each week in addition to what you've already unlocked
                case "Forest":
                    // The hat shop is located further down than the Traveling Merchant
                    if (Game1.player.getTileLocation().Y > 90)
                    {
                        HatShop.OnChange(shopMenu, wasShopOpened);
                    }
                    break;
                // Club shop sells random furniture/clothing items weekly
                case "Club":
                    ClubShop.OnChange(shopMenu, wasShopOpened);
                    break;
            }
        }
    }
}
