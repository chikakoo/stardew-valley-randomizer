using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using static StardewValley.LocalizedContentManager;

namespace Randomizer
{
	public class AssetLoader
	{
		private readonly ModEntry _mod;
		private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

		public AssetLoader(ModEntry mod)
		{
			_mod = mod;
		}

		public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			// Try to get the replacement asset from the replacements dictionary
			if (_replacements.TryGetValue(e.Name.BaseName, out string replacementAsset))
			{
				e.LoadFromModFile<Texture2D>(replacementAsset, AssetLoadPriority.Medium);
			}
		}

		private void AddReplacement(string originalAsset, string replacementAsset)
		{
			// Normalize the asset name so the keys are consistent
			IAssetName normalizedAssetName = _mod.Helper.GameContent.ParseAssetName(originalAsset);

			// Add the replacement to the dictionary
			_replacements[normalizedAssetName.BaseName] = replacementAsset;
		}

		public void InvalidateCache()
		{
			// Invalidate all replaced assets so that the changes are reapplied
			foreach (string assetName in _replacements.Keys)
			{
				_mod.Helper.GameContent.InvalidateCache(assetName);
			}
		}

		/// <summary>
		/// Nothing to do here at the moment
		/// </summary>
		public void CalculateReplacementsBeforeLoad()
		{
		}

		/// <summary>
		/// The current locale
		/// </summary>
		private string _currentLocale = "default";

		/// <summary>
		/// Replaces the title scrren graphics - done whenever the locale is changed or the game is first loaded
		/// Won't actually replace it if it already did
		/// </summary>
		public void TryReplaceTitleScreen()
		{
			IClickableMenu genericMenu = Game1.activeClickableMenu;
			if (genericMenu is null || genericMenu is not TitleMenu) { return; }

			if (_currentLocale != _mod.Helper.Translation.Locale)
			{
				ReplaceTitleScreen((TitleMenu)genericMenu);
			}
		}

		/// <summary>
		/// Replaces the title screen after returning from a game - called by the appropriate event handler
		/// </summary>
		public void ReplaceTitleScreenAfterReturning()
		{
			ReplaceTitleScreen();
		}

		/// <summary>
		/// Replaces the title screen graphics and refreshes the settings UI page
		/// </summary>
		/// <param name="titleMenu">The title menu - passed if we're already on the title screen</param>
		private void ReplaceTitleScreen(TitleMenu titleMenu = null)
		{
			_currentLocale = _mod.Helper.Translation.Locale;
			AddReplacement("Minigames/TitleButtons", $"Assets/Minigames/{Globals.GetTranslation("title-graphic")}");
			_mod.Helper.GameContent.InvalidateCache("Minigames/TitleButtons");

			if (titleMenu != null)
			{
				LanguageCode code = _mod.Helper.Translation.LocaleEnum;
				_mod.Helper.Reflection.GetMethod(titleMenu, "OnLanguageChange", true).Invoke(code);
			}

			_mod.CalculateAndInvalidateUIEdits();
		}

