using HarmonyLib;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer;

/// <summary>
/// Randomizes the mail rewards that gets sent with rewards that are similar
/// </summary>
public class MailRewardRandomizer
{
    /// <summary>
    /// The index of the mail reward in the mail data
    /// This is the entry that is delimited by percent signs (%)
    /// </summary>
    private const int MailRewardIndex = 1;

    /// <summary>
    /// The RNG for this class
    /// </summary>
    private static RNG Rng { get; set; }

    /// <summary>
    /// A single mail reward chance, containg the reward
    /// and how much of it the player would receive
    /// </summary>
    private class MailItemReward
    {
        /// <summary>
        /// The item in the reward
        /// </summary>
        public Item ItemReward { get; set; }

        /// <summary>
        /// The number of items to receive
        /// </summary>
        public int Amount { get; set; }

        public MailItemReward(Item item)
        {
            ItemReward = item;
            Amount = 1;
        }

        public MailItemReward(Item item, Range itemAmountRange)
        {
            ItemReward = item;
            Amount = itemAmountRange.GetRandomValue(Rng);
        }
    }

    /// <summary>
    /// A class to keep track of the possible mail rewards
    /// </summary>
    private class MailReward
    {
        /// <summary>
        /// The maximum number of random items that can be in the reward pool
        /// </summary>
        private const int MaxRandomItemAmount = 25;

        /// <summary>
        /// The list of possible mail rewards
        /// </summary>
        public List<MailItemReward> ItemRewardList {  get; set; }

        /// <summary>
        /// The amount of money in this reward
        /// If this has a value, then it is assumed that
        /// the reward is money, and not an item
        /// </summary>
        public int MoneyAmount { get; set; }

        /// <summary>
        /// Gets whether this is a money reward
        /// </summary>
        public bool IsMoneyReward
        {
            get => MoneyAmount > 0;
        }

        public MailReward(List<MailItemReward> itemRewardList)
        {
            ItemRewardList = Rng.GetRandomValuesFromList(
                itemRewardList, numberOfvalues: MaxRandomItemAmount);
        }

        public MailReward(Range moneyAmountRange)
        {
            MoneyAmount = moneyAmountRange.GetRandomValue(Rng);
        }

        /// <summary>
        /// Gets a random mail reward string from a list of possible items
        /// </summary>
        /// <param name="items">The list of items</param>
        /// <returns>A randomly chosen reward string</returns>
        public static string GetItemRewardString(List<MailReward> items)
            => Rng.GetRandomValueFromList(items).ToString();

        /// <summary>
        /// The sting format to use in Data/mail
        /// Note that the space at the end is intentional
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString()
        {
            if (IsMoneyReward)
            {
                return $"item money {MoneyAmount} ";
            }

            var rewardStrings = ItemRewardList
                .Select(item => $"{item.ItemReward.QualifiedId} {item.Amount}")
                .ToList();

            return $"item id {string.Join(" ", rewardStrings)}";
        }

        /// <summary>
        /// The string to use in the spoiler log
        /// </summary>
        /// <returns></returns>
        public string GetSpoilerLogString()
        {
            if (IsMoneyReward)
            {
                return $"{MoneyAmount}g";
            }

            var displayString = ItemRewardList
                .Select(item => $"- {item.Amount} x {item.ItemReward.DisplayName}")
                .ToList();

            return $"\n{string.Join("\n", displayString)}";
        }
    }

    /// <summary>
    /// An Enum of the different mail types, so we can map each letter
    /// to a randomized reward of a similar item
    /// </summary>
    private enum MailRewardTypes
    {
        SmallMoney,
        MediumMoney,
        LargeMoney,

        AnimalItems,
        CaveItems,
        CheapFarmItems,
        CheapResources,
        Crop,
        DesertItems,
        Food,
        FieldStudyItems,
        FishOrFishFood,
        RandomLargeTimeOrBelow,
    }

