using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;

namespace Randomizer
{
	class MenuAdjustments
	{
		public static void TryAdjustMenu(object sender, MenuChangedEventArgs e)
		{
			// Bundle menu - fix ring deposit
			if (Globals.Config.Bundles.Randomize && (e.NewMenu is JunimoNoteMenu))
			{
				BundleMenuAdjustments.FixRingSelection(sender, e);
			}

			// Shop menu - inject stock
			else if ((Globals.Config.Shops.RandomizeMainShops || Globals.Config.Shops.RandomizeMiscShops) && (e.NewMenu is ShopMenu menu))
			{

				/* Only set up to work with shops in which the seller has a name (e.g. NOT the traveling cart lady
				 * It is not ever necessary to repoint a shopStock method. Stock can be carefully removed from shops (see EmptyStock() method), and added or modified at will.
				 */

				if (menu.portraitPerson != null)
				{
					switch (menu.portraitPerson.Name)
					{

						/* Balance sapling prices and add a random early-game-obtainable Item of the Day */
						case "Pierre":
							ShopMenuAdjustments.AdjustSeedShopStock(menu);
							break;

						/* Balance weapon and armor prices
						 * TODO: Add small chance of random higher-tier weapon appearing at high price
						 */
						case "Marlon":
							ShopMenuAdjustments.AdjustAdventureShopStock(menu);
							break;

						/* Add clay to stock in case player is having trouble getting enough*/
						case "Robin":
							ShopMenuAdjustments.AdjustCarpenterShopStock(menu);
							break;

						/* Sells a random assort of mid-game-obtainable items */
						case "Krobus":
							ShopMenuAdjustments.AdjustSewerShopStock(menu);
							break;

						/* Sells a random assortment of late-game-obtainable items */
						case "Sandy":
							ShopMenuAdjustments.AdjustOasisShopStock(menu);
							break;

						/* Gus sells random foods each day */
						case "Gus":
							ShopMenuAdjustments.AdjustSaloonShopStock(menu);
							break;

						// add more shops

						default:
							break;
					}
				}
			}
		}


		/// <summary>
		/// Fixes the item name that you get at the start of the game
		/// </summary>
		public static void FixParsnipSeedBox()
		{
			GameLocation farmHouse = Game1.locations.Where(x => x.Name == "FarmHouse").First();

			List<StardewValley.Objects.Chest> chestsInRoom =
				farmHouse.Objects.Values.Where(x =>
					x.DisplayName == "Chest")
					.Cast<StardewValley.Objects.Chest>()
					.Where(x => x.giftbox.Value)
				.ToList();

			if (chestsInRoom.Count > 0)
			{
				string parsnipSeedsName = ItemList.GetItemName((int)ObjectIndexes.ParsnipSeeds);
				StardewValley.Item itemInChest = chestsInRoom[0].items[0];
				if (itemInChest.Name == "Parsnip Seeds")
				{
					itemInChest.Name = parsnipSeedsName;
					itemInChest.DisplayName = parsnipSeedsName;
				}
			}
		}

	}
}
