using System;
using System.Collections.Generic;

namespace Randomizer
{
    /// <summary>
    /// Represents an item drop - contains an item and probability
    /// </summary>
    public class ItemDrop
	{
		public Item ItemToDrop { get; set; }
		public double Probability { get; set; }

		public ItemDrop(ObjectIndexes itemId, double probability)
		{
			ItemToDrop = itemId.GetItem();
            Probability = probability;
		}

        public ItemDrop(string stringId, double probability)
        {
			ItemToDrop = new Item(stringId)
			{
                OverrideName = stringId
			};
            Probability = probability;
        }

        /// <summary>
        /// Parses an item drop string into a list of item drops
        /// - The value -4 is parsed as coal
        /// - The value -6 is parsed as gold ore
		/// 
		/// See the MonsterData Initialize function for more
        /// </summary>
        /// <param name="itemDropString">The string to parse</param>
        /// <returns />
        public static List<ItemDrop> ParseString(string itemDropString)
		{
			List<ItemDrop> itemDrops = new();

			string[] itemTokens = itemDropString.Split(' ');
			for (int i = 0; i + 1 < itemTokens.Length; i += 2)
			{
				ObjectIndexes? itemIndex = default;
				string itemId = itemTokens[i].Trim();

                if (itemId == "-4")
                {
                    itemIndex = ObjectIndexes.Coal;
                }
                else if (itemId == "-6")
                {
                    itemIndex = ObjectIndexes.GoldOre;
                }
				else
				{
                    try
                    {
                        itemIndex = ObjectIndexesExtentions.GetObjectIndex(itemId);
                    }
                    catch (Exception)
                    {
                        Globals.ConsoleWarn($"Object index not found when parsing monster item drop in string (expected in some mods): {itemDropString}");
                    }
                }

				if (!double.TryParse(itemTokens[i + 1], out double probability))
				{
					Globals.ConsoleError($"Invalid token when parsing monster item probability in string: {itemDropString}");
					probability = 0.75;
				}

				if (itemIndex.HasValue)
				{
                    itemDrops.Add(new ItemDrop(itemIndex.Value, probability));
                } 
				else
				{
                    itemDrops.Add(new ItemDrop(itemId, probability));
                }
			}

			return itemDrops;
		}

		/// <summary>
		/// The string format to use in output dictionaries
		/// </summary>
		/// <returns>String formatted like: "Id Probability" (e.g. 72 0.01)</returns>
		public override string ToString()
		{
			return $"{ItemToDrop.Id} {Probability}";
		}
	}
}
