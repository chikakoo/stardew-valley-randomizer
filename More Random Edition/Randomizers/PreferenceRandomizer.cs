using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the preferences of all NPCs
	/// </summary>
	public class PreferenceRandomizer
	{
		/// <summary>
		/// Default data for universal preferences - these can be overridden by an NPC's individual preference
		/// </summary>
		private readonly static Dictionary<string, string> DefaultUniversalPreferenceData = new Dictionary<string, string>()
		{
			["Universal_Love"] = "74 446 797 373 279",
			["Universal_Like"] = "-2 -7 -26 -75 -80 72 395 613 634 635 636 637 638 724 459 873",
			["Universal_Neutral"] = "194 216 262 304 815",
			["Universal_Dislike"] = "-4 -8 -12 -15 -16 -19 -22 -24 -25 -28 -74 78 169 246 247 305 309 310 311 403 419 423 535 536 537 725 726 749 271",
			["Universal_Hate"] = "0 -20 -21 92 110 111 112 142 152 153 157 178 105 168 170 171 172 374 376 378 380 397 420 684 721 766 767 772 203 308 265 909 910",
		};

		/// <summary>
		/// Item Category indexes - only includes categories which are giftable - full list available here: https://stardewcommunitywiki.com/Modding:Object_data#Categories
		/// </summary>
		private readonly static Dictionary<int, string> GiftableItemCategories = new Dictionary<int, string>
		{
			[-2] = "Gems",
			[-4] = "Fish",
			[-5] = "Eggs",
			[-6] = "Milk",
			[-7] = "Cooking",
			[-8] = "Crafting",
			[-12] = "Minerals",
			[-15] = "Metals",
			[-16] = "Resources",
			[-20] = "Trash",
			[-21] = "Bait",
			[-22] = "Tackles",
			[-24] = "Decor",
			[-26] = "Artisan Goods",
			[-27] = "Tree Products",
			[-28] = "Monster Loot",
			[-74] = "Seeds",
			[-75] = "Vegetables",
			[-79] = "Fruit",
			[-80] = "Flowers",
			[-81] = "Foragables"
		};

		// Set of indices to use when parsing npc prefstrings.
		private const int LovesIndex = 1;
		private const int LikesIndex = 3;
		private const int DislikesIndex = 5;
		private const int HatesIndex = 7;
		private const int NeutralIndex = 9;

		private const int IndexOffset = 2;

		/// <summary>
		/// Randomize NPC Preferences information.
		/// </summary>
		/// <returns>Dictionary&lt;string, string&gt; which holds the replacement prefstrings for the enabled preferences (NPC/Universal).</returns>

		public static Dictionary<string, string> Randomize()
		{
			Dictionary<string, string> replacements = new Dictionary<string, string>();

			List<int> universalUnusedCategories = new List<int>(GiftableItemCategories.Keys);
			List<Item> universalUnusedItems = ItemList.GetGiftables();

			// Generate randomized Universal Preferences strings
			// Only add if corresponding config option enabled - keeps RNG stable
			// Also allows for configuring Universal/NPC Prefs independently
			bool UniversalPrefsEnabled = Globals.Config.NPCs.RandomizeUniversalPreferences;
			foreach (KeyValuePair<string, string> universalPrefs in DefaultUniversalPreferenceData)
			{
				if (UniversalPrefsEnabled)
				{
					replacements.Add(universalPrefs.Key, GetUniversalPreferenceString(universalUnusedCategories, universalUnusedItems));
				}
			}

			// Generate randomized NPC Preferences strings
			// Same as above - only add if config option is enabled
			bool NPCPrefsEnabled = Globals.Config.NPCs.RandomizeIndividualPreferences;

			foreach (string NPC in NPC.GiftableNPCs)
			{
				List<int> unusedCategories = new List<int>(GiftableItemCategories.Keys);
				List<Item> unusedItems = ItemList.GetGiftables();

				string[] tokens = Globals.GetTranslation($"{NPC}-prefs").Split('/');
				string name = NPC;

				for (int index = LovesIndex; index <= NeutralIndex; index += IndexOffset)
				{
					tokens[index] = GetPreferenceString(index, unusedCategories, unusedItems);
				}

				if (NPCPrefsEnabled)
				{
					replacements.Add(name, string.Join("/", tokens));
				}

			}

			// Update Loves/Hates for Bundle reqs
			UpdateBundlePrefs(replacements);
			WriteToSpoilerLog(replacements);
			return replacements;
		}

		/// <summary>
		/// Builds universal preference string
		/// </summary>
		/// <param name="unusedCategories">Keeps track of which categories have not yet been assigned</param>
		/// <param name="unusedItems">Keeps track of which items have not yet been assigned.</param>
		/// <returns><c>string</c> containing IDs of categories and items</returns>
		private static string GetUniversalPreferenceString(List<int> unusedCategories, List<Item> unusedItems)
		{
			// No need to vary quantities per index.May end up with lots of loved items, lots of hated items, both, neither, etc.
			int catNum = Range.GetRandomValue(0, 10);
			int itemNum = Range.GetRandomValue(5, 30);

			string catString = "";
			string itemString = "";

			while (unusedCategories.Any() && catNum > 0)
			{
				catString += Globals.RNGGetAndRemoveRandomValueFromList(unusedCategories) + " ";
				catNum--;
			}

			while (unusedItems.Any() && itemNum > 0)
			{
				itemString += Globals.RNGGetAndRemoveRandomValueFromList(unusedItems).Id + " ";
				itemNum--;
			}

			return (catString + itemString).Trim();
		}

		/// <summary>
		/// Builds an NPC's preference string for a given index (loves, hates, etc.).
		/// </summary>
		/// <param name="index">the index of the NPC's prefstring.</param>
		/// <param name="unusedCategories">Holds list of categories which have not yet been assigned - prevents double-assignment.</param>
		/// <param name="unusedItems">Holds list of Items which have not yet been assigned - prevents double-assignment.</param>
		/// <returns>NPC's preference string for a given index.</returns>
		private static string GetPreferenceString(int index, List<int> unusedCategories, List<Item> unusedItems)
		{
			int minItems;
			int maxItems;

			// Should probably be moved into its own function
			// Determine how many items to add per category - data available here: https://pastebin.com/gFEduBVd
			// Basically, add more loved/liked items than hated/disliked, and few neutrals
			switch (index)
			{
				// Loved Items - higher minimum, so generated bundles have more items to draw from
				case LovesIndex:
					minItems = 6;
					maxItems = 11;
					break;

				case LikesIndex:
				case DislikesIndex:
					minItems = 1;
					maxItems = 18;
					break;

				case HatesIndex:
					minItems = 1;
					maxItems = 11;
					break;

				case NeutralIndex:
					minItems = 1;
					maxItems = 3;
					break;

				default:
					minItems = 0;
					maxItems = 0;
					break;
			}

			int itemNum = Range.GetRandomValue(minItems, maxItems);
			int catNum = Range.GetRandomValue(1, 4);

			string itemString = GetRandomItemString(unusedItems, itemNum);
			string catString = GetRandomCategoryString(unusedCategories, catNum);

			string tokenString = catString + " " + itemString;
			return tokenString;
		}

		/// <summary>Builds a string consisting of <paramref name="quantity"/> randomly selected IDs from <paramref name="unusedItems"/>.</summary>
		/// <param name="quantity">the number of IDs to add.</param>
		/// <param name="unusedItems"> the list of IDs to pull from.</param>
		/// <returns>A string of Item IDs with no leading/trailing whitespace.</returns>
		private static string GetRandomItemString(List<Item> unusedItems, int quantity)
		{
			List<Item> giftableItems = new List<Item>(unusedItems);
			string itemString = "";

			for (int itemQuantity = quantity; itemQuantity > 0; itemQuantity--)
			{
				itemString += Globals.RNGGetAndRemoveRandomValueFromList(giftableItems).Id + " ";
			}

			return itemString.Trim();
		}

		/// <summary>Builds a string consisting of <paramref name="quantity"/> randomly selected IDs from <paramref name="unusedCategoryIDs"/>.</summary>
		/// <param name="quantity">the number of IDs to add.</param>
		/// <param name="unusedCategoryIDs"> the list of IDs to pull from.</param>
		/// <returns>A string of Category IDs with no leading/trailing whitespace.</returns>
		private static string GetRandomCategoryString(List<int> unusedCategoryIDs, int quantity)
		{
			string catString = "";

			for (int catQuantity = quantity; catQuantity > 0; catQuantity--)
			{
				catString += Globals.RNGGetAndRemoveRandomValueFromList(unusedCategoryIDs) + " ";
			}

			return catString.Trim();
		}

		/// <summary>
		/// Updates Universal Loves, Universal Hates, and all NPC Loves for Bundles.
		/// </summary>
		private static void UpdateBundlePrefs(Dictionary<string, string> replacements)
		{
			List<Item> newLovesList = ItemList.GetItemListFromString(replacements["Universal_Love"], ' ');
			List<Item> newHatesList = ItemList.GetItemListFromString(replacements["Universal_Hate"], ' ');

			NPC.UpdateUniversalLoves(newLovesList);
			NPC.UpdateUniversalHates(newHatesList);

			foreach (KeyValuePair<string, string> NPCPreferences in replacements)
			{
				// If Universal Preference, skip
				if (DefaultUniversalPreferenceData.ContainsKey(NPCPreferences.Key)) { continue; }

				string NPCLoves = NPCPreferences.Value.Split('/')[LovesIndex];
				NPC.UpdateNPCLoves(NPCPreferences.Key, ItemList.GetItemListFromString(NPCLoves, ' '));
			}
		}

		/// <summary>
		/// Write to the spoiler log.
		/// </summary>
		private static void WriteToSpoilerLog(Dictionary<string, string> replacements)
		{
			if (!Globals.Config.NPCs.RandomizeIndividualPreferences && !Globals.Config.NPCs.RandomizeUniversalPreferences) { return; }

			Globals.SpoilerWrite("===== NPC GIFT TASTES =====");
			foreach (KeyValuePair<string, string> NPCPreferences in replacements)
			{
				if (DefaultUniversalPreferenceData.ContainsKey(NPCPreferences.Key))
				{
					Globals.SpoilerWrite($"{NPCPreferences.Key.Replace('_', ' ')}: {TranslateIDs(NPCPreferences.Value)}");
					Globals.SpoilerWrite("");
				}
				else
				{
					string npcName = NPCPreferences.Key;
					string[] tokens = NPCPreferences.Value.Split('/');

					Globals.SpoilerWrite(npcName);

					Globals.SpoilerWrite($"    Loves: {TranslateIDs(tokens[LovesIndex])}");
					Globals.SpoilerWrite($"    Likes: {TranslateIDs(tokens[LikesIndex])}");
					Globals.SpoilerWrite($"    Dislikes: {TranslateIDs(tokens[DislikesIndex])}");
					Globals.SpoilerWrite($"    Hates: {TranslateIDs(tokens[HatesIndex])}");
					Globals.SpoilerWrite($"    Neutral: {TranslateIDs(tokens[NeutralIndex])}");
					Globals.SpoilerWrite("");
				}
			}
			Globals.SpoilerWrite("");
		}

		/// <summary>
		/// Returns string with names of items in a comma-separated list.
		/// </summary>
		/// <param name="ItemIDString">the list of item IDs to parse. Expected format: ID numbers separated by spaces.</param>
		/// <returns>String of item names in a comma-separated list.</returns>
		private static string TranslateIDs(string ItemIDString)
		{
			string[] IDStringArray = ItemIDString.Split(' ');
			string outputString = "";

			for (int arrayPos = 0; arrayPos < IDStringArray.Length; arrayPos++)
			{
				bool IDParsed = int.TryParse(IDStringArray[arrayPos], out int ID);

				if (!IDParsed)
				{
					Globals.ConsoleWarn($"Input string was not in a correct format: '{IDStringArray[arrayPos]}'");
					continue;
				}

				// Positive numbers only - negative numbers represent categories
				if (ID > 0)
				{
					outputString += ItemList.GetItemName(ID);
				}
				else
				{
					outputString += "[" + GiftableItemCategories[ID] + "]";
				}

				// Not last item - put comma after
				if (arrayPos != IDStringArray.Length - 1)
				{
					outputString += ", ";
				}
			}

			return outputString;
		}
	}
}