    /// <summary>
    /// The map of each relevant letter's key to what randomized reward type
    /// the player should receive
    /// </summary>
    private static readonly Dictionary<string, MailRewardTypes> _mailRewardMap = new()
    {
        { "mom1", MailRewardTypes.Food },
        { "mom2", MailRewardTypes.SmallMoney },
        { "mom4", MailRewardTypes.Food },

        { "dad1", MailRewardTypes.SmallMoney },
        { "dad2", MailRewardTypes.SmallMoney },
        { "dad4", MailRewardTypes.CheapResources },

        { "QiChallengeComplete", MailRewardTypes.LargeMoney },
        { "quest10", MailRewardTypes.SmallMoney },
        { "quest35", MailRewardTypes.MediumMoney },

        { "lewisStatue", MailRewardTypes.SmallMoney },
        { "ClintReward2", MailRewardTypes.CaveItems },

        { "Caroline", MailRewardTypes.Crop },
        { "Clint", MailRewardTypes.CaveItems },
        { "Demetrius", MailRewardTypes.FieldStudyItems },
        { "Emily", MailRewardTypes.RandomLargeTimeOrBelow },
        { "Evelyn", MailRewardTypes.Food },
        { "George", MailRewardTypes.CheapResources },
        { "Gus", MailRewardTypes.Food },
        { "Jodi", MailRewardTypes.CheapFarmItems },
        { "Kent", MailRewardTypes.RandomLargeTimeOrBelow },
        { "Lewis", MailRewardTypes.SmallMoney },
        { "Linus", MailRewardTypes.FishOrFishFood },
        { "Marnie", MailRewardTypes.AnimalItems },
        { "Pam", MailRewardTypes.RandomLargeTimeOrBelow },
        { "Pierre", MailRewardTypes.SmallMoney },
        { "Robin", MailRewardTypes.CheapResources },
        { "Sandy", MailRewardTypes.DesertItems },
        { "Shane", MailRewardTypes.Food },
        { "Wizard", MailRewardTypes.RandomLargeTimeOrBelow }
    };

    /// <summary>
    /// Randomizes the mail rewards with items that are similar
    /// </summary>
    /// <param name="mailReplacements">The current running list of mail replacements</param>
    public static void Randomize(
        Dictionary<string, string> mailReplacements)
    {
        if (!Globals.Config.RandomizeMailRewards) { return; }

        Rng = RNG.GetFarmRNG(nameof(MailRewardRandomizer));
        RandomizeMailAndAddToReplacements(mailReplacements);
    }

    /// <summary>
    /// Randomizes the mail rewards and adds the entry to the dictionary
    /// If the entry already exists, modifies the existing one instead
    /// - Includes a warning message, as this is unexpected
    /// </summary>
    /// <param name="mailReplacements">The current running list of mail replacements</param>
    public static void RandomizeMailAndAddToReplacements(
        Dictionary<string, string> mailReplacements)
    {
        Globals.SpoilerWrite("==== MAIL REWARDS ====");

        var mailData = DataLoader.Mail(Game1.content);
        foreach (var mailRewardMapKV in _mailRewardMap)
        {
            var mailId = mailRewardMapKV.Key;
            var rewardType = mailRewardMapKV.Value;

            if (!mailData.ContainsKey(mailId))
            {
                Globals.ConsoleError($"Could not retrieve mail id {mailId} from dictionary.");
                continue;
            }

            var mailString = mailData[mailId];

            if (mailReplacements.ContainsKey(mailId))
            {
                Globals.ConsoleWarn($"Mail id {mailId} already modified - using the modified version for random mail rewards.");
                mailString = mailReplacements[mailId];
                mailReplacements.Remove(mailId);
            }

            var mailTokens = mailString.Split("%");
            var newMailReward = GetNewMailReward(rewardType);
            mailTokens[MailRewardIndex] = newMailReward.ToString();
            mailReplacements.Add(mailId, string.Join("%", mailTokens));

            Globals.SpoilerWrite($"{mailId}: {newMailReward.GetSpoilerLogString()}");
        }

        Globals.SpoilerWrite("");
    }
    
