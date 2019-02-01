﻿namespace Randomizer
{
	/// <summary>
	/// Represents an item that is normally foragable
	/// </summary>
	public class ForagableItem : Item
	{
		public ForagableItem(int id) : base(id)
		{
			ShouldBeForagable = true;
		}
	}
}
