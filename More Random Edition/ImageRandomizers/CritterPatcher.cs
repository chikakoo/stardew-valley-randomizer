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
        private const int CritterImageWidth = 320;
        private RNG Rng { get; set; }
        private static Texture2D CritterSpriteSheet { get; set; }

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
                new Point(64, 228),
                new Point(128, 228),
                new Point(192, 228),
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
                            mainSheetStartingPoint: new Point(0, 273)),
                        new(size: 18,
                            numberOfSprites: 3,
                            mainSheetStartingPoint: new Point(0, 291),
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
        /// The critter sprite sheet is kind of a mess of sprites all over the place
        /// This data is to group each animal into its own set so that we can hue
        /// shift each sprite in the same way
        /// </summary>
       // private static readonly List<List<CritterSpriteLocation>> CritterLocationData = new()
        //{
            // DATA TO USE
            // Seagull - 32x32 (14 sprites)
            // 0, 0

            // Crow - 32x32 (11 sprites)
            // 128, 32; 11 

            // Perching Bird - 32x32 (9 sprites) AND 12x12 (5 sprites) (start at 0, 32*9)
            // 160, 64 AND 47, 333

            // Birds - 32x32 (9 sprites)
            // 160, 128
            // 160, 384
            // 160, 416
            // 160, 512
            // 160, 544

            // Butterflies - 16x16 (4 sprites)
            // 128, 96
            // 192, 96
            // 256, 96
            // 128, 112
            // 192, 112
            // 256, 112
            // 64, 228
            // 128, 228
            // 192, 228
            // 256, 288
            // 128, 336
            // 192, 336
            // 0, 384
            // 64, 384

            // Small Butterflies - 16x16 (3 sprites)
            // 0, 128
            // 48, 128
            // 96, 128
            // 0, 144
            // 48, 144
            // 96, 144
            // 224, 304
            // 272, 304

            // Woodpecker - 16x16 (5 sprites)
            // 0, 256

            // Owl - 32x32 (4 sprites)
            // 96, 256

            // Bunnies - 32x32 (7 sprites)
            // 6 sprites at 128, 160; 1 sprite at 256, 192
            // 6 sprites at 128, 224; 1 sprite at 288, 192

            // Squirrel - 32x32 (8 sprites)
            // 0, 192

            // Frogs - 16x16 (7 sprites)
            // 0, 224
            // 0, 240

            // Crab - 18x18 (6 sprites) (NOT A TYPO, IT IS 18)
            // 0, 273 (3 of the sprites)
            // - then, at 0, 291, the other 3 (0, 18*3 on the frog spritesheet)

            // Red Monkey - 20x24 (7 sprites), then 15x12 (3 sprites, starting at 0,20*7)
            // 0, 309 AND...
            // - 0, 333 = the 3 monkey heads

            // Monkey - 20x24 (4 sprites)
            // 141, 309

            // Gorilla - 32x32 (7 sprites)
            // 0, 352

            // Opossum - 32x32 (9 sprites)
            // 0, 480


            //// Seagull
            //new List<CritterSpriteLocations>() { 
            //    new(size: 32, startingPoint: new Point(0, 0), numberOfSprites: 10),
            //    new(size: 32, startingPoint: new Point(0, 32), numberOfSprites: 4)},

            //// Crow
            //new List<CritterSpriteLocations>() { 
            //    new(size: 32, startingPoint: new Point(128, 32), numberOfSprites: 6),
            //    new(size: 32, startingPoint: new Point(0, 64), numberOfSprites: 5)},

            //// Brown Perching Bird
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(160, 64), numberOfSprites: 5),
            //    new(size: 32, startingPoint: new Point(0, 96), numberOfSprites: 4),
            //    new(size: 12, startingPoint: new Point(47, 333), numberOfSprites: 5) },

            //// Blue Bird
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(160, 128), numberOfSprites: 5),
            //    new(size: 32, startingPoint: new Point(0, 160), numberOfSprites: 4)},

            //// Purple Bird
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(160, 384), numberOfSprites: 5),
            //    new(size: 32, startingPoint: new Point(0, 416), numberOfSprites: 4)},

            //// Red Bird
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(160, 416), numberOfSprites: 5),
            //    new(size: 32, startingPoint: new Point(0, 448), numberOfSprites: 4)},

            //// Owl
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(96, 256), numberOfSprites: 4) },
            
            //// Woodpecker
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(0, 256), numberOfSprites: 5) },

            //// Chicken Bird
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(160, 512), numberOfSprites: 5),
            //    new(size: 32, startingPoint: new Point(0, 544), numberOfSprites: 4)},

            //// Dove
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(160, 544), numberOfSprites: 5),
            //    new(size: 32, startingPoint: new Point(0, 576), numberOfSprites: 4)},

            //// -- Butterflies --

            //// Big, colored
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(128, 96), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(192, 96), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(256, 96), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(128, 112), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(192, 112), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(256, 112), numberOfSprites: 4) },

            //// Small, patterned
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(0, 128), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(48, 128), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(96, 128), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(0, 144), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(48, 144), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(96, 144), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(224, 304), numberOfSprites: 3) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(272, 304), numberOfSprites: 3) },

            //// Tropical
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(64, 288), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(128, 288), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(192, 288), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(256, 288), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(128, 336), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(192, 336), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(0, 384), numberOfSprites: 4) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(64, 384), numberOfSprites: 4) },
        
            //// -- Ground Critters --

            //// Gray Bunny
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(128, 160), numberOfSprites: 6),
            //    new(size: 32, startingPoint: new Point(256, 192), numberOfSprites: 1) },

            //// White Bunny
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(288, 192), numberOfSprites: 1),
            //    new(size: 32, startingPoint: new Point(128, 224), numberOfSprites: 6) },

            //// Squirrel
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(0, 192), numberOfSprites: 8) },

            //// Frogs
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(0, 224), numberOfSprites: 7) },
            //new List<CritterSpriteLocations>() {
            //    new(size: 16, startingPoint: new Point(0, 240), numberOfSprites: 7) },

            //// Crab
            //new List<CritterSpriteLocations>() {
            //    new(size: 18, startingPoint: new Point(0, 273), numberOfSprites: 3),
            //    new(size: 18, startingPoint: new Point(0, 291), numberOfSprites: 3)},

            //// Red Monkey
            //new List<CritterSpriteLocations>() {
            //    new(width: 20, height: 24, startingPoint: new Point(0, 309), numberOfSprites: 7),
            //    new(width: 15, height: 12, startingPoint: new Point(0, 333), numberOfSprites: 3) },

            //// Monkey
            //new List<CritterSpriteLocations>() {
            //    new(width: 20, height: 24, startingPoint: new Point(141, 309), numberOfSprites: 4) },

            //// Gorilla
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(0, 352), numberOfSprites: 7) },

            //// Opossum 
            //new List<CritterSpriteLocations>() {
            //    new(size: 32, startingPoint: new Point(0, 480), numberOfSprites: 9) },
        //};

        /// <summary>
        /// The path to the critter asset
        /// </summary>
        public const string StardewAssetPath = $"TileSheets/critters";

        public CritterPatcher()
        {
            SubFolder = "CustomImages/TileSheets/Critters";
        }

        public override void OnAssetRequested(IAssetData asset)
        {
            if (!Globals.Config.Animals.RandomizeCritters)
            {
                return;
            }

            CritterSpriteSheet = Globals.ModRef.Helper.GameContent.Load<Texture2D>(StardewAssetPath);
            Rng = RNG.GetDailyRNG(nameof(CritterPatcher));

            var editor = asset.AsImage();
            ReplaceAllCritters(editor);

            if (Globals.Config.SaveRandomizedImages)
            {
                TryWriteRandomizedImage(Globals.ModRef.Helper.GameContent.Load<Texture2D>(StardewAssetPath));
            }
        }

        private void ReplaceAllCritters(IAssetDataForImage editor)
        {
            // For each sprite set (for example, all kinds of birds with 9 sprites)
            foreach (var spriteSet in CritterSpriteSets)
            {
                // For each place in the main sprite sheet we need to replace
                foreach (var spriteLocationList in spriteSet.SpriteLocations) 
                {
                    var subDirectory = spriteSet.SubDirectory;
                    var files = GetAllFileNamesInFolder(subDirectory);
                    if (files.Count == 0)
                    {
                        Globals.ConsoleWarn($"No critter images found - skipping critter set: {subDirectory}");
                    }

                    // For each sub-section of the sprite sheet we need to replace
                    // e.g. the Red Monkey has two pieces (hence, we use the same sprite sheet image here)
                    var spriteSheetDirectory = Path.Combine(PatcherImageFolder, subDirectory);
                    var spriteSheetFullPath = GetRandomCritterImage(files, subDirectory, spriteSheetDirectory);
                    foreach (var spriteLocation in spriteLocationList)
                    {
                        //TODO: "using" here?
                        
                        var hueShiftValue = Rng.NextIntWithinRange(0, Globals.Config.Animals.CritterHueShiftMax);
                        Texture2D critterSpriteSheet = ImageManipulator.ShiftImageHue(
                            Globals.ModRef.Helper.ModContent
                                .Load<Texture2D>(spriteSheetFullPath), hueShiftValue);

                        PatchMainSpriteSheet(editor, critterSpriteSheet, spriteLocation);
                    }
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
                //TODO: test that this actually works
                Globals.ConsoleWarn($"Refreshing critter list, as we ran out for set: {critterSetName}");
                fileNames = GetAllFileNamesInFolder(critterSetName);
            }

            var fileName = Rng.GetAndRemoveRandomValueFromList(fileNames);
            return Path.Combine(pathToImages, fileName);
        }

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

                // TODO: FIND OUT WHY THIS PART IS ONLY AFFECTING THE FIRST IMAGE ON WRAPAROUND

                // If we've gone beyond the edge of the page, wrap around to the start of the next line
                if (targetX + spriteWidth > CritterImageWidth)
                {
                    // We now need to offset everything by these values, as we're on the second line
                    xOffset = targetX;
                    yOffset = spriteHeight;

                    targetX = 0;
                    targetY += yOffset;
                }

                Rectangle targetRectangle = new(targetX, targetY, spriteWidth, spriteHeight);

                editor.PatchImage(critterSpriteSheet, sourceRectangle, targetRectangle);
            }
        }


        //private static void HueShiftAllCritters(IAssetDataForImage editor, RNG rng)
        //{
            //foreach(List<CritterSpriteLocation> spriteLocsList in CritterLocationData)
            //{
            //    int hueShiftValue = rng.NextIntWithinRange(0, Globals.Config.Animals.CritterHueShiftMax);
            //    foreach (CritterSpriteLocation spriteLocs in spriteLocsList)
            //    {
            //        HueShiftFromSpriteLocs(spriteLocs, editor, hueShiftValue);
            //    }
            //}
        //}

        //private static void HueShiftFromSpriteLocs(
        //    CritterSpriteLocation spriteLocs, 
        //    IAssetDataForImage editor, 
        //    int hueShiftValue)
        //{
        //    int startingX = spriteLocs.MainSheetStartingPoint.X;
        //    int y = spriteLocs.MainSheetStartingPoint.Y;
        //    int width = spriteLocs.Width;
        //    int height = spriteLocs.Height;
        //    for (int i = 0; i < spriteLocs.NumberOfSprites; i++)
        //    {
        //        int x = (width * i) + startingX;
        //        Rectangle cropRectangle = new(x, y, width, height);

        //        using Texture2D hueShiftedCritter = ImageManipulator.ShiftImageHue(
        //            ImageManipulator.Crop(CritterSpriteSheet, cropRectangle, Game1.graphics.GraphicsDevice),
        //            hueShiftValue);

        //        editor.PatchImage(hueShiftedCritter, targetArea: cropRectangle);
        //    }
        //}

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
                SpriteLocations = new()
                {
                    spriteSheetStartingPoints
                        .Select(point => new CritterSpriteLocation(size, numberOfSprites, point))
                        .ToList()
                };
            }

            public CritterSpriteSet(
                string subDirectory,
                int width,
                int height,
                int numberOfSprites,
                params Point[] spriteSheetStartingPoints)
            {
                SubDirectory = subDirectory;
                SpriteLocations = new()
                {
                    spriteSheetStartingPoints
                        .Select(point => new CritterSpriteLocation(
                            width, height, numberOfSprites, point))
                        .ToList()
                };
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
