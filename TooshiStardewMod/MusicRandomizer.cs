﻿using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the music in the game
	/// </summary>
	public class MusicRandomizer
	{
		/// <summary>
		/// Randomizes all the music to another song
		/// </summary>
		/// <returns>A dictionary of song names to their alternatives</returns>
		public static Dictionary<string, string> Randomize()
		{
			List<string> musicList = new List<string>
			{
				"CloudCountry",
				"coin",
				"grandpas_theme",
				"jojaOfficeSoundscape",
				"musicboxsong",
				"spring_day_ambient",
				"SettlingIn",
				"spring1",
				"springtown",
				"marnieShop",
				"Saloon1",
				"spring_night_ambient",
				"spring2",
				"Hospital_Ambient",
				"ocean",
				"distantBanjo",
				"communityCenter",
				"spring3",
				"WizardSong",
				"clubloop",
				"MarlonsTheme",
				"Upper_Ambient",
				"Secret Gnomes",
				"libraryTheme",
				"fallFest",
				"tickTock",
				"event1",
				"playful",
				"nightTime",
				"echos",
				"FlowerDance",
				"summer1",
				"summer_day_ambient",
				"summer2",
				"summer3",
				"Frost_Ambient",
				"XOR",
				"event2",
				"aerobics",
				"sweet",
				"jaunty",
				"pool_ambient",
				"moonlightJellies",
				"fall1",
				"fall_day_ambient",
				"fall2",
				"fall3",
				"woodsTheme",
				"Lava_Ambient",
				"Of Dwarves",
				"junimoStarSong",
				"spirits_eve",
				"winter1",
				"winter_day_ambient",
				"winter2",
				"wavy",
				"winter3",
				"christmasTheme",
				"50s",
				"ragtime",
				"sampractice",
				"breezy",
				"kindadumbautumn",
				"Crystal Bells",
				"bigDrums",
				"sadpiano",
				"desolate",
				"EmilyDream",
				"EmilyTheme",
				"bugLevelLoop",
				"AbigailFlute",
				"wedding"
			};
			List<string> musicReplacementPool = new List<string>(musicList);
			Dictionary<string, string> musicReplacements = new Dictionary<string, string>();

			foreach (string song in musicList)
			{

				musicReplacements.Add(song.ToLower(), Globals.RNGGetAndRemoveRandomValueFromList(musicReplacementPool));
			}

			return musicReplacements;
		}

	}
}