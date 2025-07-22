using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Machines;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SVItem = StardewValley.Item;
using SVObject = StardewValley.Object;

namespace Randomizer;

public class MachineRandomizer
{
    /// <summary>
    /// The unique ID to use for the recycling machine output data
    /// </summary>
    private static readonly string UniqueRecyclingMachineOutputId = 
        $"{Globals.ModRef.ModManifest.UniqueID}_Random_Replacement";

    /// <summary>
    /// Randomizes all machines and returns the replacements to make
    /// </summary>
    /// <returns>The replacements</returns>
    public static Dictionary<string, MachineData> RandomizeMachines()
    {
        Dictionary<string, MachineData> replacements = new();

        RandomizeRecyclingMachines(replacements);

        return replacements;
    }

    /// <summary>
    /// Randomizes recycling machines by adding a new rule that accepts any (O) item as input
    /// and outputs an item of similar cost (defined in the settings)
    /// </summary>
    /// <param name="replacements">The Data/Machines replacements to make</param>
    private static void RandomizeRecyclingMachines(
        Dictionary<string, MachineData> replacements)
    {
        if (!Globals.Config.RecyclingMachine.Randomize) { return; }

        var recyclingMachineId = BigCraftableIndexes.RecyclingMachine.GetItem().QualifiedId;
        var recyclingMachineData = DataLoader
            .Machines(Game1.content)?[recyclingMachineId];
        if (recyclingMachineData == default)
        {
            Globals.ConsoleError("Could not find Recycling Machine data to modify.");
            return;
        }

        var newRecyclingMachineData = recyclingMachineData.DeepClone();
        newRecyclingMachineData.OutputRules.Add(
            new()
            {
                Id = UniqueRecyclingMachineOutputId,
                MinutesUntilReady = 60,
                UseFirstValidOutput = true,
                Triggers = new()
                {
                    new()
                    {
                        Id = "ItemPlacedInMachine",
                        Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                        RequiredCount = 1,
                        Condition = "ITEM_TYPE Input (O)"
                    }
                },
                OutputItem = new()
                {
                    new()
                    {
                        Id = UniqueRecyclingMachineOutputId,
                        OutputMethod = 
                            $"{typeof(MachineRandomizer).AssemblyQualifiedName}: {nameof(MachineOutputMethod)}"
                    }
                }
            }
        );

        replacements.Add(recyclingMachineId, newRecyclingMachineData);
    }

    /// <summary>
    /// Must match the delegate: StardewValley.Delegates.MachineOutputDelegate
    ///
    /// Changes the Recycling machine's output by returning an item within the
    /// defined price ranges of the input
    /// - Creates a list of items between the upper and lower range
    /// - If there are no items in the list, return back the input item
    /// </summary>
    /// <param name="machine">(Unused) The machine to output from</param>
    /// <param name="inputItem">The item put into the machine</param>
    /// <param name="probe">(Unused) Whether we're only checking the output, and not actually producing it</param>
    /// <param name="outputData">(Unused) The Data/Machines output data</param>
    /// <param name="player">(Unused) The interacting player</param>
    /// <param name="overrideMinutesUntilReady">The time until the item should be ready for harvest, in minutes</param>
    /// <returns>The item to produce</returns>
    public static SVItem MachineOutputMethod(
        SVObject machine,
        SVItem inputItem,
        bool probe,
        MachineItemOutput outputData,
        Farmer player,
        out int? overrideMinutesUntilReady) 
    {
        int price = inputItem.sellToStorePrice();
        Range salePriceRange = GetPriceRange(price);

        var itemsWithinRange = Game1.objectData
            .Where(kv => kv.Key != inputItem.ItemId &&
                kv.Key != ObjectIndexes.Stardrop.GetId() &&
                ItemRegistry.QualifyItemId(kv.Key).StartsWith("(O)") &&
                ItemRegistry.Create(kv.Key) is SVObject &&
                salePriceRange.Contains(kv.Value.Price))
            .ToList();

        // If there's no valid items, give back the input
        // Else grab a random item in the range
        string newItemId = itemsWithinRange.Count == 0
            ? inputItem.ItemId
            : RNG.GetRandomValueFromListUsingRNG(
                itemsWithinRange, Game1.random).Key;
        
        if (string.IsNullOrWhiteSpace(newItemId))
        {
            newItemId = inputItem.ItemId;
            Globals.ConsoleError("Attempted to give non-existent item in the Recycling Machine! Giving back the input.");
        }

        var newItem = ItemRegistry.Create(newItemId);
        overrideMinutesUntilReady = GetMinutesUntilReady(price, newItem.sellToStorePrice());

        return newItem;
    }

