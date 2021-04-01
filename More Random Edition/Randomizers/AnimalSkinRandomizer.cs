using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class AnimalSkinRandomizer
	{
		/// <summary>
		/// Represents the type of animal replacement we'll be doing
		/// </summary>
		private enum AnimalReplacementType
		{
			Pet,
			Horse,
			Animal
		}

		private static Dictionary<string, string> _replacements;
		private readonly static List<string> PossibleCritterReplacements = new List<string>()
		{
			"crittersBears",
			"crittersseagullcrow",
			"crittersWsquirrelPseagull",
			"crittersBlueBunny"
		};

		private static string _critterSpoilerString;
		private static string _animalSpoilerString;

		/// <summary>
		/// Randomizes animal skins
		/// Replaces the critters tilesheet and replaces a random animal with a bear
		/// </summary>
		/// <returns>The dictionary for the AssetLoader to use</returns>
		public static Dictionary<string, string> Randomize()
		{
			_replacements = new Dictionary<string, string>();

			AddCritterReplacement(Globals.RNGGetRandomValueFromList(PossibleCritterReplacements));
			DoAnimalReplacements();

			if (!Globals.Config.RandomizeAnimalSkins)
			{
				return new Dictionary<string, string>();
			}

			WriteToSpoilerLog();
			return _replacements;
		}

		/// <summary>
		/// Adds an entry to the replacements to replace a random animal with a bear
		/// </summary>
		private static void DoAnimalReplacements()
		{
			AnimalReplacementType replacementType = Globals.RNGGetRandomValueFromList(
				Enum.GetValues(typeof(AnimalReplacementType)).Cast<AnimalReplacementType>().ToList());

			switch (replacementType)
			{
				case AnimalReplacementType.Pet:
					List<string> pets = new List<string> { "cat", "dog" };
					string pet = Globals.RNGGetRandomValueFromList(pets);

					AddAnimalReplacement(pet, "BearDog");
					break;
				case AnimalReplacementType.Horse:
					AddAnimalReplacement("horse", "BearHorse");
					break;
				default:
					List<string> animals = new List<string> { "Pig", "Goat", "Brown Cow", "White Cow" };
					string animal = Globals.RNGGetRandomValueFromList(animals);

					AddAnimalReplacement(animal, "Bear");
					AddAnimalReplacement($"Baby{animal}", "BabyBear");
					break;
			}
		}

		/// <summary>
		/// Adds the critter replacement to the dictionary
		/// </summary>
		/// <param name="critterName">The critter asset name</param>
		private static void AddCritterReplacement(string critterName)
		{
			_critterSpoilerString = $"Critter sheet replaced with {critterName}";
			_replacements.Add("TileSheets/critters", $"Assets/TileSheets/{critterName}");
		}

		/// <summary>
		/// Adds the animal replacement to the dictionary
		/// </summary>
		/// <param name="oldAnimal">The old animal asset name</param>
		/// <param name="newAnimal">The new animal asset name</param>
		private static void AddAnimalReplacement(string oldAnimal, string newAnimal)
		{
			_animalSpoilerString = $"{oldAnimal} replaced with {newAnimal}";
			_replacements.Add($"Animals/{oldAnimal}", $"Assets/Characters/{newAnimal}");
		}

		/// <summary>
		/// Writes the NPC replacements to the spoiler log
		/// </summary>
		private static void WriteToSpoilerLog()
		{
			Globals.SpoilerWrite("==== ANIMAL SKINS ====");
			Globals.SpoilerWrite(_critterSpoilerString);
			Globals.SpoilerWrite(_animalSpoilerString);
			Globals.SpoilerWrite("");
		}
	}
}
