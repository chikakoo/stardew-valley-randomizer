using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Represents an item that is normally foragable
	/// </summary>
	public class ForagableItem : Item
	{
		/// <summary>
		/// The list of seasons this item is foragable in
		/// </summary>
		public List<Seasons> ForagableSeasons = new();

		public ForagableItem(ObjectIndexes index) : base(index)
		{
			ShouldBeForagable = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
			ItemsRequiredForRecipe = new Range(1, 3);
		}
	}
}
