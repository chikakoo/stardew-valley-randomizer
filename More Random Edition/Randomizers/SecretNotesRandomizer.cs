using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	class SecretNotesRandomizer
	{

		private static Dictionary<string, string> prefs;

		/// <summary>
		/// For each secret note, generate a random number of NPCs for whom to reveal loved items.
		/// </summary>
		/// <returns><c>Dictionary&lt;int, string&gt;</c> containing the secret note IDs and strings to replace.</returns>
		public static Dictionary<int, string> FixSecretNotes(Dictionary<string, string> preferenceReplacements)
		{
			// Only use generated notes if config option is enabled
			if (!Globals.Config.NPCs.RandomizeIndividualPreferences)
			{
				return new Dictionary<int, string>();
			}

			prefs = preferenceReplacements;
			Dictionary<int, string> _replacements = new Dictionary<int, string>();

			for (int noteIndex = 1; noteIndex < 9; noteIndex++)
			{
				int characterNum = Range.GetRandomValue(1, 4);              // Pick anywhere from 1 to 3 characters to reveal loved items for
				int itemNum = Range.GetRandomValue(5, 7) - characterNum;    // Reveal more items if there are fewer characters selected

				List<string> NoteNPCs = Globals.RNGGetRandomValuesFromList(NPC.GiftableNPCs, characterNum);
				string NPCLovesString = FormatRevealString(NoteNPCs, itemNum);

				string noteText = Globals.GetTranslation($"secret-note-{noteIndex}") + NPCLovesString;
				_replacements.Add(noteIndex, noteText);
			}

			WriteToSpoilerLog(_replacements);

			return _replacements;
		}

		/// <summary>
		/// Formats set of reveal commands (e.g. <c>"%revealtasteSam270%revealtasteMaru113..."</c>).
		/// </summary>
		/// <param name="NPCs">NPCs to reveal items for</param>
		/// <param name="itemNum">Number of items to reveal for each NPC</param>
		/// <returns><c>String</c> containing NPCs' preferences to reveal.</returns>
		private static string FormatRevealString(List<string> NPCs, int itemNum)
		{
			string lovesString = "";

			foreach (string NPC in NPCs)
			{
				string[] tokens = prefs[NPC].Split('/');
				List<string> items = tokens[1].Split(' ')                               // Split into individual item and category numbers
											  .ToList()                                 // Convert to List
											  .Where(x => int.Parse(x) > 0)             // Filter out negative numbers - these represent categories
											  .ToList();                                // And send back the results as a List

				for (int num = itemNum; num > 0; num--)
				{
					lovesString += GetItemRevealString(NPC, items);
				}
			}

			return lovesString;
		}

		/// <summary>
		/// Builds a single item reveal string (e.g. <c>%revealtasteAbigail206</c>).
		/// </summary>
		/// <param name="NPC">Name of the NPC to build the item reveal string for.</param>
		/// <returns>String representing item reveal command.</returns>
		private static string GetItemRevealString(string name, List<string> items)
		{
			if (items.Any())
			{
				return "%revealtaste" + name + Globals.RNGGetAndRemoveRandomValueFromList(items);
			}
			else
			{
				return "";
			}
		}

		private static void WriteToSpoilerLog(Dictionary<int, string> replacements)
		{
			if (!Globals.Config.NPCs.RandomizeIndividualPreferences || !Globals.Config.CreateSpoilerLog) { return; }

			Globals.SpoilerWrite("===== SECRET NOTES =====");
			foreach (KeyValuePair<int, string> pair in replacements)
			{
				Globals.SpoilerWrite($"{pair.Key}: {pair.Value}");
			}
			Globals.SpoilerWrite("");
		}

	}
}
