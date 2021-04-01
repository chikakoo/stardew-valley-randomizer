using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer
{
	class BigCraftable : Item
	{
		public BigCraftable(int id) : base(id)
		{
			IsBigCraftable = true;
			CanStack = false;
		}

		public BigCraftable(int id, ObtainingDifficulties difficultyToObtain) : this(id)
		{
			DifficultyToObtain = difficultyToObtain;
		}

	}
}
