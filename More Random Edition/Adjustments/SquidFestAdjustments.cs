using HarmonyLib;
using StardewValley;
using StardewValley.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using SVCrop = StardewValley.Crop;
using SVSeason = StardewValley.Season;

namespace Randomizer;

public class SquidFestAdjustments
{
    /// <summary>
    /// This is the method to repalce the existing Farmer.caughtFish
    /// This will make the game work for the new squid fish duing SquidFest
    /// </summary>
    /// <param name="itemId">The fish that was caught</param>
    /// <param name="numberCaught">The number of fish that were caught</param>
    internal static void CaughtFish(
        string itemId,
        int numberCaught)
    {
        if (Utility.GetDayOfPassiveFestival("SquidFest") > 0 && 
            itemId == FishRandomizer.OldSquidId)
        {
            Game1.stats.Increment(
                StatKeys.SquidFestScore(Game1.dayOfMonth, Game1.year),
                numberCaught);
        }
    }

    /// <summary>
    /// The prefix path for the squid fest fix - this will replace the original function
    /// More info on this here: https://harmony.pardeike.net/articles/patching-prefix.html
    /// </summary>
    /// <param name="__instance">The SVCrop instance (unused)</param>
    /// <param name="itemId"></param>
    /// <param name="size"></param>
    /// <param name="from_fish_pond"></param>
    /// <param name="numberCaught"></param>
    /// <param name="__result">The value that we want the function to return</param>
    /// <returns>Whether we should fall back to the original function's code</returns>
    [HarmonyPatch(typeof(SVCrop))]
    internal static bool CaughtFish_Prefix(
        SVCrop __instance,
        string itemId,
        int size,
        bool from_fish_pond,
        int numberCaught)
    {
        try
        {
            if (!Globals.Config.Fish.ShuffleSeasonsAndLocations)
            {
                return true;
            }

            CaughtFish(itemId, numberCaught);
            return true;
        }
        catch (Exception ex)
        {
            Globals.ConsoleError($"Failed to fix the SquidFest counter in {nameof(CaughtFish_Prefix)}.\n{ex}");
            return true;
        }
    }

    /// <summary>
    /// Replaces the Farmer.caughtFish method in Stardew Valley's Farmer.cs 
    /// with this file's CaughtFish method
    /// 
    /// Note that harmony should only be used as a last resort, so we should consider
    /// moving away from it if it's ever possible
    /// </summary>
    public static void FixSquidFestFish()
    {
        var harmony = new Harmony(Globals.ModRef.ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(
                typeof(Farmer),
                nameof(Farmer.caughtFish),
                new Type[] { 
                    typeof(string),
                    typeof(int),
                    typeof(bool),
                    typeof(int)
                }),
                prefix: new HarmonyMethod(
                typeof(SquidFestAdjustments),
                nameof(CaughtFish_Prefix))
        );
    }
}
