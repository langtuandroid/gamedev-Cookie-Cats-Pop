using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	[Serializable]
	public class WheelSlot
	{
		public bool IsReward()
		{
			return this.stepsToAdd == 0;
		}

		public int stepsToAdd;

		public InventoryItem item;
	}
}
