using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SVSeason = StardewValley.Season;

namespace Randomizer
{
    /// <summary>
    /// An enum for the seasons
    /// </summary>
    public enum Seasons
	{
		Spring,
		Summer,
		Fall,
		Winter
	}

	public static class SeasonsExtensions
	{
        /// <summary>
        /// Converts a SVSeason over to the rando version
        /// </summary>
        /// <param name="svSeason">The SVSeason to convert</param>
        /// <returns>The new Seasons value</returns>
        public static Seasons ConvertFromSVSeason(SVSeason svSeason)
        {
            switch(svSeason)
            {
                case SVSeason.Spring: 
                    return Seasons.Spring;
                case SVSeason.Summer: 
                    return Seasons.Summer;
                case SVSeason.Fall: 
                    return Seasons.Fall;
                case SVSeason.Winter:
                    return Seasons.Winter;
                default:
                    Globals.ConsoleError($"Converting unknown SVSeason to Seasons: {svSeason}");
                    return Seasons.Spring;
            }
        }

        /// <summary>
        /// Gets the season of the game as one of the enum values
        /// </summary>
        /// <returns>The current season</returns>
        public static Seasons GetCurrentSeason()
		{
			string currentSeason = Game1.currentSeason.ToLower();
			switch(currentSeason)
			{
				case "spring": return Seasons.Spring;
                case "summer": return Seasons.Summer;
                case "fall": return Seasons.Fall;
                case "winter": return Seasons.Winter;
				default:
					Globals.ConsoleError($"Tried to parse unexpected season string: {currentSeason}!");
					return Seasons.Spring; // Default to something
            }
		}

        /// <summary>
        /// Gets a random color that fits the given season
        /// </summary>
        /// <param name="season">The season</param>
        /// <param name="rng">The rng to use</param>
        /// <returns>The chosen color</returns>
        public static Color GetRandomColorForSeason(this Seasons season, RNG rng)
        {
            Range SpringHueRange = new(100, 155);
            Range SummerHueRange = new(50, 65);
            Range FallHueRange = new(10, 40);
            Range WinterHueRange = new(180, 260);

            return season switch
            {
                Seasons.Spring => ImageManipulator.GetRandomColor(rng, SpringHueRange),
                Seasons.Summer => ImageManipulator.GetRandomColor(rng, SummerHueRange),
                Seasons.Fall => ImageManipulator.GetRandomColor(rng, FallHueRange),
                _ => ImageManipulator.GetRandomColor(rng, WinterHueRange)
            };
        }

        /// <summary>
        /// Gets a random color for a list of given seasons
        /// 1 in list: calls the normal function for it
        /// 2 in list: averages them out
        /// 3+ in list: uses CyanAndBlue (passes Winter over)
        /// </summary>
        /// <param name="seasons">The seasons</param>
        /// /// <param name="rng">The rng to use</param>
        /// <returns>The chosen color</returns>
        public static Color GetRandomColorForSeasons(List<Seasons> seasons, RNG rng)
        {
            switch (seasons.Count)
            {
                case 1:
                    return seasons[0].GetRandomColorForSeason(rng);
                case 2:
                    var color1 = seasons[0].GetRandomColorForSeason(rng);
                    var color2 = seasons[1].GetRandomColorForSeason(rng);
                    return ImageManipulator.AverageColors(color1, color2);
                default:
                    return GetRandomColorForSeason(Seasons.Winter, rng);
            }
        }

        /// <summary>
        /// Gets a random season from the enum
        /// </summary>
        /// <param name="rng">The rng to use</param>
        /// <returns>The random season</returns>
        public static Seasons GetRandomSeason(RNG rng)
        {
            var allSeasons = Enum.GetValues(typeof(Seasons))
                .Cast<Seasons>()
                .ToList();

            return rng.GetRandomValueFromList(allSeasons);
        }
    }
}
