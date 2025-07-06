using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Tools;
using System;

namespace Randomizer;

public class FishingRodAdjustments
{
    /// <summary>
    /// The prefix path for the trying to get the trout derby prefix
    /// Runs code to set the flag before running FishingRod.tickUpdate
    /// More info on this here: https://harmony.pardeike.net/articles/patching-prefix.html
    /// </summary>
    /// <param name="__instance">The FishingRod instance</param>
    /// <param name="time">The game time passed to the original function (unused)</param>
    /// <param name="who">The player passed to the original function</param>
    /// <returns>True, as we ALWAYS want to execute the original code</returns>
    [HarmonyPatch(typeof(FishingRod))]
    internal static bool TryGetTroutDerbyTag_Prefix(
        FishingRod __instance,
        GameTime time,
        Farmer who)
    {
        if (!Globals.Config.Fish.ShuffleSeasonsAndLocations)
        {
            return true;
        }

        try
        {
            // All checks that return in the original code
            if (__instance.isTimingCast ||
                __instance.isFishing ||
                __instance.isReeling ||
                who.UsingTool && __instance.castedButBobberStillInAir ||
                __instance.showingTreasure ||
                !__instance.fishCaught)
            {
                return true;
            }

            if (!who.IsLocalPlayer ||
                Game1.input.GetMouseState().LeftButton != ButtonState.Pressed &&
                !Game1.didPlayerJustClickAtAll() &&
                !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
            {
                return true;
            }

            // Only allow Golden Tags in these locations
            if (who.currentLocation is not StardewValley.Locations.Town &&
                who.currentLocation is not StardewValley.Locations.Forest &&
                who.currentLocation is not StardewValley.Locations.Mountain)
            {
                return true;
            }

            // Handles potential issues in ItemList.GetItemFromStringId for non-objects
            var caughtItemId = __instance.whichFish.QualifiedItemId;
            if (!caughtItemId.StartsWith("(O)"))
            {
                return true;
            }

            // Checks for the correct time and RNG
            var baseChance = Globals.Config.Fish.GoldenTagChance / 100d;
            if (!__instance.fromFishPond &&
                Game1.IsSummer &&
                Game1.dayOfMonth >= 20 &&
                Game1.dayOfMonth <= 21 &&
                Game1.random.NextDouble() < baseChance * __instance.numberOfFishCaught)
            {
                // Don't award a tag for a non-fish!
                var caughtItem = ItemList.GetItemFromStringId(caughtItemId);
                if (caughtItem != null && caughtItem.IsFish)
                {
                    __instance.gotTroutDerbyTag = true;
                }
            }
        }

        catch (Exception ex)
        {
            Globals.ConsoleError($"Failed when trying to get the trout derby tag in {nameof(TryGetTroutDerbyTag_Prefix)}.\n{ex}");
        }

        return true;
    }

    /// <summary>
    /// Prefixes FishingRod.tickUpdate method in Stardew Valley's FishingRod.cs 
    /// with this file's TryGetTroutDerbyTag_Prefix method
    /// 
    /// Note that harmony should only be used as a last resort, so we should consider
    /// moving away from it if it's ever possible
    /// </summary>
    public static void TryGetTroutDerbyTag()
    {
        var harmony = new Harmony(Globals.ModRef.ModManifest.UniqueID);
        harmony.Patch(
            original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.tickUpdate)),
            prefix: new HarmonyMethod(typeof(FishingRodAdjustments), nameof(TryGetTroutDerbyTag_Prefix))
        );
    }
}
