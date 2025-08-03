using HarmonyLib;
using StardewValley;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using SVCrop = StardewValley.Crop;
using SVSeason = StardewValley.Season;

namespace Randomizer;

public class FlowerSeedAdjustments
{
    /// <summary>
    /// This is the method to replace the existing Crop.getRandomFlowerSeedForThisSeason
    /// This will make the seed grow a flower of an actual appropriate type
    /// </summary>
    /// <param name="season">The relevant season</param>
    /// <returns>The ID of the random flower seed</returns>
    internal static string GetRandomFlowerSeedForThisSeason(SVSeason season)
    {
        Seasons randoSeason = SeasonsExtensions.ConvertFromSVSeason(season);
        if (randoSeason == Seasons.Winter)
        {
            randoSeason = Game1.random.Choose(
                Seasons.Spring, Seasons.Summer, Seasons.Fall);
        }

        List<string> flowerSeedIds = ItemList.GetFlowerSeeds(randoSeason)
            .Select(seed => seed.Id)
            .ToList();

        return RNG.GetRandomValueFromListUsingRNG(flowerSeedIds, Game1.random);
    }

    /// <summary>
    /// The prefix path for the flower crop replacement - this will replace the original function
    /// More info on this here: https://harmony.pardeike.net/articles/patching-prefix.html
    /// </summary>
    /// <param name="__instance">The SVCrop instance (unused)</param>
    /// <param name="season">The season passed to the original function</param>
    /// <param name="__result">The value that we want the function to return</param>
    /// <returns>Whether we should fall back to the original function's code</returns>
    [HarmonyPatch(typeof(SVCrop))]
    internal static bool GetRandomFlowerSeedForThisSeason_Prefix(
    SVCrop __instance,
    SVSeason season,
    ref string __result)
    {
        try
        {
            if (!Globals.Config.Crops.RandomizeStats)
            {
                return true;
            }

            __result = GetRandomFlowerSeedForThisSeason(season);
            return false;
        }
        catch (Exception ex)
        {
            Globals.ConsoleError($"Failed to grow a new flower in {nameof(GetRandomFlowerSeedForThisSeason_Prefix)}, growing the default instead.\n{ex}");
            return true;
        }
    }

    /// <summary>
    /// Replaces the Crop.getRandomFlowerSeedpForSeason method in Stardew Valley's Crop.cs 
    /// with this file's GetRandomFlowerSeedForSeason method
    /// 
    /// Note that harmony should only be used as a last resort, so we should consider
    /// moving away from it if it's ever possible
    /// </summary>
    public static void ReplaceGetRandomFlowerSeedForSeason()
    {
        var harmony = new Harmony(Globals.ModRef.ModManifest.UniqueID);
        harmony.Patch(
           original: AccessTools.Method(
            typeof(SVCrop),
            nameof(SVCrop.getRandomFlowerSeedForThisSeason),
            new Type[] { typeof(SVSeason) }),
           prefix: new HarmonyMethod(
           typeof(FlowerSeedAdjustments),
           nameof(GetRandomFlowerSeedForThisSeason_Prefix))
        );
    }
}
