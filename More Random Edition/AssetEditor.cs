using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class AssetEditor
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

		/// <summary>
		/// Whether we're currently ignoring replacing object information
		/// This is done between day loads to prevent errors with the Special Orders
		/// Eventually this can be removed when we modify the orders themselves
		/// </summary>
		private bool IgnoreObjectInformationReplacements { get; set; }

		public AssetEditor(ModEntry mod)
		{
			this._mod = mod;
		}

		private void ApplyEdits<TKey, TValue>(IAssetData asset, IDictionary<TKey, TValue> edits)
		{
			IAssetDataForDictionary<TKey, TValue> assetDict = asset.AsDictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> edit in edits)
			{
				assetDict.Data[edit.Key] = edit.Value;
			}
		}

		public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			if (e.Name.IsEquivalentTo("Data/CraftingRecipes"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _recipeReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Bundles"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _bundleReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Blueprints"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _blueprintReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Strings/StringsFromCSFiles"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _grandpaStringReplacements); });
				e.Edit((asset) => { ApplyEdits(asset, _stringReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Strings/UI"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _uiStringReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/ObjectInformation"))
			{
				if (IgnoreObjectInformationReplacements)
				{
					e.Edit((asset) => { ApplyEdits(asset, new Dictionary<int, string>()); });
				}
				else
				{
					e.Edit((asset) => { ApplyEdits(asset, _objectInformationReplacements); });
				}
			}
			else if (e.Name.IsEquivalentTo("Data/Fish"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _fishReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Quests"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _questReplacements); });
			}
			// Not sure if this is intentional or should be else if
			if (e.Name.IsEquivalentTo("Data/mail"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _mailReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Locations"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _locationsReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Strings/Locations"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _locationStringReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/fruitTrees"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _fruitTreeReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Crops"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _cropReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/TV/CookingChannel"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _cookingChannelReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/weapons"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _weaponReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Boots"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _bootReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/Monsters"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _monsterReplacements); });
			}
			else if (e.Name.IsEquivalentTo("Data/NPCDispositions"))
			{
				e.Edit((asset) => { ApplyEdits(asset, _birthdayReplacements); });
			}
		}

		public void InvalidateCache()
		{
			_mod.Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
			_mod.Helper.GameContent.InvalidateCache("Data/Bundles");
			_mod.Helper.GameContent.InvalidateCache("Data/Blueprints");
			_mod.Helper.GameContent.InvalidateCache("Strings/StringsFromCSFiles");
			_mod.Helper.GameContent.InvalidateCache("Strings/UI");
			_mod.Helper.GameContent.InvalidateCache("Data/ObjectInformation");
			_mod.Helper.GameContent.InvalidateCache("Data/Events/Farm");
			_mod.Helper.GameContent.InvalidateCache("Data/Fish");
			_mod.Helper.GameContent.InvalidateCache("Data/Quests");
			_mod.Helper.GameContent.InvalidateCache("Data/mail");
			_mod.Helper.GameContent.InvalidateCache("Data/Locations");
			_mod.Helper.GameContent.InvalidateCache("Strings/Locations");
			_mod.Helper.GameContent.InvalidateCache("Data/fruitTrees");
			_mod.Helper.GameContent.InvalidateCache("Data/Crops");
			_mod.Helper.GameContent.InvalidateCache("Data/TV/CookingChannel");
			_mod.Helper.GameContent.InvalidateCache("Data/weapons");
			_mod.Helper.GameContent.InvalidateCache("Data/Boots");
			_mod.Helper.GameContent.InvalidateCache("Data/Monsters");
			_mod.Helper.GameContent.InvalidateCache("Data/NPCDispositions");
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
			_mod.Helper.GameContent.InvalidateCache("Strings/UI");
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
			_bundleReplacements = BundleRandomizer.Randomize();
			MusicRandomizer.Randomize();

			QuestInformation questInfo = QuestRandomizer.Randomize();
			_questReplacements = questInfo.QuestReplacements;
			_mailReplacements = questInfo.MailReplacements;

			CraftingRecipeAdjustments.FixCookingRecipeDisplayNames();
			_cookingChannelReplacements = CookingChannel.GetTextEdits();

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
			_mod.Helper.GameContent.InvalidateCache("Data/ObjectInformation");
		}

		/// <summary>
		/// Turns off the flag to ignore object information replacements and invalidates the cache
		/// so that the randomized values are reloaded
		/// </summary>
		public void RedoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = false;
			_mod.Helper.GameContent.InvalidateCache("Data/ObjectInformation");
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