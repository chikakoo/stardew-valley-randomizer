﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Randomizer
{
	public class WeaponImageBuilder : ImageBuilder
	{
		private const string SwordSubDirectory = "Swords";
		private const string DaggerSubDirectory = "Daggers";
		private const string HammerAndClubSubDirectory = "HammersAndClubs";
		private const string SlingshotSubDirectory = "Slingshots";

		private List<string> SwordImages { get; set; }
		private List<string> DaggerImages { get; set; }
		private List<string> HammerAndClubImages { get; set; }
		private List<string> SlingshotImages { get; set; }

		/// <summary>
		/// A map of the weapon position in the dictionary to the id it belongs to
		/// </summary>
		private readonly Dictionary<Point, int> WeaponPositionToIDMap = new Dictionary<Point, int>()
		{
			{ new Point(0, 0), 0 },
			{ new Point(0, 1), 1 },
			{ new Point(0, 2), 2 },
			{ new Point(0, 3), 3 },
			{ new Point(0, 4), 4 },
			{ new Point(0, 5), 5 },
			{ new Point(0, 6), 6 },
			{ new Point(0, 7), 7 },

			{ new Point(1, 0), 8 },
			{ new Point(1, 1), 9 },
			{ new Point(1, 2), 10 },
			{ new Point(1, 3), 11 },
			{ new Point(1, 4), 12 },
			{ new Point(1, 5), 13 },
			{ new Point(1, 6), 14 },
			{ new Point(1, 7), 15 },

			{ new Point(2, 0), 16 },
			{ new Point(2, 1), 17 },
			{ new Point(2, 2), 18 },
			{ new Point(2, 3), 19 },
			{ new Point(2, 4), 20 },
			{ new Point(2, 5), 21 },
			{ new Point(2, 6), 22 },
			{ new Point(2, 7), 23 },

			{ new Point(3, 0), 24 },
			{ new Point(3, 1), 25 },
			{ new Point(3, 2), 26 },
			{ new Point(3, 3), 27 },
			{ new Point(3, 4), 28 },
			{ new Point(3, 5), 29 },
			{ new Point(3, 6), 30 },
			{ new Point(3, 7), 31 },

			{ new Point(4, 0), 32 },
			{ new Point(4, 1), 33 },
			{ new Point(4, 2), 34 },
			{ new Point(4, 3), 35 },
			{ new Point(4, 4), 36 },
			{ new Point(4, 5), 37 },
			{ new Point(4, 6), 38 },
			{ new Point(4, 7), 39 },

			{ new Point(5, 0), 40 },
			{ new Point(5, 1), 41 },
			{ new Point(5, 2), 42 },
			{ new Point(5, 3), 43 },
			{ new Point(5, 4), 44 },
			{ new Point(5, 5), 45 },
			{ new Point(5, 6), 46 },
			// ID 47 is the scythe - skipping

			{ new Point(6, 0), 48 },
			{ new Point(6, 1), 49 },
			{ new Point(6, 2), 50 },
			{ new Point(6, 3), 51 },
			{ new Point(6, 4), 52 },
		};

		public WeaponImageBuilder() : base()
		{
			BaseFileName = "weapons.png";
			SubDirectory = "Weapons";
			PositionsToOverlay = WeaponPositionToIDMap.Keys.ToList();

			SwordImages = Directory.GetFiles($"{ImageDirectory}/{SwordSubDirectory}").Where(x => x.EndsWith(".png")).ToList();
			DaggerImages = Directory.GetFiles($"{ImageDirectory}/{DaggerSubDirectory}").Where(x => x.EndsWith(".png")).ToList();
			HammerAndClubImages = Directory.GetFiles($"{ImageDirectory}/{HammerAndClubSubDirectory}").Where(x => x.EndsWith(".png")).ToList();
			SlingshotImages = Directory.GetFiles($"{ImageDirectory}/{SlingshotSubDirectory}").Where(x => x.EndsWith(".png")).ToList();
		}

		/// <summary>
		/// Gets a random file name that matches the weapon type at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(Point position)
		{
			switch (GetWeaponTypeFromPosition(position))
			{
				case WeaponType.SlashingSword:
				case WeaponType.StabbingSword:
					return Globals.RNGGetAndRemoveRandomValueFromList(SwordImages);
				case WeaponType.Dagger:
					//return Globals.RNGGetAndRemoveRandomValueFromList(DaggerImages);
					break;
				case WeaponType.ClubOrHammer:
					//return Globals.RNGGetAndRemoveRandomValueFromList(HammerAndClubImages);
					break;
				case WeaponType.Slingshot:
					//return Globals.RNGGetAndRemoveRandomValueFromList(SlingshotImages);
					break;

			}

			return $"{ImageDirectory}/default.png";

			//Globals.ConsoleError($"No weapon type defined at image position: {position.X}, {position.Y}");
			//return Globals.RNGGetAndRemoveRandomValueFromList(SwordImages);
		}

		/// <summary>
		/// Gets the weapon type from the given position in the image
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns />
		private WeaponType GetWeaponTypeFromPosition(Point position)
		{
			int weaponId = WeaponPositionToIDMap[position];
			WeaponItem weapon = WeaponRandomizer.Weapons[weaponId];
			return weapon.Type;
		}
	}
}