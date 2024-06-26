﻿using System.Collections.Generic;
using System;
using System.Linq;

namespace Randomizer
{
    public enum HatIndexes
    {
        CowboyHat = 0,
        BowlerHat = 1,
        TopHat = 2,
        Sombrero = 3,
        StrawHat = 4,
        OfficialCap = 5,
        BlueBonnet = 6,
        PlumChapeau = 7,
        SkeletonMask = 8,
        GoblinMask = 9,
        ChickenMask = 10,
        Earmuffs = 11,
        DelicateBow = 12,
        Tropiclip = 13,
        ButterflyBow = 14,
        HuntersCap = 15,
        TruckerHat = 16,
        SailorsCap = 17,
        GoodOlCap = 18,
        Fedora = 19,
        CoolCap = 20,
        LuckyBow = 21,
        PolkaBow = 22,
        GnomesCap = 23,
        EyePatch = 24,
        SantaHat = 25,
        Tiara = 26,
        HardHat = 27,
        Souwester = 28,
        Daisy = 29,
        WatermelonBand = 30,
        MouseEars = 31,
        CatEars = 32,
        CowgalHat = 33,
        CowpokeHat = 34,
        ArchersCap = 35,
        PandaHat = 36,
        BlueCowboyHat = 37,
        RedCowboyHat = 38,
        ConeHat = 39,
        LivingHat = 40,
        EmilysMagicHat = 41,
        MushroomCap = 42,
        DinosaurHat = 43,
        TotemMask = 44,
        LogoCap = 45,
        WearableDwarfHelm = 46,
        FashionHat = 47,
        PumpkinMask = 48,
        HairBone = 49,
        KnightsHelmet = 50,
        SquiresHelmet = 51,
        SpottedHeadscarf = 52,
        Beanie = 53,
        FloppyBeanie = 54,
        FishingHat = 55,
        BlobfishMask = 56,
        PartyHat1 = 57,
        PartyHat2 = 58,
        PartyHat3 = 59,
        ArcaneHat = 60,
        ChefHat = 61,
        PirateHat = 62,
        FlatToppedHat = 63,
        ElegantTurban = 64,
        WhiteTurban = 65,
        GarbageHat = 66,
        GoldenMask = 67,
        PropellerHat = 68,
        BridalVeil = 69,
        WitchHat = 70,
        CopperPan = 71,
        GreenTurban = 72,
        MagicCowboyHat = 73,
        MagicTurban = 74,
        GoldenHelmet = 75,
        DeluxePirateHat = 76,
        PinkBow = 77,
        FrogHat = 78,
        SmallCap = 79,
        BluebirdMask = 80,
        DeluxeCowboyHat = 81,
        MrQisHat = 82,
        DarkCowboyHat = 83,
        RadioactiveGoggles = 84,
        SwashbucklerHat = 85,
        QiMask = 86,
        StarHelmet = 87,
        Sunglasses = 88,
        Goggles = 89,
        ForagersHat = 90,
        TigerHat = 91,
        QuestionMark = 92,
        WarriorHelmet = 93,
    }

    public class HatFunctions
    {
        public const string HatIdPrefix = "(H)";

        /// <summary>
        /// Gets the hat id in the form that Stardew references them
        /// </summary>
        /// <param name="hat">The hat index</param>
        /// <returns>The integer hat id, as a string</returns>
        public static string GetHatId(HatIndexes hat)
        {
            return ((int)hat).ToString();
        }

        /// <summary>
        /// Gets the qualified id for the given hat index
        /// </summary>
        /// <param name="index">The index of the hat</param>
        public static string GetQualifiedId(HatIndexes index)
        {
            return $"{HatIdPrefix}{(int)index}";
        }

        /// <summary>
        /// Gets a random hat's qualified id
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static string GetRandomHatQualifiedId(RNG rng, List<string> idsToExclude = null)
        {
            return GetRandomHatQualifiedIds(rng, numberToGet: 1, idsToExclude)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of random hat qualified ids
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="numberToGet">The number of ids to get</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static List<string> GetRandomHatQualifiedIds(
            RNG rng,
            int numberToGet,
            List<string> idsToExclude = null)
        {
            var allHatIds = Enum.GetValues(typeof(HatIndexes))
                .Cast<HatIndexes>()
                .Select(index => GetQualifiedId(index))
                .Where(id => idsToExclude == null || !idsToExclude.Contains(id))
                .ToList();

            return rng.GetRandomValuesFromList(allHatIds, numberToGet);
        }
    }
}