		/// <summary>
		/// Asset replacements
		/// TODO: make a class for this so that it's not in one giant file
		/// </summary>
		public void CalculateReplacements()
		{
			// Clear any previous replacements
			_replacements.Clear();

			if (Globals.Config.Crops.Randomize)
			{
				//TODO: probably get rid of this completely or move it somewhere
				//AddReplacement("Maps/springobjects", "Assets/Maps/springobjects.png");
			}

			if (Globals.Config.RandomizeAnimalSkins)
			{
				// Replace critters
				switch (Globals.RNG.Next(0, 4))
				{
					case 0:
						AddReplacement("TileSheets/critters", "Assets/TileSheets/crittersBears");
						break;
					case 1:
						AddReplacement("TileSheets/critters", "Assets/TileSheets/crittersseagullcrow");
						break;
					case 2:
						AddReplacement("TileSheets/critters", "Assets/TileSheets/crittersWsquirrelPseagull");
						break;
					case 3:
						AddReplacement("TileSheets/critters", "Assets/TileSheets/crittersBlueBunny");
						break;
				}

				//Change an animal to bear 
				int isPet = Globals.RNG.Next(1, 4);
				string[] Animal = new string[4]; Animal[0] = "Pig"; Animal[1] = "Goat"; Animal[2] = "Brown Cow"; Animal[3] = "White Cow";
				string[] Pet = new string[2]; Pet[0] = "cat"; Pet[1] = "dog";

				if (isPet == 1)
				{
					int petRng = Globals.RNG.Next(0, Pet.Length - 1);
					AddReplacement($"Animals/{Pet[petRng]}", "Assets/Characters/BearDog");
				}
				if (isPet == 2)
				{
					AddReplacement($"Animals/horse", "Assets/Characters/BearHorse");
				}
				else
				{
					int animalRng = Globals.RNG.Next(0, Animal.Length - 1);
					AddReplacement($"Animals/{Animal[animalRng]}", "Assets/Characters/Bear");
					AddReplacement($"Animals/Baby{Animal[animalRng]}", "Assets/Characters/BabyBear");
				}
			}

			// Character swaps
			if (Globals.Config.RandomizeNPCSkins)
			{
				// Keep track of all swaps made
				Dictionary<string, string> currentSwaps = new Dictionary<string, string>();

				// Copy the array of possible swaps to a new list
				List<PossibleSwap> possibleSwaps = _mod.PossibleSwaps.ToList();

				// Make swaps until either a random number of swaps are made or we run out of possible swaps to make
				int swapsRemaining = Globals.RNG.Next(5, 11);
				while (swapsRemaining > 0 && possibleSwaps.Any())
				{
					// Get a random possible swap
					int index = Globals.RNG.Next(0, possibleSwaps.Count);
					PossibleSwap swap = possibleSwaps[index];

					// Remove it from the list so it isn't chosen again
					possibleSwaps.RemoveAt(index);

					// Check if the characters haven't been swapped yet
					if (currentSwaps.ContainsKey(swap.FirstCharacter) || currentSwaps.ContainsKey(swap.SecondCharacter))
					{
						continue;
					}

					// Add the swap to the dictionary
					currentSwaps[swap.FirstCharacter] = swap.SecondCharacter;
					currentSwaps[swap.SecondCharacter] = swap.FirstCharacter;
					_mod.Monitor.Log($"Swapping {swap.FirstCharacter} and {swap.SecondCharacter}");

					// Add the replacements
					AddReplacement($"Characters/{swap.FirstCharacter}", $"Assets/Characters/{swap.SecondCharacter}");
					AddReplacement($"Characters/{swap.SecondCharacter}", $"Assets/Characters/{swap.FirstCharacter}");
					AddReplacement($"Portraits/{swap.FirstCharacter}", $"Assets/Portraits/{swap.SecondCharacter}");
					AddReplacement($"Portraits/{swap.SecondCharacter}", $"Assets/Portraits/{swap.FirstCharacter}");

					// Decrement the number of swaps remaining
					swapsRemaining--;
				}
			}

			ReplaceRain();
		}

		/// <summary>
		/// Randomizes the images - depending on what settings are on
		/// It's still important to build the images to make sure seeds are consistent
		/// </summary>
		public void RandomizeImages()
		{
			WeaponImageBuilder weaponImageBuilder = new WeaponImageBuilder();
			weaponImageBuilder.BuildImage();
			HandleImageReplacement(weaponImageBuilder, "TileSheets/weapons");

			CropGrowthImageBuilder cropGrowthImageBuilder = new CropGrowthImageBuilder();
			cropGrowthImageBuilder.BuildImage();
			HandleImageReplacement(cropGrowthImageBuilder, "TileSheets/crops");

			SpringObjectsImageBuilder springObjectsImageBuilder = new SpringObjectsImageBuilder(cropGrowthImageBuilder.CropIdsToImageNames);
			springObjectsImageBuilder.BuildImage();
			HandleImageReplacement(springObjectsImageBuilder, "Maps/springobjects");

			BundleImageBuilder bundleImageBuilder = new BundleImageBuilder();
			bundleImageBuilder.BuildImage();
			HandleImageReplacement(bundleImageBuilder, "LooseSprites/JunimoNote");
		}

		/// <summary>
		/// Handles actually adding the image replacement
		/// If the image doesn't exist, sleep for 0.1 second increments until it does
		/// </summary>
		/// <param name="imageBuilder">The image builder</param>
		/// <param name="xnbPath">The path to the xnb image to replace</param>
		private void HandleImageReplacement(ImageBuilder imageBuilder, string xnbPath)
		{
			if (imageBuilder.ShouldSaveImage())
			{
				while (!File.Exists(imageBuilder.OutputFileFullPath))
				{
					Thread.Sleep(100);
				}
				AddReplacement(xnbPath, imageBuilder.SMAPIOutputFilePath);
			}
		}

		/// <summary>
		/// Replaces the rain - intended to be called once per day start
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		/// </summary>
		public void ReplaceRain(object sender = null, DayEndingEventArgs e = null)
		{
			if (!Globals.Config.RandomizeRain) { return; }
			if (Globals.RNG == null) { return; }

			RainTypes rainType = Globals.RNGGetRandomValueFromList(
				Enum.GetValues(typeof(RainTypes)).Cast<RainTypes>().ToList());

			AddReplacement("TileSheets/rain", $"Assets/TileSheets/{rainType}Rain");
			_mod.Helper.GameContent.InvalidateCache("TileSheets/rain");
		}
	}
}