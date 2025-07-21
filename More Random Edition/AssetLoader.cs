using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer;

public class AssetLoader
{
    private readonly ModEntry _mod;
    private readonly Dictionary<string, Texture2D> _editedAssetReplacements = new();

    /// <summary>
    /// Tracking hue shifted NPCs so we can invalidate them all daily
    /// if we are hue shifting them daily
    /// 
    /// This prevents their images from being re-randomized by mistake
    /// </summary>
    private readonly HashSet<string> _hueShiftedNpcAssets = new();

    /// <summary>
    /// The asset names to invalidate when returning to title
    /// </summary>
    private readonly HashSet<string> _originalReplacedAssets = new();

    /// <summary>Constructor</summary>
    /// <param name="mod">A reference to the ModEntry</param>
    public AssetLoader(ModEntry mod)
    {
        _mod = mod;
    }

    /// <summary>
    /// When an asset is requested, execute the approriate patcher's code, or replace
    /// the value from our dictionary
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(RainPatcher.StardewAssetPath))
        {
            e.Edit(new RainPatcher().OnAssetRequested);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(AnimalIconPatcher.StardewAssetPath))
        {
            e.Edit(new AnimalIconPatcher().OnAssetRequested);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(CritterPatcher.StardewAssetPath))
        {
            e.Edit(new CritterPatcher().OnAssetRequested);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(TitleScreenPatcher.StardewAssetPath))
        {
            e.Edit(new TitleScreenPatcher().OnAssetRequested);
        }

        // Files that we have in memory: we're replacing an xnb asset with a Texture2D object
        else if (_editedAssetReplacements.TryGetValue(e.Name.BaseName, out Texture2D editedAsset))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsImage();
                editor.PatchImage(editedAsset);
            });
        }
    }

    /// <summary>
    /// Invalidate replaced assets so that the changes are reapplied
    /// Called when a save is loaded
    /// </summary>
    public void InvalidateSaveLoadCache()
        => ReplaceCatIcon();

    /// <summary>
    /// Invalidate replaced assets so that the changes are reapplied
    /// Called at the start of every day
    /// </summary>
    public void InvalidateDailyCache()
    {
        if (Globals.Config.RandomizeRain)
        {
            _mod.Helper.GameContent.InvalidateCache(RainPatcher.StardewAssetPath);
        }
        
        if (Globals.Config.Animals.RandomizeCritters)
        {
            _mod.Helper.GameContent.InvalidateCache(CritterPatcher.StardewAssetPath);
        }
    }

    /// <summary>
    /// Invalidate replaced assets so that the changes are reapplied
    /// 
    /// Called at the end of every day for assets that are modified when the save is first load, but then
    /// modified again every day after
    /// 
    /// We invalidate the npc caches so the actual game data is used when re-hue shifting
    /// This prevents NPCs from being shuffled again by mistake
    /// </summary>
    public void InvalidateDayEndDailyCache()
    {
        if (Globals.Config.Monsters.HueShiftMax > 0 &&
            Globals.Config.Monsters.RandomizeHueShiftDaily)
        {
            MonsterHueShifter.GetHueShiftedMonsterAssets().ForEach(monsterData =>
                AddReplacement(monsterData.StardewAssetPath, monsterData.MonsterImage));
        }

        if (Globals.Config.NPCs.SpriteHueShiftMax > 0 &&
            Globals.Config.NPCs.RandomizeHueShiftDaily)
        {
            _hueShiftedNpcAssets.ToList().ForEach(npcAsset =>
            {
                RemoveReplacement(npcAsset);
                _mod.Helper.GameContent.InvalidateCache(npcAsset);
            });
            NpcSkinManipulator.GetSwappedNpcAssets().ForEach(npcSwap =>
                AddReplacement(npcSwap.StardewAssetPath, npcSwap.NpcImage));
        }
    }

    /// <summary>
    /// Replace assets while on the title screen, includes returning back
    /// to the title screen
    /// </summary>
    public void ReplaceTitleScreenAssets()
    {
        _editedAssetReplacements.Clear();
        _hueShiftedNpcAssets.Clear();

        _mod.Helper.GameContent.InvalidateCache(TitleScreenPatcher.StardewAssetPath);
        ReplaceCatIcon();

        _originalReplacedAssets.ToList().ForEach(originalAsset =>
            _mod.Helper.GameContent.InvalidateCache(originalAsset));
        _originalReplacedAssets.Clear();
    }

    /// <summary>
    /// Replaces the cat icon on the new game and the pause menu if pets are randomized
    /// Otherwise, restore the icon
    /// </summary>
    private void ReplaceCatIcon()
        => _mod.Helper.GameContent.InvalidateCache(AnimalIconPatcher.StardewAssetPath);

    /// <summary>
    /// Randomizes the images - depending on what settings are on
    /// It's still important to build the images to make sure seeds are consistent
    /// 
    /// Note that the cache is invalidated already when the save file is loaded
    /// See ModEntry.CalculateAllReplacements
    /// </summary>
    public void RandomizeImages()
    {
        _editedAssetReplacements.Clear();
        _hueShiftedNpcAssets.Clear();

        CropGrowthImageBuilder cropGrowthImageBuilder = new();

        HandleImageReplacement(new WeaponImageBuilder());
        HandleImageReplacement(cropGrowthImageBuilder);
        HandleImageReplacement(new ObjectImageBuilder(cropGrowthImageBuilder.CropIdsToLinkingData));
        HandleImageReplacement(new BundleImageBuilder());

        Globals.SpoilerWrite("==== ANIMALS ====");
        if (Globals.Config.Animals.RandomizeHorses)
        {
            HandleImageReplacement(new AnimalRandomizer(AnimalTypes.Horses));
        }
        if (Globals.Config.Animals.RandomizePets)
        {
            HandleImageReplacement(new AnimalRandomizer(AnimalTypes.Pets));
        }
        Globals.SpoilerWrite("");

        MonsterHueShifter.GetHueShiftedMonsterAssets().ForEach(monsterData =>
            AddReplacement(monsterData.StardewAssetPath, monsterData.MonsterImage));

        NpcSkinManipulator.GetSwappedNpcAssets().ForEach(npcSwap =>
        {
            _hueShiftedNpcAssets.Add(npcSwap.StardewAssetPath);
            AddReplacement(npcSwap.StardewAssetPath, npcSwap.NpcImage);
        });
    }

    /// <summary>
    /// Adds the image builder's modified assets to the dictionary
    /// Replace the localized version - our cache invalidator will invalidate it and the base one
    /// </summary>
    /// <param name="imageBuilder">The image builder</param>
    private void HandleImageReplacement(ImageBuilder imageBuilder)
    {
        Dictionary<string, Texture2D> modifiedAssets = imageBuilder.GenerateModifiedAssets();
        foreach (KeyValuePair<string, Texture2D> assetData in modifiedAssets)
        {
            var assetName = assetData.Key;
            var texture = assetData.Value;

            AddReplacement(assetName, texture);
        }
    }

    /// <summary>
    /// Adds a replacement to our internal dictionary and invalidates the cache so it will be reloaded
    /// </summary>
    /// <param name="originalAsset">The original asset</param>
    /// <param name="replacementAsset">The asset to replace it with</param>
    private void AddReplacement(string originalAsset, Texture2D replacementAsset)
    {
        IAssetName normalizedAssetName = _mod.Helper.GameContent.ParseAssetName(originalAsset);
        _editedAssetReplacements[normalizedAssetName.BaseName] = replacementAsset;
        _originalReplacedAssets.Add(originalAsset);
        _mod.Helper.GameContent.InvalidateCache(originalAsset);
    }

    /// <summary>
    /// Removes a replacement to our internal dictionary and invalidates the cache so it will be reloaded
    /// </summary>
    /// <param name="originalAsset">The original asset</param>
    private void RemoveReplacement(string originalAsset)
    {
        IAssetName normalizedAssetName = _mod.Helper.GameContent.ParseAssetName(originalAsset);
        _editedAssetReplacements.Remove(normalizedAssetName.BaseName);
        _mod.Helper.GameContent.InvalidateCache(originalAsset);
    }
}