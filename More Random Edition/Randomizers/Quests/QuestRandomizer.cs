﻿using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class QuestRandomizer
	{
		private static RNG Rng { get; set; }

		private static List<string> People { get; set; }
		private static List<Item> Crops { get; set; }
		private static List<Item> Dishes { get; set; }
		private static List<Item> FishList { get; set; }
		private static List<Item> Items { get; set; }

		private static CropItem ParsnipCrop { get; set; }
		private static Dictionary<string, string> DefaultQuestData { get; set; }

		private readonly static List<string> QuestableNPCsList = new()
		{ // Kent is not included because of him not appearing for awhile
        	"Alex",
			"Elliot",
			"Harvey",
			"Sam",
			"Sebastian",
			"Shane",
			"Abigail",
			"Emily",
			"Haley",
			"Leah",
			"Maru",
			"Penny",
			"Caroline",
			"Clint",
			"Demetrius",
			"Evelyn",
			"George",
			"Gus",
			"Jas",
			"Jodi",
			"Lewis",
			"Linus",
			"Marnie",
			"Pam",
			"Pierre",
			"Robin",
			"Vincent",
			"Willy",
			"Wizard"
		};

		/// <summary>
		/// Maps the quest to what type of item it gives
		/// </summary>
		private static readonly Dictionary<string, QuestItemTypes> QuestIdToQuestTypeMap = new()
		{
			{ "3", QuestItemTypes.Static },
			{ "6", QuestItemTypes.Static },
			{ "22", QuestItemTypes.Static },
			{ "101", QuestItemTypes.Crop },
			{ "103", QuestItemTypes.Dish },
			{ "104", QuestItemTypes.Crop },
			{ "105", QuestItemTypes.Crop },
			{ "106", QuestItemTypes.Crop },
			{ "108", QuestItemTypes.Crop },
			{ "109", QuestItemTypes.Fish },
			{ "110", QuestItemTypes.Item },
			{ "111", QuestItemTypes.Item },
			{ "112", QuestItemTypes.Item },
			{ "113", QuestItemTypes.Item },
			{ "114", QuestItemTypes.Fish },
			{ "115", QuestItemTypes.Crop },
			{ "116", QuestItemTypes.Crop },
			{ "117", QuestItemTypes.Dish },
			{ "118", QuestItemTypes.Fish },
			{ "119", QuestItemTypes.Crop },
			{ "120", QuestItemTypes.Item },
			{ "121", QuestItemTypes.Fish },
			{ "122", QuestItemTypes.Item },
			{ "123", QuestItemTypes.Item },
			{ "124", QuestItemTypes.Fish },
			{ "125", QuestItemTypes.Crop }
        };

		/// <summary>
		/// A mapping of quest IDs to what mail key it belongs to
		/// </summary>
		private static readonly Dictionary<string, string> QuestToMailMap = new()
		{
			{ "101", "spring_19_1" },
			{ "103", "summer_14_1" },
			{ "104", "summer_20_1" },
			{ "105", "summer_25_1" },
			{ "106", "fall_3_1" },
			{ "108", "fall_19_1" },
			{ "109", "winter_2_1" },
			{ "110", "winter_6_1" },
			{ "111", "winter_12_1" },
			{ "112", "winter_17_1" },
			{ "113", "winter_21_1" },
			{ "114", "winter_26_1" },
			{ "115", "spring_6_2" },
			{ "116",  "spring_15_2" },
			{ "117", "spring_21_2" },
			{ "118", "summer_6_2" },
			{ "119", "summer_15_2"},
			{ "120", "summer_21_2" },
			{ "121", "fall_6_2"},
			{ "122", "fall_19_2" },
			{ "123", "winter_5_2"},
			{ "124", "winter_13_2" },
			{ "125", "winter_19_2" }
		};

		/// <summary>
		/// The default mail data that could potentially be replaced
		/// </summary>
		private static Dictionary<string, string> DefaultMailData { get; set; }

		/// <summary>
		/// The quest replacement information
		/// This will be used by the QuestLogAdjustments to fix quest names in the quest log
		/// </summary>
		public static Dictionary<string, string> QuestReplacements { get; set; } = new();

		/// <summary>
		/// Randomizes quest items to get, people, rewards, etc.
		/// </summary>
		/// <returns>The quest information to modify</returns>
		public static QuestInformation Randomize()
		{
            QuestReplacements.Clear();
            Dictionary<string, string> mailReplacements = new();
            if (!Globals.Config.RandomizeQuests) 
			{
				return new QuestInformation(QuestReplacements, mailReplacements); 
			}

            Rng = RNG.GetFarmRNG(nameof(QuestRandomizer));
            People = QuestableNPCsList;
			Crops = ItemList.GetCrops(true).ToList();
			Dishes = ItemList.GetCookedItems().ToList();
			FishList = FishItem.Get().ToList();
			Items = ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.Impossible).ToList();

			PopulateQuestDictionary();
			PopulateMailDictionary();
			RandomizeQuestsAndMailStrings(QuestReplacements, mailReplacements);

			WriteToSpoilerLog(QuestReplacements);

			return new QuestInformation(QuestReplacements, mailReplacements);
		}

		/// <summary>
		/// Fills the entries of the quest dictionary with the internationalized strings
		/// Sets up the parsnip crop for later use
		/// </summary>
		private static void PopulateQuestDictionary()
		{
			var parsnipCropId = (ObjectIndexes.ParsnipSeeds.GetItem() as SeedItem).CropId;
			ParsnipCrop = ItemList.Items[parsnipCropId] as CropItem;

            DefaultQuestData = new();
            foreach (string questId in QuestIdToQuestTypeMap.Keys)
			{
				DefaultQuestData.Add(questId, Globals.GetTranslation($"quest-{questId}"));
			}
		}

		/// <summary>
		/// Fills the entries of the mail dictionary with the internationalized strings
		/// </summary>
		private static void PopulateMailDictionary()
		{
			List<string> mailKeys = new()
			{
				"spring_19_1",
				"summer_14_1",
				"summer_20_1",
				"summer_25_1",
				"fall_3_1",
				"fall_19_1",
				"winter_2_1",
				"winter_6_1",
				"winter_12_1",
				"winter_17_1",
				"winter_21_1",
				"winter_26_1",
				"spring_6_2",
				"spring_15_2",
				"spring_21_2",
				"summer_6_2",
				"summer_15_2",
				"summer_21_2",
				"fall_6_2",
				"fall_19_2",
				"winter_5_2",
				"winter_13_2",
				"winter_19_2"
			};

			DefaultMailData = new Dictionary<string, string>();
			foreach (string mailKey in mailKeys)
			{
				DefaultMailData.Add(mailKey, Globals.GetTranslation($"mail-{mailKeys}"));
			}
		}

		/// <summary>
		/// Populates the given quest and mail replacement dictionaries
		/// </summary>
		/// <param name="questReplacements">The dictionary of quest replacements to fill</param>
		/// <param name="mailReplacements">The dictionary of mail replacements to fill</param>
		private static void RandomizeQuestsAndMailStrings(
			Dictionary<string, string> questReplacements,
			Dictionary<string, string> mailReplacements)
		{
			foreach (string questId in DefaultQuestData.Keys)
			{
				object tokenObject = GetTokenObject(questId);

				string questString = Globals.GetTranslation($"quest-{questId}", tokenObject);
				questReplacements.Add(questId, questString);

				if (QuestToMailMap.ContainsKey(questId) && DefaultMailData.ContainsKey(QuestToMailMap[questId]))
				{
					string mailKey = QuestToMailMap[questId];
					string mailString = Globals.GetTranslation($"mail-{mailKey}", tokenObject);
					mailReplacements.Add(mailKey, mailString);
				}
			}
		}

		/// <summary>
		/// Gets the object used for i18n token replacements
		/// </summary>
		/// <param name="questId">The quest ID - used to get the quest type</param>
		/// <returns>
		/// A generic object in the following format:
		/// - person - the npc name (translated)
		/// - otherPerson - a second npc name (translated)
		/// - englishPerson - untranslated npc name (used for quest keys)
		/// - itemName - the translated item name you will need to get for the quest - empty string if nothing
		/// - cropStart - the first 4 characters of the crop name - empty string if not a crop
		/// - id - the id of the item you need to get - 0 if nothing
		/// - article - "a" or "an" - currently only used for English replacements
		/// - number - random number between 2 and 10; used to determine how many of an item you need to get
		/// - reward - the money reward for a quest - between 300 and 3000
		/// </returns>
		private static object GetTokenObject(string questId)
		{
			ReplacementObject replacements = new(Rng);
			string itemName = "";
			string cropStart = "";
			string qualifiedId = "";

			QuestItemTypes questType = QuestIdToQuestTypeMap[questId];
			switch (questType)
			{
				case QuestItemTypes.Static:
					switch (questId)
					{
						case "3":
							itemName = ItemList.GetItemName(ObjectIndexes.Beet);
							break;
						case "6":
							itemName = ParsnipCrop.DisplayName;
							qualifiedId = ParsnipCrop.QualifiedId;
							break;
						case "22":
                            itemName = ItemList.GetItemName(ObjectIndexes.LargemouthBass);
							break;
						default:
							Globals.ConsoleError($"In the static quest type for unexpected quest: {questId}");
							break;
					}
					break;
				case QuestItemTypes.Crop:
					itemName = replacements.Crop.DisplayName;
					cropStart = Globals.GetStringStart(itemName, 4);
					qualifiedId = replacements.Crop.QualifiedId;
					break;
				case QuestItemTypes.Dish:
					itemName = replacements.Dish.DisplayName;
					qualifiedId = replacements.Dish.QualifiedId;
					break;
				case QuestItemTypes.Fish:
					itemName = replacements.Fish.DisplayName;
					qualifiedId = replacements.Fish.QualifiedId;
					break;
				case QuestItemTypes.Item:
					itemName = replacements.Item.DisplayName;
					qualifiedId = replacements.Item.QualifiedId;
					break;
				default:
					break;
			}

			string article = Globals.GetArticle(itemName);

			return new
			{
				person = Globals.GetTranslation($"{replacements.Person}-name"),
				otherPerson = Globals.GetTranslation($"{replacements.OtherPerson}-name"),
				englishPerson = replacements.Person,

				item = itemName,
				cropStart,
				id = qualifiedId,
				a = article,

				number = replacements.Number,
				reward = replacements.Reward
			};
		}

		/// <summary>
		/// Used as a container of all of the replacements to be potentially made for a given quest
		/// </summary>
		private class ReplacementObject
		{
			public string Person { get; }
			public string OtherPerson { get; }
			public Item Crop { get; }
			public Item Dish { get; }
			public Item Fish { get; }
			public Item Item { get; }
			public int Number { get; }
			public int Reward { get; }

			public ReplacementObject(RNG rng)
			{
				Person = rng.GetRandomValueFromList(People);
				OtherPerson = rng.GetRandomValueFromList(People.Where(x => x != Person).ToList());
				Crop = rng.GetRandomValueFromList(Crops);
				Dish = rng.GetRandomValueFromList(Dishes);
				Fish = rng.GetRandomValueFromList(FishList);
				Item = rng.GetRandomValueFromList(Items);
				Number = rng.NextIntWithinRange(2, 10);
				Reward = rng.NextIntWithinRange(300, 3000);
			}
		}

		/// <summary>
		/// Writes the dictionary info to the spoiler log
		/// </summary>
		/// <param name="questList">The info to write out</param>
		private static void WriteToSpoilerLog(Dictionary<string, string> questList)
		{
			Globals.SpoilerWrite("==== QUESTS ====");
			foreach (KeyValuePair<string, string> pair in questList)
			{
				var questId = pair.Key;
				var questStrings = pair.Value.Split("/");

				var title = questStrings[(int)QuestIndexes.Title];
				var objective = questStrings[(int)QuestIndexes.Objective];
				var npcAndItem = questStrings[(int)QuestIndexes.NPCAndItem];
				var reward = questStrings[(int)QuestIndexes.Reward];

				var npcAndRewardString = $" - {npcAndItem} - {reward}G";
				if (npcAndItem == "null" || string.IsNullOrEmpty(npcAndItem))
				{
					npcAndRewardString = string.Empty;
				}

				Globals.SpoilerWrite($"{questId} ({title}): {objective}{npcAndRewardString}");
			}
			Globals.SpoilerWrite("");
		}
	}
}
