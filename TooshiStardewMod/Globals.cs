﻿using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Used for any global access - USE SPARINGLY
	/// </summary>
	public class Globals
	{
		public static Mod ModRef { get; set; }
		public static Random RNG { get; set; }

		/// <summary>
		/// A shortcut to write to the console
		/// </summary>
		/// <param name="input">The input string</param>
		public static void ConsoleWrite(string input)
		{
			ModRef.Monitor.Log(input);
		}

		/// <summary>
		/// Gets a random boolean value
		/// </summary>
		/// <returns />
		public static bool RNGGetNextBoolean()
		{
			return RNG.Next(0, 2) == 0;
		}

		public static bool RNGGetNextBoolean(int percentage)
		{
			if (percentage < 0 || percentage > 100) Globals.ConsoleWrite("WARNING: Percentage is invalid (less than 0 or greater than 100)");
			return RNG.Next(0, 100) < percentage;
		}

		/// <summary>
		/// Gets a random value out of the given list
		/// </summary>
		/// <typeparam name="T">The type of the list</typeparam>
		/// <param name="list">The list</param>
		/// <returns />
		public static T RNGGetRandomValueFromList<T>(List<T> list)
		{
			if (list == null || list.Count == 0)
			{
				ConsoleWrite("ERROR: Attempted to get a random value out of an empty list!");
				return default(T);
			}

			return list[RNG.Next(list.Count)];
		}

		/// <summary>
		/// Gets a random value out of the given list and removes it
		/// </summary>
		/// <typeparam name="T">The type of the list</typeparam>
		/// <param name="list">The list</param>
		/// <returns />
		public static T RNGGetAndRemoveRandomValueFromList<T>(List<T> list)
		{
			if (list == null || list.Count == 0)
			{
				ConsoleWrite("ERROR: Attempted to get a random value out of an empty list!");
				return default(T);
			}
			int selectedIndex = RNG.Next(list.Count);
			T selectedValue = list[selectedIndex];
			list.RemoveAt(selectedIndex);
			return selectedValue;
		}

		/// <summary>
		/// Gets a random set of values form a list
		/// </summary>
		/// <typeparam name="T">The type of the list</typeparam>
		/// <param name="inputList">The list</param>
		/// <param name="numberOfvalues">The number of values to return</param>
		/// <returns>
		/// The randomly chosen values - might be less than the number of values if the list doesn't contain that many
		/// </returns>
		public static List<T> RNGGetRandomValuesFromList<T>(List<T> inputList, int numberOfvalues)
		{
			List<T> listToChooseFrom = new List<T>(inputList); // Don't modify the original list
			List<T> randomValues = new List<T>();
			if (listToChooseFrom == null || listToChooseFrom.Count == 0)
			{
				ConsoleWrite("ERROR: Attempted to get random values out of an empty list!");
				return randomValues;
			}

			int numberOfIterations = Math.Min(numberOfvalues, listToChooseFrom.Count);
			for (int i = 0; i < numberOfIterations; i++)
			{
				randomValues.Add(RNGGetAndRemoveRandomValueFromList(listToChooseFrom));
			}

			return randomValues;
		}
	}
}
