using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Randomizer
{
	/// <summary>The mod entry point</summary>
	public class ModEntry : Mod
	{
		private AssetLoader _modAssetLoader;
		private AssetEditor _modAssetEditor;

		private IModHelper _helper;

		static IGenericModConfigMenuAPI api;

		/// <summary>The mod entry point, called after the mod is first loaded</summary>
		/// <param name="helper">Provides simplified APIs for writing mods</param>
		public override void Entry(IModHelper helper)
		{
			_helper = helper;
			Globals.ModRef = this;
			Globals.Config = Helper.ReadConfig<ModConfig>();

			ImageBuilder.CleanUpReplacementFiles();

			_modAssetLoader = new AssetLoader(this);
			_modAssetEditor = new AssetEditor(this);
			helper.Content.AssetLoaders.Add(_modAssetLoader);
			helper.Content.AssetEditors.Add(_modAssetEditor);

			PreLoadReplacments();
			helper.Events.GameLoop.GameLaunched += (sender, args) => TryLoadModConfigMenu();
			helper.Events.GameLoop.SaveLoaded += (sender, args) => CalculateAllReplacements();
			helper.Events.Display.RenderingActiveMenu += (sender, args) => _modAssetLoader.TryReplaceTitleScreen();
			helper.Events.GameLoop.ReturnedToTitle += (sender, args) => _modAssetLoader.ReplaceTitleScreenAfterReturning();

			if (Globals.Config.Music.Randomize) { helper.Events.GameLoop.UpdateTicked += (sender, args) => MusicRandomizer.TryReplaceSong(); }
			if (Globals.Config.RandomizeRain) { helper.Events.GameLoop.DayEnding += _modAssetLoader.ReplaceRain; }

			if (Globals.Config.Crops.Randomize)
			{
				helper.Events.Multiplayer.PeerContextReceived += (sender, args) => MenuAdjustments.FixParsnipSeedBox();
			}

			if (Globals.Config.Crops.Randomize || Globals.Config.Fish.Randomize)
			{
				helper.Events.Display.RenderingActiveMenu += (sender, args) => CraftingRecipeAdjustments.HandleCraftingMenus();

				// Fix for the Special Orders causing crashes
				// Re-instate the object info when the save is first loaded for the session, and when saving so that the
				// items have the correct names on the items sold summary screen
				helper.Events.GameLoop.DayEnding += (sender, args) => _modAssetEditor.UndoObjectInformationReplacements();
				helper.Events.GameLoop.SaveLoaded += (sender, args) => _modAssetEditor.RedoObjectInformationReplacements();
				helper.Events.GameLoop.Saving += (sender, args) => _modAssetEditor.RedoObjectInformationReplacements();
			}

			if (Globals.Config.RandomizeForagables)
			{
				helper.Events.GameLoop.GameLaunched += (sender, args) => WildSeedAdjustments.ReplaceGetRandomWildCropForSeason();
			}

			if (Globals.Config.Fish.Randomize)
			{
				helper.Events.GameLoop.DayStarted += (sender, args) => OverriddenSubmarine.UseOverriddenSubmarine();
				helper.Events.GameLoop.DayEnding += (sender, args) => OverriddenSubmarine.RestoreSubmarineLocation();
			}

			if (Globals.Config.Bundles.Randomize)
			{
				helper.Events.Display.RenderingActiveMenu += (sender, args) => BundleMenuAdjustments.FixRingDeposits();

				if (Globals.Config.Bundles.ShowDescriptionsInBundleTooltips)
				{
					helper.Events.Display.RenderedActiveMenu += (sender, args) => BundleMenuAdjustments.AddDescriptionsToBundleTooltips();
				}
			}

			if (Globals.Config.Bundles.Randomize || Globals.Config.Shops.RandomizeMainShops || Globals.Config.Shops.RandomizeMiscShops)
			{
				helper.Events.Display.MenuChanged += MenuAdjustments.TryAdjustMenu;
			}
		}

		private void TryLoadModConfigMenu()
		{
			// Check to see if Generic Mod Config Menu is installed
			if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
			{
				Globals.ConsoleTrace("GenericModConfigMenu not present - skipping mod menu setup");
				return;
			}

			api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			api.RegisterModConfig(ModManifest, () => Globals.Config = new ModConfig(), () => Helper.WriteConfig(Globals.Config));

			ModConfigMenuHelper menuHelper = new ModConfigMenuHelper(api, ModManifest);
			menuHelper.RegisterModOptions();

		}

		/// <summary>
		/// Loads the replacements that can be loaded before a game is selected
		/// </summary>
		public void PreLoadReplacments()
		{
			_modAssetLoader.CalculateReplacementsBeforeLoad();
			_modAssetEditor.CalculateEditsBeforeLoad();
		}

		/// <summary>
		/// Does all the randomizer replacements that take place after a game is loaded
		/// </summary>
		public void CalculateAllReplacements()
		{
			//Seed is pulled from farm name
			byte[] seedvar = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value));
			int seed = BitConverter.ToInt32(seedvar, 0);

			Monitor.Log($"Seed Set: {seed}");

			Globals.RNG = new Random(seed);
			Globals.SpoilerLog = new SpoilerLogger(Game1.player.farmName.Value);

			// Make replacements and edits
			_modAssetLoader.CalculateReplacements();
			_modAssetEditor.CalculateEdits();
			_modAssetLoader.RandomizeImages();
			Globals.SpoilerLog.WriteFile();

			// Invalidate all replaced and edited assets so they are reloaded
			_modAssetLoader.InvalidateCache();
			_modAssetEditor.InvalidateCache();

			// Ensure that the bundles get changed if they're meant to
			Game1.GenerateBundles(Game1.bundleType, true);

			ChangeDayOneForagables();
			MenuAdjustments.FixParsnipSeedBox();
		}

		/// <summary>
		/// A passthrough to calculate adn invalidate UI edits
		/// Used when the lanauage is changed
		/// </summary>
		public void CalculateAndInvalidateUIEdits()
		{
			_modAssetEditor.CalculateAndInvalidateUIEdits();
		}

		/// <summary>
		/// Fixes the foragables on day 1 - the save file is created too quickly for it to be
		/// randomized right away, so we'll change them on the spot on the first day
		/// </summary>
		public void ChangeDayOneForagables()
		{
			SDate currentDate = SDate.Now();
			if (currentDate.DaysSinceStart < 2)
			{
				List<GameLocation> locations = Game1.locations
					.Concat(
						from location in Game1.locations.OfType<BuildableGameLocation>()
						from building in location.buildings
						where building.indoors.Value != null
						select building.indoors.Value
					).ToList();

				List<Item> newForagables =
					ItemList.GetForagables(Seasons.Spring)
						.Where(x => x.ShouldBeForagable) // Removes the 1/1000 items
						.Cast<Item>().ToList();

				foreach (GameLocation location in locations)
				{
					List<int> foragableIds = ItemList.GetForagables().Select(x => x.Id).ToList();
					List<Vector2> tiles = location.Objects.Pairs
						.Where(x => foragableIds.Contains(x.Value.ParentSheetIndex))
						.Select(x => x.Key)
						.ToList();

					foreach (Vector2 oldForagableKey in tiles)
					{
						Item newForagable = Globals.RNGGetRandomValueFromList(newForagables, true);
						location.Objects[oldForagableKey].ParentSheetIndex = newForagable.Id;
						location.Objects[oldForagableKey].Name = newForagable.Name;
					}
				}
			}
		}


	}
}