    /// <summary>
    /// Gets the range, given the price and percentages above/below the range to use
    /// For example, base of 100, and below/above percentages at 10 gives a range from 90-110
    /// 
    /// If the percent chance is rolled for a better item, will return the
    /// base price + 1 as the lower price, and the high price as the max value
    /// </summary>
    /// <param name="basePrice">The base price</param>
    /// <returns>The computed price range</returns>
    private static Range GetPriceRange(int basePrice)
    {
        const int highestPrice = 1_000_000; // Nothing is this expensive, max int value causes issues

        var rng = new RNG(); // No need for this to be seeded
        if (rng.NextBoolean(Globals.Config.RecyclingMachine.PercentAnyRandomItem))
        {
            var message = HUDMessage.ForItemGained(
                ItemRegistry.Create(
                    BigCraftableIndexes.RecyclingMachine.GetItem().QualifiedId), 
                    count: 0);
            message.message = Globals.GetTranslation("recycling-machine-good-roll");
            Game1.addHUDMessage(message);

            return new(basePrice + 1, highestPrice);
        }

        int percentBelowPrice = Globals.Config.RecyclingMachine.PercentBelowPrice;
        int percentAbovePrice = Globals.Config.RecyclingMachine.PercentAbovePrice;

        var lowPrice = percentBelowPrice < 0
            ? 0
            : basePrice - basePrice * (percentBelowPrice / 100f);
        var highPrice = percentAbovePrice < 0
            ? highestPrice
            : basePrice + basePrice * (percentAbovePrice / 100f);

        return new((int)lowPrice, (int)highPrice);
    }

    /// <summary>
    /// Gets the minutes until the Recycling Machine output will be ready
    /// - If getting back a cheaper item, default to one hour
    /// - If the output costs the max price for default processing time or less, 
    ///   default to one hour
    /// - Else increase 10 minutes per PercentTimeIncreaseInterval% more costly the output is
    /// - Capped at one day
    /// </summary>
    /// <param name="inputPrice">The price of the item put into the machine</param>
    /// <param name="outputPrice">The price of the item to get out of the machine</param>
    /// <returns>The computed time</returns>
    private static int GetMinutesUntilReady(int inputPrice, int outputPrice)
    {
        const int minutesInOneHour = 60;

        int difference =  outputPrice - inputPrice;
        if (difference < 0 || 
            outputPrice <= Globals.Config.RecyclingMachine.MaxPriceForDefaultProcessingTime) {
            return minutesInOneHour;
        }

        // Increases the time by 10 minutes for each x% the output is more than the input
        // For example: Input = 100, Output = 200
        // - This is a 100% total increase:
        //   If this value is 5, that's 100 / 5 = 20 intervals it increased by
        //   so the time should increase by 20 * 10 minutes
        int percentIntervalForIncreases = Globals.Config.RecyclingMachine.PercentTimeIncreaseInterval;
        const int minutesInOneDay = 20 * minutesInOneHour;

        int percentIncrease = (int)((double)difference / inputPrice * 100);
        int minutesToIncreaseBy = (int)Math.Ceiling((double)percentIncrease / percentIntervalForIncreases) * 10;
        int minutesUntilReady = Math.Min(minutesToIncreaseBy + minutesInOneHour, minutesInOneDay);

        return minutesUntilReady;
    }
}
