using System.Collections.Generic;
using System.Linq;

namespace Randomizer;

/// <summary>
/// Spoiler-free log of all bundles for easy tracking
/// </summary>
public class BundleLogger : Logger
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="farmName">The name of the farm - used to easily identify the log</param>
	public BundleLogger(string farmName)
	{
		if (!Globals.Config.CreateBundleLog) { return; }

		PathPrefix = "BundleLog";
		InitializePath(farmName);
	}

	/// <summary>
	/// Writes text to the file, provided the settings allow it
	/// </summary>
	public override void WriteFile()
	{
		if (!Globals.Config.CreateBundleLog)
		{
			TextToWrite = "";
			return;
		}

		base.WriteFile();
	}

	/// <summary>
	/// Returns whether we should write to the bundle log
	/// - Checks the bundle/bundle log setting
	/// - We do NOT write the following bundles:
	/// - Vault - it's just money
	/// - Joja - it's kind of a spoiler on its own
	/// </summary>
	/// <param name="room">The room to write data for</param>
	/// <returns>True if we should write, false otherwise</returns>
	public static bool ShouldWriteToBundleLog(CommunityCenterRooms room)
	{
		return Globals.Config.Bundles.Randomize &&
			Globals.Config.CreateBundleLog &&
			room != CommunityCenterRooms.Vault &&
			room != CommunityCenterRooms.Joja;
	}

	/// <summary>
	/// Writes the room title to the bundle log
	/// - Skips if settings don't allow it, or if we aren't writing info for that room
	/// </summary>
	/// <param name="room">The room to write the title for</param>
	public void WriteRoomTitle(CommunityCenterRooms room)
	{
		if (!ShouldWriteToBundleLog(room))
		{
			return;
		}

		BufferLine($"-- {room} --");
	}

	/// <summary>
	/// Writes the given bundle, provided it's for a room we're writing for
	/// </summary>
	/// <param name="bundle">The bundle to write</param>
	public void WriteBundle(Bundle bundle)
	{
		if (!ShouldWriteToBundleLog(bundle.Room))
		{
			return;
		}

		int minimumRequiredItems = bundle.MinimumRequiredItems == default
			? bundle.RequiredItems.Count
			: bundle.MinimumRequiredItems.Value;
		BufferLine($"- {bundle.DisplayName} ({minimumRequiredItems}) -");

		bundle.RequiredItems.ForEach(item => WriteItem(item));

		BufferLine("");
	}

	/// <summary>
	/// Writes the given bundle required item to the spoiler log
	/// </summary>
	/// <param name="bundleItem">The item to write</param>
	public void WriteItem(RequiredBundleItem bundleItem)
	{
		var quantityString = bundleItem.NumberOfItems > 1
			? $"{bundleItem.NumberOfItems} "
			: string.Empty;
		var qualityString = GetQualityString(bundleItem.MinimumQuality);
		var extraInfoString = GetExtraInfoString(bundleItem.Item);

		BufferLine($"{quantityString}{qualityString}{bundleItem.Item.DisplayName}{extraInfoString}");
	}

	/// <summary>
	/// Gets a string indicating the quantity of the item
	/// </summary>
	/// <param name="quality">The quantity to get the string for</param>
	/// <returns>The string to log</returns>
	private static string GetQualityString(ItemQualities quality)
	{
		return quality switch
		{
			ItemQualities.Silver => "(S) ",
			ItemQualities.Gold => "(G) ",
			ItemQualities.Iridium => "(I) ",
			_ => string.Empty,
		};
	}

	/// <summary>
	/// Gets the extra info string to help with locating items
	/// (seasons|locations|fishing conditions)
	/// </summary>
	/// <param name="item">The item to get the string for</param>
	/// <returns>The extra info, or the empty string if there is none</returns>
	private static string GetExtraInfoString(Item item)
	{
		List<string> results = new()
		{
			GetSeasonString(item),
			GetLocationString(item),
			GetFishingConditionString(item)
		};
		results = results
			.Where(result => !string.IsNullOrEmpty(result))
			.ToList();

		return results.Any()
			? $" ({string.Join("|", results)})"
			: string.Empty;
	}

	/// <summary>
	/// Gets the seasons that the item is obtained in
	/// Only relevant for fish, foragables, and crops/seeds
	/// </summary>
	/// <param name="item">The item to get the season string for</param>
	/// <returns>The season string</returns>
	private static string GetSeasonString(Item item)
	{
		if (item is FishItem fishItem)
		{
			return GetSeasonString(fishItem.AvailableSeasons);
		}
		if (item is ForagableItem foragableItem)
		{
			return GetSeasonString(foragableItem.ForagableSeasons);
		}
		if (item is CropItem cropItem)
		{
			return GetSeasonString(cropItem.MatchingSeedItem.GrowingSeasons);
		}
		if (item is SeedItem seedItem)
		{
			return GetSeasonString(seedItem.GrowingSeasons);
		}
		return string.Empty;
	}

	/// <summary>
	/// Gets the season string - consists of season abbreviations next to each other
	/// </summary>
	/// <param name="seasons">The seasons to get the string for</param>
	/// <returns>The season string, * if all seasons are present</returns>
	private static string GetSeasonString(List<Seasons> seasons)
	{
		if (seasons.Count >= 4)
		{
			return "*";
		}

		string seasonString = string.Empty;
		seasons.ForEach(season => 
			seasonString += GetSeasonAbbreviation(season));
		return seasonString;
	}

	/// <summary>
	/// Gets a shorthand version of the given season
	/// </summary>
	/// <param name="season">The season</param>
	/// <returns>The shorthand version of the season</returns>
	private static string GetSeasonAbbreviation(Seasons season)
	{
		return season switch
		{
			Seasons.Spring => "Sp",
			Seasons.Summer => "Su",
			Seasons.Fall => "Fa",
			Seasons.Winter => "Wi",
			_ => string.Empty
		};
	}

	/// <summary>
	/// Gets the location string - consists of location abbreviations next to each other
	/// - This only applies to fish items
	/// - Excludes locations that will never be required for bundles
	/// - Also excludes locations that have no fishing spots
	/// </summary>
	/// <param name="item">The item to get the string for</param>
	/// <returns>The location string</returns>
	private static string GetLocationString(Item item)
	{
		if (item is not FishItem fishItem)
		{
			return string.Empty;
		}

		string locationString = string.Empty;
		fishItem.AvailableLocations.ForEach(loc =>
			locationString += GetFishingLocationAbbeviation(loc));

		// Replace the mine location with one that contains the floors instead
		if (locationString.Contains("Mine"))
		{
			string mineFloorString = fishItem.MineFloorString == default
				? FishItem.ComputeMineFloorString(fishItem.QualifiedId)
				: fishItem.MineFloorString;
			string mineFloorDisplayString = $"M({mineFloorString.Replace(" ", "")})";

			locationString = locationString.Replace("Mine", mineFloorDisplayString);
		}
		
		return locationString;
	}

	/// <summary>
	/// Gets the abbreviation for a fishing location that is 
	/// accessible pre-community center completion
	/// </summary>
	/// <param name="location"></param>
	/// <returns></returns>
	private static string GetFishingLocationAbbeviation(Locations location)
	{
		return location switch
		{
			Locations.Desert => "Ds",
			Locations.Forest => "For",
			Locations.Town => "Tn",
			Locations.Mountain => "Mt",
			Locations.Beach => "O",
			Locations.Submarine => "Of",
			Locations.Woods => "Wd",
			Locations.Sewer => "Sew",
			Locations.UndergroundMine => "Mine",
			_ => string.Empty
		};
	}

	/// <summary>
	/// Gets the fishing condition string for a FishItem
	/// - This contains whether you can only catch the fish in the rain or sun
	/// </summary>
	/// <param name="item">The item to get the string for</param>
	/// <returns>The fishing condition string</returns>
	private static string GetFishingConditionString(Item item)
	{
		if (item is not FishItem fishItem)
		{
			return string.Empty;
		}

		return fishItem.Weathers.Count == 1
			? GetWeatherAbbreviation(fishItem.Weathers[0])
			: string.Empty;
	}

	/// <summary>
	/// Gets a shorthand version of the given weather
	/// </summary>
	/// <param name="weather">The weather</param>
	/// <returns>The shorthand version of the weather</returns>
	private static string GetWeatherAbbreviation(Weather weather)
	{
		return weather switch
		{
			Weather.Sunny => "Sun",
			Weather.Rainy => "Rain",
			_ => string.Empty
		};
	}
}
