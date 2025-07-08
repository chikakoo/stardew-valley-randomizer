namespace Randomizer
{
    /// <summary>
    /// Represents items that you get from raising animals
    /// </summary>
    public class AnimalItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="index">The id of the item</param>
		public AnimalItem(ObjectIndexes index, 
			ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.LargeTimeRequirements) 
			: base(index)
		{
			IsAnimalProduct = true;
			DifficultyToObtain = difficultyToObtain;
		}
	}
}
