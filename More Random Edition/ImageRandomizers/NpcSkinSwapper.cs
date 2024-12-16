using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Randomizer;

public class NpcSkinSwapper
{
    private static RNG Rng { get; set; }

    /// <summary>
    /// The path to the randomized images directory
    /// This is only used if the setting is on to save the images
    /// </summary>
    private static string RandomizedImagesDirectory =>
        Globals.GetFilePath(Path.Combine("assets", "CustomImages", "NPCs"));

    /// <summary>
	/// A list of swap pools - these are lists of lists of NPCs that can be shuffled among each other
	/// </summary>
	private static readonly List<List<string>> SwapPools = new()
    {
        // Normal-sized NPCs
        new()
        {
            "Abigail",
            "Alex",
            "Caroline",
            "Demetrius",
            "Elliott",
            "Emily",
            "Evelyn",
            "George",
            "Gus",
            "Haley",
            "Harvey",
            "Jas",
            "Kent",
            "Leah",
            "Lewis",
            "Linus",
            "Marnie",
            "Maru",
            "Morris",
            "Penny",
            "Pierre",
            "Robin",
            "Sam",
            "Sandy",
            "Sebastian",
            "Shane",
            "Vincent",
            "Wizard"
        },

        // Smaller NPCs
        new()
        {
            "Dwarf",
            "Krobus"
        },

        // NPCs with limited walking animations
        new()
        {
            "Gunther",
            "MrQi",
        }
    };

    /// <summary>
    /// Gets a list of data containing the swapped NPC assets and their corresponding
    /// Stardew asset names so they can be replaced
    /// </summary>
    /// <returns />
    public static List<NpcSwapperData> GetSwappedNpcAssets()
    {
        CleanUpRandomizedImageDirectory();

        List<NpcSwapperData> npcSwapperData = new();
        if (!Globals.Config.NPCs.SpriteShuffle)
        {
            return npcSwapperData;
        }

        Globals.SpoilerWrite("==== NPC SKINS ====");

        Rng = RNG.GetFarmRNG($"{nameof(NpcSkinSwapper)}");

        npcSwapperData = GetNpcSwaps();
        npcSwapperData.ForEach(npcSwap => TrySaveImage(npcSwap));

        Globals.SpoilerWrite("");

        return npcSwapperData;
    }

    private static List<NpcSwapperData> GetNpcSwaps()
    {
        List<NpcSwapperData> npcSwapData = new();
        foreach(var swapPool in SwapPools)
        {
            List<string> possibleSwaps = new(swapPool);
            foreach (var npc in swapPool)
            {
                var replacementNPC = Rng.GetAndRemoveRandomValueFromList(possibleSwaps);
                NPCSkinSwapPaths npcSwap = new(npc, replacementNPC);
                AddNpcSwaps(npcSwapData, npcSwap);

                Globals.SpoilerWrite($"{npc} => {replacementNPC}");
            }
        }
        return npcSwapData;
    }

    private static void AddNpcSwaps(
        List<NpcSwapperData> npcSwapData, NPCSkinSwapPaths npcSwap)
    {
        AddSwapForSuffix(npcSwapData, npcSwap);

        //TODO - enable and deal with these
        //AddSwapForSuffix(npcSwapData, npcSwap, NPCSkinSwapPaths.BeachSuffix);
        //AddSwapForSuffix(npcSwapData, npcSwap, NPCSkinSwapPaths.WinterSuffix);
        //AddSwapForSuffix(npcSwapData, npcSwap, NPCSkinSwapPaths.HospitalSuffix);
        //AddSwapForSuffix(npcSwapData, npcSwap, NPCSkinSwapPaths.JojaMartSuffix);
    } 

    private static void AddSwapForSuffix(
        List<NpcSwapperData> npcSwapData, 
        NPCSkinSwapPaths npcSwap, 
        string suffix = "")
    {
        var npcSpriteSheetName = $"{npcSwap.OriginalCharacterPath}{suffix}";
        Texture2D replacementNpcSpriteSheet = Globals.ModRef.Helper.GameContent
            .Load<Texture2D>($"{npcSwap.ReplacementCharacterPath}{suffix}");

        //TODO: we should be modifying the replacement sprite sheet here
        // to match the dimensions correctly

        npcSwapData.Add(new NpcSwapperData(
            $"{npcSwap.OriginalNPC}{suffix}", 
            npcSpriteSheetName, 
            replacementNpcSpriteSheet));

        //TODO: do portraits as well
    }

    /// <summary>
    /// Clean up the randomized image directory
    /// This is so they're gone if you turn off the setting
    /// </summary>
    private static void CleanUpRandomizedImageDirectory()
    {
        Directory.CreateDirectory(RandomizedImagesDirectory);
        DirectoryInfo directoryInfo = new(RandomizedImagesDirectory);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
    }

    /// <summary>
    /// If we're saving randomized images, and there was actually a hue shift,
    /// then write the images to a CustomImages/NPCs directory
    /// </summary>
    /// <param name="image">The image</param>
    private static void TrySaveImage(NpcSwapperData npcSwapperData)
    {
        if (Globals.Config.SaveRandomizedImages)
        {
            Texture2D image = npcSwapperData.NpcImage;
            using FileStream stream = File.OpenWrite(
                Path.Combine(RandomizedImagesDirectory, $"{npcSwapperData.AssetName}.png"));
            image.SaveAsPng(stream, image.Width, image.Height);
        }
    }

    /// <summary>
    /// Used to easily calculate all the replacement paths for an npc swap
    /// </summary>
    private class NPCSkinSwapPaths
    {
        public const string BeachSuffix = "_Beach";
        public const string WinterSuffix = "_Winter";
        public const string HospitalSuffix = "_Hospital";
        public const string JojaMartSuffix = "_JojaMart";

        public static string CharacterPath => $"Characters";
        public static string PortraitPath => $"Portraits";

        public string OriginalNPC { get; }
        public string ReplacementNPC { get; }

        public string OriginalCharacterPath 
            => $"{CharacterPath}{Path.DirectorySeparatorChar}{OriginalNPC}";
        public string ReplacementCharacterPath 
            => $"{CharacterPath}{Path.DirectorySeparatorChar}{ReplacementNPC}";
        public string OriginalPortraitPath 
            => $"{PortraitPath}{Path.DirectorySeparatorChar}{OriginalNPC}";
        public string ReplacementPortraitPath 
            => $"{PortraitPath}{Path.DirectorySeparatorChar}{ReplacementNPC}";

        public NPCSkinSwapPaths(string originalNPC, string replacementNPC)
        {
            OriginalNPC = originalNPC;
            ReplacementNPC = replacementNPC;
        }
    }

    /// <summary>
    /// The NPC data to return back to the asset loader
    /// This can be either sprites or portraits
    /// </summary>
    public class NpcSwapperData
    {
        /// <summary>
        /// The path to the asset (the entire thing, including the file name)
        /// </summary>
        public string StardewAssetPath { get; private set; }

        /// <summary>
        /// The name of the modified asset (just the file name, used for the saving the image)
        /// </summary>
        public string AssetName { get; private set; }

        /// <summary>
        /// The modified image
        /// </summary>
        public Texture2D NpcImage { get; private set; }

        public NpcSwapperData(string assetName, string assetPath, Texture2D npcImage)
        {
            AssetName = assetName;
            StardewAssetPath = assetPath;
            NpcImage = npcImage;
        }
    }
}
