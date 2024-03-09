﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using StardewValley.GameData.Weapons;

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
		/// The number of items per row in the weapon image file
		/// </summary>
		protected const int ItemsPerRow = 8;

		/// <summary>
		/// A map of the weapon position in the dictionary to the id it belongs to
		/// </summary>
		private Dictionary<Point, int> WeaponPositionToIDMap;

		public WeaponImageBuilder() : base()
		{
            Rng = RNG.GetFarmRNG(nameof(WeaponImageBuilder));
            StardewAssetPath = "TileSheets/weapons";
            SubDirectory = "Weapons";
			SetUpWeaponPositionToIDMap();
			PositionsToOverlay = WeaponPositionToIDMap.Keys.ToList();

			SwordImages = Directory.GetFiles(Path.Combine(ImageDirectory, SwordSubDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x).
				ToList();
			DaggerImages = Directory.GetFiles(Path.Combine(ImageDirectory, DaggerSubDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();
			HammerAndClubImages = Directory.GetFiles(Path.Combine(ImageDirectory, HammerAndClubSubDirectory))
				.Where(x => x.EndsWith(".png"))
				.OrderBy(x => x)
				.ToList();

			//TODO: enable this when we actually randomize slingshot images
			//SlingshotImages = Directory.GetFiles(Path.Combine(ImageDirectory, SlingshotSubDirectory))
			//	.Where(x => x.EndsWith(".png"))
			//	.OrderBy(x => x)
			//	.ToList();
		}

		/// <summary>
		/// Sets up the weapon ID map
		/// </summary>
		private void SetUpWeaponPositionToIDMap()
		{
			WeaponPositionToIDMap = new Dictionary<Point, int>();
			foreach (string stringKey in WeaponRandomizer.Weapons.Keys)
			{
				int id = int.Parse(stringKey);
				WeaponPositionToIDMap[GetPointFromId(id)] = id;
			}
		}

		/// <summary>
		/// Gets the point in the weapons file that belongs to the given item id
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns />
		protected static Point GetPointFromId(int id)
		{
			return new Point(id % ItemsPerRow, id / ItemsPerRow);
		}

		/// <summary>
		/// Gets a random file name that matches the weapon type at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(Point position)
		{
			string fileName = "";
			switch (GetWeaponTypeFromPosition(position))
			{
				case WeaponType.SlashingSword:
				case WeaponType.StabbingSword:
					fileName = Rng.GetAndRemoveRandomValueFromList(SwordImages);
					break;
				case WeaponType.Dagger:
					fileName = Rng.GetAndRemoveRandomValueFromList(DaggerImages);
					break;
				case WeaponType.ClubOrHammer:
					fileName = Rng.GetAndRemoveRandomValueFromList(HammerAndClubImages);
					break;
				case WeaponType.Slingshot:
					// TODO:Use slingshot images when we actually randomize them
					break;
				default:
					Globals.ConsoleError($"No weapon type defined at image position: {position.X}, {position.Y}");
					break;

			}

			if (string.IsNullOrEmpty(fileName))
			{
				Globals.ConsoleWarn($"Using default image for weapon at image position - you may not have enough weapon images: {position.X}, {position.Y}");
				return null;
			}
			return fileName;
		}

		/// <summary>
		/// Gets the weapon type from the given position in the image
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns />
		private WeaponType GetWeaponTypeFromPosition(Point position)
		{
			int weaponId = WeaponPositionToIDMap[position];
			WeaponData weapon = WeaponRandomizer.Weapons[weaponId.ToString()];
			return (WeaponType)weapon.Type;
		}

		/// <summary>
		/// Whether the settings premit random weapon images
		/// </summary>
		/// <returns>True if so, false otherwise</returns>
		public override bool ShouldSaveImage()
		{
			return Globals.Config.Weapons.UseCustomImages;
		}
	}
}
