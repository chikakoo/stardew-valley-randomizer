using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class AssetEditor : IAssetEditor
	{
		private readonly ModEntry _mod;
		private Dictionary<string, string> _recipeReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _bundleReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _blueprintReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _uiStringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _grandpaStringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _stringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _locationStringReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _fishReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _questReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _mailReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _locationsReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _objectInformationReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _fruitTreeReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _cropReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _cookingChannelReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _weaponReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _bootReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _monsterReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _birthdayReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _preferenceReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _secretNotesReplacements = new Dictionary<int, string>();


		/// <summary>
		/// Whether we're currently ignoring replacing object information
		/// This is done between day loads to prevent errors with the Special Orders
		/// Eventually this can be removed when we modify the orders themselves
		/// </summary>
		private bool IgnoreObjectInformationReplacements { get; set; }

		public AssetEditor(ModEntry mod)
		{
			_mod = mod;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Data/CraftingRecipes")) { return Globals.Config.CraftingRecipes.Randomize; }
			if (asset.AssetNameEquals("Data/Bundles")) { return Globals.Config.Bundles.Randomize; }
			if (asset.AssetNameEquals("Data/Blueprints")) { return Globals.Config.RandomizeBuildingCosts; }
			if (asset.AssetNameEquals("Strings/StringsFromCSFiles")) { return true; }
			if (asset.AssetNameEquals("Strings/UI")) { return true; }
			if (asset.AssetNameEquals("Data/ObjectInformation")) { return true; }
			if (asset.AssetNameEquals("Data/Fish")) { return Globals.Config.Fish.Randomize; }
			if (asset.AssetNameEquals("Data/Quests") || asset.AssetNameEquals("Data/mail")) { return Globals.Config.RandomizeQuests; }
			if (asset.AssetNameEquals("Data/Locations")) { return Globals.Config.Fish.Randomize || Globals.Config.RandomizeForagables || Globals.Config.AddRandomArtifactItem; }
			if (asset.AssetNameEquals("Strings/Locations")) { return Globals.Config.Crops.Randomize; } // For now, as the only thing is the sweet gem berry text
			if (asset.AssetNameEquals("Data/fruitTrees")) { return Globals.Config.RandomizeFruitTrees; }
			if (asset.AssetNameEquals("Data/Crops")) { return Globals.Config.Crops.Randomize; }
			if (asset.AssetNameEquals("Data/TV/CookingChannel")) { return Globals.Config.Crops.Randomize || Globals.Config.Fish.Randomize; }
			if (asset.AssetNameEquals("Data/weapons")) { return Globals.Config.Weapons.Randomize; }
			if (asset.AssetNameEquals("Data/Boots")) { return Globals.Config.Boots.Randomize; }
			if (asset.AssetNameEquals("Data/Monsters")) { return Globals.Config.Monsters.Randomize; }
			if (asset.AssetNameEquals("Data/NPCDispositions")) { return Globals.Config.NPCs.RandomizeBirthdays; }
			if (asset.AssetNameEquals("Data/NPCGiftTastes")) { return Globals.Config.NPCs.RandomizeIndividualPreferences || Globals.Config.NPCs.RandomizeUniversalPreferences; }
			if (asset.AssetNameEquals("Data/SecretNotes")) { return Globals.Config.NPCs.RandomizeIndividualPreferences; }
			return false;
		}

		private void ApplyEdits<TKey, TValue>(IAssetData asset, IDictionary<TKey, TValue> edits)
		{
			IAssetDataForDictionary<TKey, TValue> assetDict = asset.AsDictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> edit in edits)
			{
				assetDict.Data[edit.Key] = edit.Value;
			}
		}

		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals("Data/CraftingRecipes"))
			{
				ApplyEdits(asset, _recipeReplacements);
			}
			else if (asset.AssetNameEquals("Data/Bundles"))
			{
				ApplyEdits(asset, _bundleReplacements);
			}
			else if (asset.AssetNameEquals("Data/Blueprints"))
			{
				ApplyEdits(asset, _blueprintReplacements);
			}
			else if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
			{
				ApplyEdits(asset, _grandpaStringReplacements);
				ApplyEdits(asset, _stringReplacements);
			}
			else if (asset.AssetNameEquals("Strings/UI"))
			{
				ApplyEdits(asset, _uiStringReplacements);
			}
			else if (asset.AssetNameEquals("Data/ObjectInformation"))
			{
				if (IgnoreObjectInformationReplacements)
				{
					ApplyEdits(asset, new Dictionary<int, string>());
				}
				else
				{
					ApplyEdits(asset, _objectInformationReplacements);
				}
			}
			else if (asset.AssetNameEquals("Data/Fish"))
			{
				ApplyEdits(asset, _fishReplacements);
			}
			else if (asset.AssetNameEquals("Data/Quests"))
			{
				ApplyEdits(asset, _questReplacements);
			}
			else if (asset.AssetNameEquals("Data/mail"))
			{
				ApplyEdits(asset, _mailReplacements);
			}
			else if (asset.AssetNameEquals("Data/Locations"))
			{
				ApplyEdits(asset, _locationsReplacements);
			}
			else if (asset.AssetNameEquals("Strings/Locations"))
			{
				ApplyEdits(asset, _locationStringReplacements);
			}
			else if (asset.AssetNameEquals("Data/fruitTrees"))
			{
				ApplyEdits(asset, _fruitTreeReplacements);
			}
			else if (asset.AssetNameEquals("Data/Crops"))
			{
				ApplyEdits(asset, _cropReplacements);
			}
			else if (asset.AssetNameEquals("Data/TV/CookingChannel"))
			{
				ApplyEdits(asset, _cookingChannelReplacements);
			}
			else if (asset.AssetNameEquals("Data/weapons"))
			{
				ApplyEdits(asset, _weaponReplacements);
			}
			else if (asset.AssetNameEquals("Data/Boots"))
			{
				ApplyEdits(asset, _bootReplacements);
			}
			else if (asset.AssetNameEquals("Data/Monsters"))
			{
				ApplyEdits(asset, _monsterReplacements);
			}
			else if (asset.AssetNameEquals("Data/NPCDispositions"))
			{
				ApplyEdits(asset, _birthdayReplacements);
			}
			else if (asset.AssetNameEquals("Data/NPCGiftTastes"))
			{
				ApplyEdits(asset, _preferenceReplacements);
			}
			else if (asset.AssetNameEquals("Data/SecretNotes"))
			{
				ApplyEdits(asset, _secretNotesReplacements);
			}
		}

		public void InvalidateCache()
		{
			_mod.Helper.Content.InvalidateCache("Data/CraftingRecipes");
			_mod.Helper.Content.InvalidateCache("Data/Bundles");
			_mod.Helper.Content.InvalidateCache("Data/Blueprints");
			_mod.Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
			_mod.Helper.Content.InvalidateCache("Strings/UI");
			_mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
			_mod.Helper.Content.InvalidateCache("Data/Events/Farm");
			_mod.Helper.Content.InvalidateCache("Data/Fish");
			_mod.Helper.Content.InvalidateCache("Data/Quests");
			_mod.Helper.Content.InvalidateCache("Data/mail");
			_mod.Helper.Content.InvalidateCache("Data/Locations");
			_mod.Helper.Content.InvalidateCache("Strings/Locations");
			_mod.Helper.Content.InvalidateCache("Data/fruitTrees");
			_mod.Helper.Content.InvalidateCache("Data/Crops");
			_mod.Helper.Content.InvalidateCache("Data/TV/CookingChannel");
			_mod.Helper.Content.InvalidateCache("Data/weapons");
			_mod.Helper.Content.InvalidateCache("Data/Boots");
			_mod.Helper.Content.InvalidateCache("Data/Monsters");
			_mod.Helper.Content.InvalidateCache("Data/NPCDispositions");
			_mod.Helper.Content.InvalidateCache("Data/NPCGiftTastes");
			_mod.Helper.Content.InvalidateCache("Data/SecretNotes");
		}

		/// <summary>
		/// Calculates edits that need to happen before a save file is loaded
		/// </summary>
		public void CalculateEditsBeforeLoad()
		{
			CalculateAndInvalidateUIEdits();
			_grandpaStringReplacements = StringsAdjustments.RandomizeGrandpasStory();
		}

		/// <summary>
		/// Calculates the UI string replacements and invalidates the cache so it can be updated
		/// Should be called on game load and after a language change
		/// </summary>
		public void CalculateAndInvalidateUIEdits()
		{
			_uiStringReplacements = StringsAdjustments.ModifyRemixedBundleUI();
			_mod.Helper.Content.InvalidateCache("Strings/UI");
		}

		public void CalculateEdits()
		{
			ItemList.Initialize();
			ValidateItemList();

			EditedObjectInformation editedObjectInfo = new EditedObjectInformation();
			FishRandomizer.Randomize(editedObjectInfo);
			_fishReplacements = editedObjectInfo.FishReplacements;

			CropRandomizer.Randomize(editedObjectInfo);
			_fruitTreeReplacements = editedObjectInfo.FruitTreeReplacements;
			_cropReplacements = editedObjectInfo.CropsReplacements;
			_objectInformationReplacements = editedObjectInfo.ObjectInformationReplacements;

			_blueprintReplacements = BlueprintRandomizer.Randomize();
			_monsterReplacements = MonsterRandomizer.Randomize(); // Must be done before recipes since rarities of drops change
			_locationsReplacements = LocationRandomizer.Randomize(); // Must be done before recipes because of wild seeds
			_recipeReplacements = CraftingRecipeRandomizer.Randomize();
			_stringReplacements = StringsAdjustments.GetCSFileStringReplacements();
			_locationStringReplacements = StringsAdjustments.GetLocationStringReplacements();
			CraftingRecipeAdjustments.FixCookingRecipeDisplayNames();
			_cookingChannelReplacements = CookingChannelAdjustments.GetTextEdits();

			// Needs to run after Cooking Recipe fix so that cooked items are properly named,
			// and needs to run before bundles so that NPC Loved Item bundles are properly generated
			_preferenceReplacements = PreferenceRandomizer.Randomize();
			_secretNotesReplacements = SecretNotesRandomizer.FixSecretNotes(_preferenceReplacements);

			_bundleReplacements = BundleRandomizer.Randomize();
			MusicRandomizer.Randomize();

			QuestInformation questInfo = QuestRandomizer.Randomize();
			_questReplacements = questInfo.QuestReplacements;
			_mailReplacements = questInfo.MailReplacements;


			_weaponReplacements = WeaponRandomizer.Randomize();
			_bootReplacements = BootRandomizer.Randomize();
			_birthdayReplacements = BirthdayRandomizer.Randomize();
		}

		/// <summary>
		/// Turns on the flag to ignore object information replacements and invalidates the cache
		/// so that the original values are reloaded
		/// </summary>
		public void UndoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = true;
			_mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
		}

		/// <summary>
		/// Turns off the flag to ignore object information replacements and invalidates the cache
		/// so that the randomized values are reloaded
		/// </summary>
		public void RedoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = false;
			_mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
		}

		/// <summary>
		/// Validates that all the items in the ObjectIndexes exist in the main item list
		/// </summary>
		private void ValidateItemList()
		{
			foreach (ObjectIndexes index in Enum.GetValues(typeof(ObjectIndexes)).Cast<ObjectIndexes>())
			{
				if (!ItemList.Items.ContainsKey((int)index))
				{
					Globals.ConsoleWarn($"Missing item: {(int)index}: {index}");
				}
			}
		}
	}
}