    /// <summary>
    /// Gets a new mail reward for the given reward type
    /// </summary>
    /// <param name="rewardType">The reward type</param>
    /// <returns>The generated mail reward</returns>
    private static MailReward GetNewMailReward(MailRewardTypes rewardType)
    {
        switch (rewardType)
        {
            case MailRewardTypes.SmallMoney:
                return new(new Range(300, 1000));
            case MailRewardTypes.MediumMoney:
                return new(new Range(2000, 5000));
            case MailRewardTypes.LargeMoney:
                return new(new Range(8000, 12000));
            case MailRewardTypes.AnimalItems:
                return new(
                    ItemList.GetAnimalProducts()
                        .Select(item => new MailItemReward(item, new Range(3, 10)))
                        .Concat(new List<MailItemReward>
                        {
                            new(ObjectIndexes.Hay.GetItem(), new Range(25, 75)),
                            new(BigCraftableIndexes.Heater.GetItem()),
                            new(BigCraftableIndexes.AutoGrabber.GetItem()),
                            new(BigCraftableIndexes.AutoPetter.GetItem()),
                            new(BigCraftableIndexes.MayonnaiseMachine.GetItem()),
                            new(BigCraftableIndexes.CheesePress.GetItem()),
                            new(BigCraftableIndexes.OstrichIncubator.GetItem())
                        })
                    .ToList());
            case MailRewardTypes.CaveItems:
                return new(
                    ItemList.GetGeodeMinerals()
                        .Concat(ItemList.GetSmeltedItems())
                        .Select(item => new MailItemReward(item, new Range(1, 5)))
                        .ToList());
            case MailRewardTypes.CheapFarmItems:
                return new(
                    new List<MailItemReward> {
                        new(ObjectIndexes.BasicFertilizer.GetItem(), new Range(5, 20)),
                        new(ObjectIndexes.QualityFertilizer.GetItem(), new Range(5, 20)),
                        new(ObjectIndexes.BasicRetainingSoil.GetItem(), new Range(5, 20)),
                        new(ObjectIndexes.QualityRetainingSoil.GetItem(), new Range(5, 20)),
                        new(ObjectIndexes.Sprinkler.GetItem(), new Range(2, 5)),
                        new(ObjectIndexes.QualitySprinkler.GetItem(), new Range(1, 3)),
                        new(BigCraftableIndexes.Scarecrow.GetItem())
                    });
            case MailRewardTypes.CheapResources:
                return new(
                    ItemList.GetResources()
                        .Select(item => new MailItemReward(item, new Range(25, 75)))
                        .ToList());
            case MailRewardTypes.Crop:
                return new(
                    ItemList.GetCrops()
                        .Select(item => new MailItemReward(item))
                        .ToList());
            case MailRewardTypes.DesertItems:
                return new(
                    ItemList.GetUniqueDesertForagables()
                        .Concat(FishItem.Get(Locations.Desert))
                        .AddItem(ObjectIndexes.CactusFruit.GetItem())
                        .Select(item => new MailItemReward(item, new Range(3, 5)))
                        .ToList());
            case MailRewardTypes.Food:
                return new(
                    ItemList.GetCookedItems()
                        .Select(item => new MailItemReward(item))
                        .ToList());
            case MailRewardTypes.FieldStudyItems:
                return new(
                    ItemList.GetArtifacts()
                        .Concat(FishItem.Get())
                        .Select(item => new MailItemReward(item))
                        .ToList());
            case MailRewardTypes.FishOrFishFood:
                return new(
                    ItemList.GetCookedItems()
                        .Cast<CookedItem>()
                        .Where(item => item.IsFishDish)
                        .Concat(FishItem.Get())
                        .Select(item => new MailItemReward(item))
                        .ToList());
            case MailRewardTypes.RandomLargeTimeOrBelow:
                return new(
                    ItemList.GetItemsBelowDifficulty(ObtainingDifficulties.UncommonItem)
                        .Select(item => new MailItemReward(item))
                        .ToList());
            default:
                Globals.ConsoleError($"Tried to get mail reward for unknown type {rewardType}. Returning 500g instead.");
                return new(new Range(500, 500)); 
        }
    }
}
