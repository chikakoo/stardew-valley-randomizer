﻿using System.Collections.Generic;

namespace Randomizer
{
	public class Penny : NPC
	{
		public static List<Item> Loves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.Diamond],
			ItemList.Items[(int)ObjectIndexes.Emerald],
			//ItemList.Items[(int)ObjectIndexes.Melon], //TODO: when crops done
			//ItemList.Items[(int)ObjectIndexes.Poppy], //TODO: when cops done
			ItemList.Items[(int)ObjectIndexes.PoppyseedMuffin],
			ItemList.Items[(int)ObjectIndexes.RedPlate],
			ItemList.Items[(int)ObjectIndexes.RootsPlatter],
			ItemList.Items[(int)ObjectIndexes.Sandfish],
			ItemList.Items[(int)ObjectIndexes.TomKhaSoup]
		};
	}
}
