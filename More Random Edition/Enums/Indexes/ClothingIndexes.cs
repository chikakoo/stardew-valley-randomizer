﻿using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// An enum representing all the clothing
	/// </summary>
	public enum ClothingIndexes
    {
        FarmerPants = 0,
        Shorts = 1,
        Dress = 2,
        Skirt = 3,
        PleatedSkirt = 4,
        DinosaurPants = 5,
        GrassSkirt = 6,
        LuauSkirt = 7,
        GeniePants = 8,
        TightPants = 9,
        BaggyPants = 10,
        SimpleDress = 11,
        RelaxedFitPants = 12,
        RelaxedFitShorts = 13,
        PolkaDotShorts = 14,
        TrimmedLuckyPurpleShorts = 15,
        PrismaticPants = 998,
        PrismaticGeniePants = 999,
        ClassicOveralls = 1000,
        MintBlouse = 1002,
        DarkShirt = 1003,
        SkullShirt = 1004,
        LightBlueShirt = 1005,
        TanStripedShirt = 1006,
        GreenOveralls = 1007,
        GoodGriefShirt = 1008,
        AquamarineShirt = 1009,
        SuitTop = 1010,
        GreenBeltedShirt1 = 1011,
        LimeGreenStripedShirt = 1012,
        RedStripedShirt = 1013,
        SkeletonShirt = 1014,
        OrangeShirt = 1015,
        NightSkyShirt = 1016,
        MayoralSuspenders = 1017,
        BrownJacket = 1018,
        SailorShirt = 1019,
        GreenVest = 1020,
        YellowandGreenShirt = 1021,
        LightBlueStripedShirt = 1026,
        PinkStripedShirt = 1027,
        HeartShirt1 = 1028,
        WorkShirt = 1029,
        StoreOwnersJacket = 1030,
        GreenTunic = 1034,
        FancyRedBlouse = 1035,
        PlainShirt = 1038,
        RetroRainbowShirt = 1039,
        LimeGreenTunic = 1042,
        WhiteOverallsShirt = 1071,
        NeatBowShirt1 = 1087,
        ShirtAndTie = 1123,
        EmilysMagicShirt = 1127,
        StripedShirt = 1128,
        TankTop = 1129,
        CowboyPoncho = 1131,
        CropTankTop = 1132,
        BikiniTop = 1134,
        WumbusShirt = 1135,
        EightiesShirt = 1136,
        LettermanJacket = 1137,
        BlackLeatherJacket = 1138,
        StrappedTop = 1139,
        ButtonDownShirt = 1140,
        CropTopShirt = 1141,
        TubeTop = 1142,
        TyeDieShirt = 1143,
        SteelBreastplate = 1148,
        CopperBreastplate = 1149,
        GoldBreastplate = 1150,
        IridiumBreastplate = 1151,
        FakeMusclesShirt = 1153,
        FlannelShirt = 1154,
        BomberJacket = 1155,
        CavemanShirt = 1156,
        FishingVest = 1157,
        FishShirt = 1158,
        ShirtAndBelt = 1159,
        GrayHoodie = 1160,
        BlueHoodie = 1161,
        RedHoodie = 1162,
        DenimJacket = 1163,
        TrackJacket = 1164,
        WhiteGi = 1165,
        OrangeGi = 1166,
        GrayVest = 1167,
        KelpShirt = 1168,
        StuddedVest = 1169,
        GaudyShirt = 1170,
        OasisGown = 1171,
        BlacksmithApron = 1172,
        NeatBowShirt2 = 1173,
        HighWaistedShirt1 = 1174,
        HighWaistedShirt2 = 1175,
        BasicPullover = 1176,
        TurtleneckSweater = 1178,
        IridiumEnergyShirt = 1179,
        TunnelersJersey = 1180,
        GraySuit = 1183,
        RedTuxedo = 1184,
        NavyTuxedo = 1185,
        HolidayShirt = 1186,
        LeafyTop = 1187,
        GoodnightShirt = 1188,
        GreenBeltedShirt2 = 1189,
        HappyShirt = 1190,
        ShirtwithBow = 1191,
        JesterShirt = 1192,
        OceanShirt = 1193,
        DarkStripedShirt = 1194,
        BandanaShirt = 1195,
        BackpackShirt = 1196,
        PurpleBlouse = 1197,
        VintagePolo = 1198,
        TogaShirt = 1199,
        StarShirt = 1200,
        ClassyTop = 1201,
        BandanaShirt1 = 1203,
        VacationShirt = 1204,
        GreenThumbShirt = 1205,
        BandanaShirt2 = 1206,
        SlimeShirt = 1207,
        ExcavatorShirt = 1208,
        SportsShirt = 1209,
        HeartShirt2 = 1210,
        DarkJacket = 1211,
        SunsetShirt = 1212,
        ChefCoat = 1213,
        ShirtOTheSea = 1214,
        ArcaneShirt = 1215,
        PlainOveralls = 1216,
        SleevelessOveralls = 1217,
        Cardigan = 1218,
        YobaShirt = 1219,
        NecklaceShirt = 1220,
        BeltedCoat = 1221,
        GoldTrimmedShirt = 1222,
        PrismaticShirt = 1223,
        PendantShirt = 1224,
        HighHeatShirt = 1225,
        FlamesShirt = 1226,
        AntiquityShirt = 1227,
        SoftArrowShirt = 1228,
        DollShirt = 1229,
        JewelryShirt = 1230,
        CanvasJacket = 1231,
        TrashCanShirt = 1232,
        RustyShirt = 1233,
        CircuitboardShirt = 1234,
        FluffyShirt = 1235,
        SauceStainedShirt = 1236,
        BrownSuit = 1237,
        GoldenShirt = 1238,
        CaptainsUniform = 1239,
        OfficerUniform = 1240,
        RangerUniform = 1241,
        BlueLongVest = 1242,
        RegalMantle = 1243,
        RelicShirt = 1244,
        BoboShirt = 1245,
        FriedEggShirt = 1246,
        BurgerShirt = 1247,
        CollaredShirt = 1248,
        ToastedShirt = 1249,
        CarpShirt = 1250,
        RedFlannelShirt = 1251,
        TortillaShirt = 1252,
        WarmFlannelShirt = 1253,
        SugarShirt = 1254,
        GreenFlannelShirt = 1255,
        OilStainedShirt = 1256,
        MorelShirt = 1257,
        SpringShirt = 1258,
        SailorShirt1 = 1259,
        RainCoat = 1260,
        SailorShirt2 = 1261,
        DarkBandanaShirt = 1262,
        DarkHighlightShirt = 1263,
        OmniShirt = 1264,
        BridalShirt = 1265,
        BrownOveralls = 1266,
        OrangeBowShirt = 1267,
        WhiteOveralls = 1268,
        PourOverShirt = 1269,
        GreenJacketShirt = 1270,
        ShortJacket = 1271,
        PolkaDotShirt = 1272,
        WhiteDotShirt = 1273,
        CamoShirt = 1274,
        DirtShirt = 1275,
        CrabCakeShirt = 1276,
        SilkyShirt = 1277,
        BlueButtonedVest = 1278,
        FadedDenimShirt = 1279,
        RedButtonedVest = 1280,
        GreenButtonedVest = 1281,
        TomatoShirt = 1282,
        FringedVest = 1283,
        GlobbyShirt = 1284,
        MidnightDogJacket = 1285,
        ShrimpEnthusiastShirt = 1286,
        TeaShirt = 1287,
        TrinketShirt = 1288,
        DarknessSuit = 1289,
        MineralDogJacket = 1290,
        MagentaShirt = 1291,
        GingerOveralls = 1292,
        BananaShirt = 1293,
        YellowSuit = 1294,
        HotPinkShirt = 1295,
        TropicalSunriseShirt = 1296,
        IslandBikini = 1297,
        MagicSprinkleShirt = 1997,
        PrismaticShirt1 = 1998,
        PrismaticShirt2 = 1999
    }

    public class ClothingFunctions
    {
        /// <summary>
        /// Gets the Stardew clothing item from the given index
        /// </summary>
        /// <param name="index">The clothing item's index</param>
        /// <returns />
        public static Clothing GetItem(ClothingIndexes index)
        {
            return new Clothing(((int)index).ToString());
        }

        /// <summary>
        /// Gets the qualified id for the given clothing index
        /// </summary>
        /// <param name="index">The index of the clothing item</param>
        public static string GetQualifiedId(ClothingIndexes index)
        {
            return GetItem(index).QualifiedItemId;
        }

        /// <summary>
        /// Gets a random clothing item's qualified id
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static string GetRandomClothingQualifiedId(RNG rng, List<string> idsToExclude = null)
        {
            return GetRandomClothingQualifiedIds(rng, numberToGet: 1, idsToExclude)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of random clothing item qualified ids
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <param name="numberToGet">The number of ids to get</param>
        /// <param name="idsToExclude">A list of ids to not include in the selection</param>
        /// <returns>The qualified id</returns>
        public static List<string> GetRandomClothingQualifiedIds(
            RNG rng,
            int numberToGet,
            List<string> idsToExclude = null)
        {
            var allClothingIds = Enum.GetValues(typeof(ClothingIndexes))
                .Cast<ClothingIndexes>()
                .Select(index => GetQualifiedId(index))
                .Where(id => idsToExclude == null || !idsToExclude.Contains(id))
                .ToList();

            return rng.GetRandomValuesFromList(allClothingIds, numberToGet);
        }
    }
}
