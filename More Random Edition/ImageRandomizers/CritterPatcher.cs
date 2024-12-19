using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
    public class CritterPatcher : ImagePatcher
    {
        /// <summary>
        /// The size of the critter image sprite sheet
        /// If this ever changes, we'd need to adjust things, so it's okay to hard-code it here
        /// </summary>
        private const int CritterImageWidth = 320;

        /// <summary>
        /// The RNG for the patcher - intended to be set to daily RNG
        /// </summary>
        private RNG Rng { get; set; }

        /// <summary>
        /// The for all the sprites to replace
        /// - This is a list of sets, separated by what folder they are in
        /// - Each Set has a list of a list of data
        ///   - The outer list consists of each critter to replace on the sprite sheet
        ///   - The inner list consists of pieces of that specific critter (e.g. the Red Monkey has a separate head image)
        /// </summary>
        private static readonly List<CritterSpriteSet> CritterSpriteSets = new()
        {
            new("Seagull",
                size: 32,
                numberOfSprites: 14,
                Point.Zero),
            new("Crow",
                size: 32,
                numberOfSprites: 11,
                new Point(128, 32)),
            new("Perching Bird",
                new List<List<CritterSpriteLocation>>
                {
                    new()
                    {
                        new(size: 32,
                            numberOfSprites: 9,
                            mainSheetStartingPoint: new Point(160, 64)),
                        new(size: 12,
                            numberOfSprites: 5,
                            mainSheetStartingPoint: new Point(47, 333),
                            spriteSheetStartingPoint: new Point(32 * 9, 0))
                    }
                }),
            new("Birds",
                size: 32,
                numberOfSprites: 9,
                new Point(160, 128),
                new Point(160, 384),
                new Point(160, 416),
                new Point(160, 512),
                new Point(160, 544)),
            new("Butterflies",
                size: 16,
                numberOfSprites: 4,
                new Point(128, 96),
                new Point(192, 96),
                new Point(256, 96),
                new Point(128, 112),
                new Point(192, 112),
                new Point(256, 112),
                new Point(64, 288),
                new Point(128, 288),
                new Point(192, 288),
                new Point(256, 288),
                new Point(128, 336),
                new Point(192, 336),
                new Point(0, 384),
                new Point(64, 384)),
            new("Small Butterflies",
                size: 16,
                numberOfSprites: 3,
                new Point(0, 128),
                new Point(48, 128),
                new Point(96, 128),
                new Point(0, 144),
                new Point(48, 144),
                new Point(96, 144),
                new Point(224, 304),
                new Point(272, 304)),
            new("Woodpecker",
                size: 16,
                numberOfSprites: 5,
                new Point(0, 256)),
            new("Owl",
                size: 32,
                numberOfSprites: 4,
                new Point(96, 256)),
            new("Bunnies",
                new List<List<CritterSpriteLocation>>
                {
                    new()
                    {
                        new(size: 32,
                            numberOfSprites: 6,
                            mainSheetStartingPoint: new Point(128, 160)),
                        new(size: 32,
                            numberOfSprites: 1,
                            mainSheetStartingPoint: new Point(256, 192),
                            spriteSheetStartingPoint: new Point(32 * 6, 0))
                    },
                    new()
                    {
                        new(size: 32,
                            numberOfSprites: 6,
                            mainSheetStartingPoint: new Point(128, 224)),
                        new(size: 32,
                            numberOfSprites: 1,
                            mainSheetStartingPoint: new Point(288, 192),
                            spriteSheetStartingPoint: new Point(32 * 6, 0))
                    }
                }),
            new("Squirrel",
                size: 32,
                numberOfSprites: 8,
                new Point(0, 192)),
            new("Frogs",
                size: 16,
                numberOfSprites: 7,
                new Point(0, 224),
                new Point(0, 240)),
            new("Crab",
                new List<List<CritterSpriteLocation>>
                {
                    new()
                    {
                        new(size: 18,
                            numberOfSprites: 3,
                            mainSheetStartingPoint: new Point(0, 272)),
                        new(size: 18,
                            numberOfSprites: 3,
                            mainSheetStartingPoint: new Point(0, 290),
                            spriteSheetStartingPoint: new Point(18 * 3, 0))
                    }
                }),
            new("Red Monkey",
                new List<List<CritterSpriteLocation>>
                {
                    new()
                    {
                        new(width: 20,
                            height: 24,
                            numberOfSprites: 7,
                            mainSheetStartingPoint: new Point(0, 309)),
                        new(width: 15,
                            height: 12,
                            numberOfSprites: 3,
                            mainSheetStartingPoint: new Point(0, 333),
                            spriteSheetStartingPoint: new Point(20 * 7, 0))
                    }
                }),
            new("Monkey",
                width: 20,
                height: 24,
                numberOfSprites: 4,
                new Point(141, 309)),
            new("Gorilla",
                size: 32,
                numberOfSprites: 7,
                new Point(0, 352)),
            new("Opossum",
                size: 32,
                numberOfSprites: 9,
                new Point(0, 480))
        };

        /// <summary>
        /// The path to the critter asset
        /// </summary>
        public const string StardewAssetPath = $"TileSheets/critters";

        public CritterPatcher()
        {
            SubFolder = "CustomImages/TileSheets/Critters";
        }

        /// <summary>
        /// Called when the asset is requested - this is where we'll do our work
        /// Exits if we aren't randomizing critters
        /// </summary>
        /// <param name="asset">The critter image asset</param>
        public override void OnAssetRequested(IAssetData asset)
        {
            if (!Globals.Config.Animals.RandomizeCritters)
            {
                return;
            }

            Rng = RNG.GetDailyRNG(nameof(CritterPatcher));

            var editor = asset.AsImage();
            ReplaceAllCritters(editor);

            if (Globals.Config.SaveRandomizedImages)
            {
                TryWriteRandomizedImage(Globals.ModRef.Helper.GameContent.Load<Texture2D>(StardewAssetPath));
            }
        }

        /// <summary>
        /// Replaces all critters with random ones in the TileSheets/Critters folders
        /// Hue shifts them as well, according to the setting
        /// </summary>
        /// <param name="editor">The critters asset from the game</param>
        private void ReplaceAllCritters(IAssetDataForImage editor)
        {
            // For each sprite set (for example, all kinds of birds with 9 sprites)
            foreach (var spriteSet in CritterSpriteSets)
            {
                var subDirectory = spriteSet.SubDirectory;
                var files = GetAllFileNamesInFolder(subDirectory);
                if (files.Count == 0)
                {
                    Globals.ConsoleWarn($"No critter images found - skipping critter set: {subDirectory}");
                    continue;
                }

                var spriteSheetDirectory = Path.Combine(PatcherImageFolder, subDirectory);

                // For each place in the main sprite sheet we need to replace
                foreach (var spriteLocationList in spriteSet.SpriteLocations) 
                {
                    // For each sub-section of the sprite sheet we need to replace
                    // e.g. the Red Monkey has two pieces (hence, we use the same sprite sheet image here)
                    var spriteSheetFullPath = GetRandomCritterImage(files, subDirectory, spriteSheetDirectory);
                    var hueShiftValue = spriteSheetFullPath.EndsWith("no-hue-shift.png")
                        ? 0
                        : Rng.NextIntWithinRange(0, Globals.Config.Animals.CritterHueShiftMax);
                    using Texture2D critterSpriteSheet = ImageManipulator.ShiftImageHue(
                        Globals.ModRef.Helper.ModContent
                            .Load<Texture2D>(spriteSheetFullPath), hueShiftValue);
                    spriteLocationList.ForEach(spriteLocation =>
                        PatchMainSpriteSheet(editor, critterSpriteSheet, spriteLocation));
                }
            }
        }

        /// <summary>
        /// Returns the path of a random critter image
        /// Refills the list if there's none in it and logs a warning
        /// </summary>
        /// <param name="fileNames">The list of possible images</param>
        /// <param name="critterSetName">The name of the critter set</param>"
        /// <param name="pathToImages">The path to the images to look in</param>
        /// <returns>The path of the random image retrieved</returns>
        private string GetRandomCritterImage(
            List<string> fileNames, string critterSetName, string pathToImages)
        {
            if (fileNames.Count == 0)
            {
                Globals.ConsoleWarn($"Refreshing critter list, as we ran out for set: {critterSetName}");
                fileNames = GetAllFileNamesInFolder(critterSetName);
            }

            var fileName = Rng.GetAndRemoveRandomValueFromList(fileNames);
            return Path.Combine(pathToImages, fileName);
        }

        /// <summary>
        /// Handles logic to patch the main sprite sheet with a given critter sprite sheet
        /// Handles wrapping around the main sprite sheet
        /// </summary>
        /// <param name="editor">The critters asset from the game</param>
        /// <param name="critterSpriteSheet">The sprite sheet we want to replace the existing one with</param>
        /// <param name="spriteLocation">The sprite location, containing the info on how to patch the sprite sheets</param>
        private static void PatchMainSpriteSheet(
            IAssetDataForImage editor, 
            Texture2D critterSpriteSheet, 
            CritterSpriteLocation spriteLocation)
        {
            var yOffset = 0;
            var xOffset = 0;
            for (var i = 0; i < spriteLocation.NumberOfSprites; i++)
            {
                var spriteWidth = spriteLocation.Width;
                var spriteHeight = spriteLocation.Height;

                var sourceX = spriteLocation.SpriteSheetStartingPoint.X + i * spriteWidth;
                var sourceY = spriteLocation.SpriteSheetStartingPoint.Y;
                Rectangle sourceRectangle = new(sourceX, sourceY, spriteWidth, spriteHeight);

                var targetX = (spriteLocation.MainSheetStartingPoint.X + i * spriteWidth) + xOffset;
                var targetY = spriteLocation.MainSheetStartingPoint.Y + yOffset;

                // If we've gone beyond the edge of the page, wrap around to the start of the next line
                if (targetX + spriteWidth > CritterImageWidth)
                {
                    // We now need to offset everything by these values, as we're on the second line
                    xOffset = -targetX;
                    yOffset = spriteHeight;

                    targetX = 0;
                    targetY += yOffset;
                }

                Rectangle targetRectangle = new(targetX, targetY, spriteWidth, spriteHeight);

                editor.PatchImage(critterSpriteSheet, sourceRectangle, targetRectangle);
            }
        }

        /// <summary>
        /// Groups all sprite locations into a set so that it can be grouped in a single directory
        /// </summary>
        private class CritterSpriteSet
        {
            /// <summary>
            /// The folder all the critter images for this set can be pulled from
            /// </summary>
            public string SubDirectory { get; set; }

            /// <summary>
            /// A list of a list of sprite locations
            /// - Each value contains a list of all the starting points on the sprite sheet to where it
            ///   needs to go on the main sheet
            /// - The sub-list is for any case where a single critter has multiple pieces to it,
            ///   like the bunny or perching bird
            /// </summary>
            public List<List<CritterSpriteLocation>> SpriteLocations { get; set; }

            /// <summary>
            /// Constructor for a more complex sprite set that has multiple sizes/locations on the main
            /// sprite sheet
            /// </summary>
            /// <param name="subDirectory">The sub directory</param>
            /// <param name="spriteLocations">The sprite locations</param>
            public CritterSpriteSet(
                string subDirectory,
                List<List<CritterSpriteLocation>> spriteLocations)
            {
                SubDirectory = subDirectory;
                SpriteLocations = spriteLocations;
            }

            /// <summary>
            /// Constructor for a sprite set that has a simple animal type only consisting of a
            /// row of images of the same dimentions, mapping all in a row to the main sheet
            /// </summary>
            /// <param name="subDirectory">The sub directory</param>
            /// <param name="size">The size of one sprite</param>
            /// <param name="numberOfSprites">The number of sprites in the sheet</param>
            /// <param name="spriteSheetStartingPoints">The starting points to map the critters to</param>
            public CritterSpriteSet(
                string subDirectory,
                int size,
                int numberOfSprites,
                params Point[] spriteSheetStartingPoints)
            {
                SubDirectory = subDirectory;
                SpriteLocations = spriteSheetStartingPoints
                    .Select(point => new List<CritterSpriteLocation> 
                    { 
                        new(size, numberOfSprites, point) 
                    }).ToList();
            }

            /// <summary>
            /// Constructor for a sprite set that has a simple animal type only consisting of a
            /// row of images of the same dimentions, mapping all in a row to the main sheet
            /// </summary>
            /// <param name="subDirectory">The sub directory</param>
            /// <param name="width">The width of one sprite</param>
            /// <param name="height">The height of one sprite</param>
            /// <param name="numberOfSprites">The number of sprites in the sheet</param>
            /// <param name="spriteSheetStartingPoints">The starting points to map the critters to</param>
            public CritterSpriteSet(
                string subDirectory,
                int width,
                int height,
                int numberOfSprites,
                params Point[] spriteSheetStartingPoints)
            {
                SubDirectory = subDirectory;
                SpriteLocations = spriteSheetStartingPoints
                    .Select(point => new List<CritterSpriteLocation> 
                    { 
                        new(width, height, numberOfSprites, point) 
                    }).ToList();
            }
        }

        /// <summary>
        /// Contains the starting point and dimensions of the sprite sheet of a critter, along
        /// with the starting point and dimensions of where it should go on the main sheet
        /// </summary>
        private class CritterSpriteLocation
        {
            /// <summary>
            /// The width of the sprite in pixels
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// The height of the sprite in pixels
            /// </summary>
            public int Height { get; set; }

            /// <summary>
            /// Where to replace the sprites on the main sheet
            /// This is (x, y) in pixels
            /// </summary>
            public Point MainSheetStartingPoint { get; set; }

            /// <summary>
            /// Where to grab the sprites from on the current sprite set (NOT the main critter sheet)]
            /// This is (x, y) in pixels
            /// </summary>
            public Point SpriteSheetStartingPoint { get; set; }

            /// <summary>
            /// How many sprites in a row there are there
            /// </summary>
            public int NumberOfSprites { get; set; }

            public CritterSpriteLocation(
                int size, 
                int numberOfSprites, 
                Point mainSheetStartingPoint)
            {
                Width = size;
                Height = size;
                MainSheetStartingPoint = mainSheetStartingPoint;
                SpriteSheetStartingPoint = Point.Zero;
                NumberOfSprites = numberOfSprites;
            }

            public CritterSpriteLocation(
                int size,
                int numberOfSprites,
                Point mainSheetStartingPoint,
                Point spriteSheetStartingPoint)
            {
                Width = size;
                Height = size;
                MainSheetStartingPoint = mainSheetStartingPoint;
                SpriteSheetStartingPoint = spriteSheetStartingPoint;
                NumberOfSprites = numberOfSprites;
            }

            public CritterSpriteLocation(
                int width, 
                int height, 
                int numberOfSprites, 
                Point mainSheetStartingPoint)
            {
                Width = width;
                Height = height;
                MainSheetStartingPoint = mainSheetStartingPoint;
                SpriteSheetStartingPoint = Point.Zero;
                NumberOfSprites = numberOfSprites;
            }

            public CritterSpriteLocation(
                int width,
                int height,
                int numberOfSprites,
                Point mainSheetStartingPoint,
                Point spriteSheetStartingPoint)
            {
                Width = width;
                Height = height;
                MainSheetStartingPoint = mainSheetStartingPoint;
                SpriteSheetStartingPoint = spriteSheetStartingPoint;
                NumberOfSprites = numberOfSprites;
            }
        }
    }
}
