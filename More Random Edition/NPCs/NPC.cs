using System.Collections.Generic;

namespace Randomizer
{
	public class NPC
	{
		public static List<Item> UniversalLoves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.GoldenPumpkin],
			ItemList.Items[(int)ObjectIndexes.Pearl],
			ItemList.Items[(int)ObjectIndexes.PrismaticShard],
			ItemList.Items[(int)ObjectIndexes.RabbitsFoot]
		};

		/// <summary>
		/// Updates UniversalLoves list to provided List&lt;Item&gt;<paramref name="newUniversalLoves"/>.
		/// </summary>
		/// <param name="newUniversalLoves">New list of universal loves.</param>
		public static void UpdateUniversalLoves(List<Item> newUniversalLoves)
		{
			UniversalLoves = newUniversalLoves;
		}

		public static List<Item> UniversalHates;

		/// <summary>
		/// Updates UniversalHates list to provided List&lt;Item&gt;<paramref name="newUniversalHates"/>.
		/// </summary>
		/// <param name="newUniversalHates">New list of universal hates.</param>
		public static void UpdateUniversalHates(List<Item> newUniversalHates)
		{
			UniversalHates = newUniversalHates;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newNPCLoves"></param>
		public static void UpdateNPCLoves(string NPC, List<Item> newNPCLoves)
		{
			switch (NPC)
			{
				case "Abigail":
					Abigail.Loves = newNPCLoves;
					break;

				case "Alex":
					Alex.Loves = newNPCLoves;
					break;

				case "Caroline":
					Caroline.Loves = newNPCLoves;
					break;

				case "Clint":
					Clint.Loves = newNPCLoves;
					break;

				case "Dwarf":
					Dwarf.Loves = newNPCLoves;
					break;

				case "Demetrius":
					Demetrius.Loves = newNPCLoves;
					break;

				case "Elliott":
					Elliott.Loves = newNPCLoves;
					break;

				case "Emily":
					Emily.Loves = newNPCLoves;
					break;

				case "Evelyn":
					Evelyn.Loves = newNPCLoves;
					break;

				case "George":
					George.Loves = newNPCLoves;
					break;

				case "Gus":
					Gus.Loves = newNPCLoves;
					break;

				case "Haley":
					Haley.Loves = newNPCLoves;
					break;

				case "Harvey":
					Harvey.Loves = newNPCLoves;
					break;

				case "Jas":
					Jas.Loves = newNPCLoves;
					break;

				case "Jodi":
					Jodi.Loves = newNPCLoves;
					break;

				case "Kent":
					Kent.Loves = newNPCLoves;
					break;

				case "Krobus":
					Krobus.Loves = newNPCLoves;
					break;

				case "Leah":
					Leah.Loves = newNPCLoves;
					break;

				case "Lewis":
					Lewis.Loves = newNPCLoves;
					break;

				case "Linus":
					Linus.Loves = newNPCLoves;
					break;

				case "Marnie":
					Marnie.Loves = newNPCLoves;
					break;

				case "Maru":
					Maru.Loves = newNPCLoves;
					break;

				case "Pam":
					Pam.Loves = newNPCLoves;
					break;

				case "Robin":
					Robin.Loves = newNPCLoves;
					break;

				case "Sam":
					Sam.Loves = newNPCLoves;
					break;

				case "Sandy":
					Sandy.Loves = newNPCLoves;
					break;

				case "Sebastian":
					Sebastian.Loves = newNPCLoves;
					break;

				case "Shane":
					Shane.Loves = newNPCLoves;
					break;

				case "Vincent":
					Vincent.Loves = newNPCLoves;
					break;

				case "Willy":
					Willy.Loves = newNPCLoves;
					break;

				case "Wizard":
					Wizard.Loves = newNPCLoves;
					break;

				default:
					break;
			}
		}


		static NPC()
		{
			UniversalHates = new List<Item>
			{
				ItemList.Items[(int)ObjectIndexes.Bait],
				ItemList.Items[(int)ObjectIndexes.WildBait],
				ItemList.Items[(int)ObjectIndexes.Carp],
				ItemList.Items[(int)ObjectIndexes.CrabPot],
				ItemList.Items[(int)ObjectIndexes.DrumBlock],
				ItemList.Items[(int)ObjectIndexes.EnergyTonic],
				ItemList.Items[(int)ObjectIndexes.ExplosiveAmmo],
				ItemList.Items[(int)ObjectIndexes.FluteBlock],
				ItemList.Items[(int)ObjectIndexes.GrassStarter],
				ItemList.Items[(int)ObjectIndexes.GreenAlgae],
				ItemList.Items[(int)ObjectIndexes.Hay],
				ItemList.Items[(int)ObjectIndexes.IronOre],
				ItemList.Items[(int)ObjectIndexes.MermaidsPendant],
				ItemList.Items[(int)ObjectIndexes.MuscleRemedy],
				ItemList.Items[(int)ObjectIndexes.OilOfGarlic],
				ItemList.Items[(int)ObjectIndexes.Poppy],
				ItemList.Items[(int)ObjectIndexes.RainTotem],
				ItemList.Items[(int)ObjectIndexes.RedMushroom],
				ItemList.Items[(int)ObjectIndexes.Sap],
				ItemList.Items[(int)ObjectIndexes.WildBait],
				ItemList.Items[(int)ObjectIndexes.SeaUrchin],
				ItemList.Items[(int)ObjectIndexes.Seaweed],
				ItemList.Items[(int)ObjectIndexes.GreenSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.BlueSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.RedSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.PurpleSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.Snail],
				ItemList.Items[(int)ObjectIndexes.StrangeBun],
				ItemList.Items[(int)ObjectIndexes.Sugar],
				ItemList.Items[(int)ObjectIndexes.Torch],
				ItemList.Items[(int)ObjectIndexes.TreasureChest],
				ItemList.Items[(int)ObjectIndexes.VoidMayonnaise],
				ItemList.Items[(int)ObjectIndexes.WarpTotemBeach],
				ItemList.Items[(int)ObjectIndexes.WarpTotemFarm],
				ItemList.Items[(int)ObjectIndexes.WarpTotemMountains],
				ItemList.Items[(int)ObjectIndexes.WarpTotemDesert],
				ItemList.Items[(int)ObjectIndexes.WhiteAlgae],
				ItemList.Items[(int)ObjectIndexes.Slime],
				ItemList.Items[(int)ObjectIndexes.BugMeat],
				ItemList.Items[(int)ObjectIndexes.BatWing]
			};
			UniversalHates.AddRange(ItemList.GetTrash());
			UniversalHates.AddRange(ItemList.GetArtifacts());
		}

		/// <summary>
		/// All NPCs which can be gifted items.
		/// </summary>
		public static List<string> GiftableNPCs = new List<string>
		{
			"Robin",
			"Demetrius",
			"Maru",
			"Sebastian",
			"Linus",
			"Pierre",
			"Caroline",
			"Abigail",
			"Alex",
			"George",
			"Evelyn",
			"Lewis",
			"Clint",
			"Penny",
			"Pam",
			"Emily",
			"Haley",
			"Jas",
			"Vincent",
			"Jodi",
			"Kent",
			"Sam",
			"Leah",
			"Shane",
			"Marnie",
			"Elliott",
			"Gus",
			"Dwarf",
			"Wizard",
			"Harvey",
			"Sandy",
			"Willy",
			"Krobus",
			"Leo"
		};

		public static List<string> QuestableNPCsList = new List<string>
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
	}
}
