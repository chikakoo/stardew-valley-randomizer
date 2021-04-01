using System.Collections.Generic;

namespace Randomizer
{
	public class NPCSkinRandomizer
	{
		/// <summary>
		/// Used to easily calculate all the replacement paths for an npc swap
		/// </summary>
		private class NPCSkinSwapPaths
		{
			public string OriginalNPC { get; }
			public string ReplacementNPC { get; }

			public string OriginalCharacterPath
			{
				get
				{
					return $"Characters/{OriginalNPC}";
				}
			}

			public string ReplacementCharacterPath
			{
				get
				{
					return $"Assets/Characters/{ReplacementNPC}";
				}
			}

			public string OriginalPortraitPath
			{
				get
				{
					return $"Portraits/{OriginalNPC}";
				}
			}

			public string ReplacementPortraitPath
			{
				get
				{
					return $"Assets/Portraits/{ReplacementNPC}";
				}
			}

			public NPCSkinSwapPaths(string originalNPC, string replacementNPC)
			{
				OriginalNPC = originalNPC;
				ReplacementNPC = replacementNPC;
			}
		}

		/// <summary>
		/// The list of possible swaps
		/// Dwarf, Krobus, Pam, and Willy are not included in this list to prevent glitchy graphics
		/// </summary>
		private static readonly List<string> _possibleSwaps = new List<string>
		{
			"Abigail",
			"Alex",
			"Caroline",
			"Demetrius",
			"Elliott",
			"Emily",
			"Evelyn",
			"George",
			"Gunther",
			"Gus",
			"Haley",
			"Harvey",
			"Jas",
			"Kent",
			"Leah",
			"Lewis",
			"Linus",
			"Marnie",
			"Maru",
			"Maru_Hospital",
			"Marlon",
			"Morris",
			"MrQi",
			"Penny",
			"Pierre",
			"Robin",
			"Sam",
			"Sandy",
			"Sebastian",
			"Shane",
			"Vincent",
			"Wizard"
		};

		private static Dictionary<string, string> _swaps;
		private static List<NPCSkinSwapPaths> _swapData;

		/// <summary>
		/// Returns a list of shuffled up character replacements
		/// </summary>
		/// <returns>The dictionary of replacements for the AssetLoader to use</returns>
		public static Dictionary<string, string> Randomize()
		{
			_swaps = new Dictionary<string, string>();
			_swapData = new List<NPCSkinSwapPaths>();
			List<string> possibleSwaps = new List<string>(_possibleSwaps);

			foreach (string npc in _possibleSwaps)
			{
				string replacementNPC = Globals.RNGGetAndRemoveRandomValueFromList(possibleSwaps);
				NPCSkinSwapPaths npcPaths = new NPCSkinSwapPaths(npc, replacementNPC);
				AddNPCSwap(npcPaths);
			}

			// Specific swaps to prevent glitchy graphics
			TryAdditionalSwap(new NPCSkinSwapPaths("Henchman", "Bouncer"));
			TryAdditionalSwap(new NPCSkinSwapPaths("Willy", "Pam"));
			TryAdditionalSwap(new NPCSkinSwapPaths("Dwarf", "Krobus"));

			if (!Globals.Config.NPCs.RandomizeSkins)
			{
				return new Dictionary<string, string>();
			}

			WriteToSpoilerLog();
			return _swaps;
		}

		/// <summary>
		/// Adds the NPC swap to the dictionary
		/// </summary>
		/// <param name="swap">The swap</param>
		private static void AddNPCSwap(NPCSkinSwapPaths swap)
		{
			_swapData.Add(swap);
			_swaps.Add(swap.OriginalCharacterPath, swap.ReplacementCharacterPath);
			_swaps.Add(swap.OriginalPortraitPath, swap.ReplacementPortraitPath);
		}

		/// <summary>
		/// Tries to add a swap to the list at a 50% rate
		/// Also needs to add the swap in the reverse order!
		/// </summary>
		/// <param name="swap">The swap to attempt to add</param>
		private static void TryAdditionalSwap(NPCSkinSwapPaths swap)
		{
			if (Globals.RNGGetNextBoolean())
			{
				NPCSkinSwapPaths reverseSwap = new NPCSkinSwapPaths(swap.ReplacementNPC, swap.OriginalNPC);
				AddNPCSwap(swap);
				AddNPCSwap(reverseSwap);
			}
		}

		/// <summary>
		/// Writes the NPC replacements to the spoiler log
		/// </summary>
		private static void WriteToSpoilerLog()
		{
			Globals.SpoilerWrite("==== NPC SKINS ====");
			foreach (NPCSkinSwapPaths npcSwap in _swapData)
			{
				Globals.SpoilerWrite($"{npcSwap.OriginalNPC}\t->\t{npcSwap.ReplacementNPC}");
			}
			Globals.SpoilerWrite("");
		}
	}
}
