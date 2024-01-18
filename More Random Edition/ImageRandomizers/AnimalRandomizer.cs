﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;

namespace Randomizer
{
    /// <summary>
    /// Creates an image file for the given type of animal
    /// </summary>
    public class AnimalRandomizer : ImageBuilder
    {
        /// <summary>
        /// The type of animal this is randomizing
        /// </summary>
        private AnimalTypes AnimalTypeToRandomize { get; set; }

        public AnimalRandomizer(AnimalTypes animalTypeToRandomize)
        {
            AnimalTypeToRandomize = animalTypeToRandomize;
            SubDirectory = $"Animals/{animalTypeToRandomize}";
            StardewAssetPath = GetStardewAssetPath();
        }

        /// <summary>
        /// Get the path to the Stardew-equivalent asset based on the animal type
        /// </summary>
        /// <returns>The path of the xnb file to replace</returns>
        private string GetStardewAssetPath()
        {
            switch(AnimalTypeToRandomize)
            {
                case AnimalTypes.Horses:
                    return "Animals/horse";
                case AnimalTypes.Pets:
                    return "Animals/cat";
                default:
                    Globals.ConsoleWarn($"Stardew asset path undefined for animal type: {AnimalTypeToRandomize}");
                    return "";
            }
        }

        /// <summary>
        /// Build the image - hue shift it if the base file name ends with "-hue-shift"
        /// </summary>
        protected override Texture2D BuildImage()
        {
            string randomAnimalFileName = GetRandomAnimalFileName();
            string imageLocation = $"{ImageDirectory}/{randomAnimalFileName}";
            Texture2D animalImage = Texture2D.FromFile(Game1.graphics.GraphicsDevice, imageLocation);

            if (randomAnimalFileName[..^4].EndsWith("-hue-shift"))
            {
                int hueShiftValue = Range.GetRandomValue(0, 359);
                Color shiftedPaleColor = ImageManipulator.IncreaseHueBy(ImageManipulator.PaleColor, hueShiftValue);
                animalImage = ImageManipulator.MultiplyImageByColor(animalImage, shiftedPaleColor);
            }

            if (ShouldSaveImage() && Globals.Config.SaveRandomizedImages)
            {
                using FileStream stream = File.OpenWrite(OutputFileFullPath);
                animalImage.SaveAsPng(stream, animalImage.Width, animalImage.Height);

                Globals.SpoilerWrite($"{AnimalTypeToRandomize} replaced with {randomAnimalFileName[..^4]}");
            }

            return animalImage;
        }

        /// <summary>
        /// Gets a random animal file name from the randomizers current directory
        /// This will use a new RNG seed so that we can get the pet name out of order
        /// </summary>
        /// <returns></returns>
        private string GetRandomAnimalFileName()
        {
            byte[] seedvar = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value));
            int seed = BitConverter.ToInt32(seedvar, 0);
            Random rng = new(seed);

            var animalImages = Directory.GetFiles($"{ImageDirectory}")
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileName(x))
                .OrderBy(x => x)
                .ToList();

            return Globals.RNGGetRandomValueFromList(animalImages, rng);
        }

        /// <summary>
        /// Gets the random pet name generated for the farm
        /// This should always be the same value since the seed should start in the same spot
        /// </summary>
        /// <returns></returns>
        public static string GetRandomPetName()
        {
            return new AnimalRandomizer(AnimalTypes.Pets).GetRandomAnimalFileName();
        }

        /// <summary>
        /// Whether we should save the image
        /// Based on the appriate Animal randomize setting
        /// </summary>
        /// <returns>True if we should save; false otherwise</returns>
        public override bool ShouldSaveImage()
        {
            switch (AnimalTypeToRandomize)
            {
                case AnimalTypes.Horses: 
                    return Globals.Config.Animals.RandomizeHorses;
                case AnimalTypes.Pets:
                    return Globals.Config.Animals.RandomizePets;
                default:
                    Globals.ConsoleError($"Tried to save randomized image of unrecognized Animal type: {AnimalTypeToRandomize}");
                    return false;
            }
        }
    }
}
