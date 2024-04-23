using System;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	public class TreasureHuntSystem
	{
		public TreasureHuntSystem(TreasureHuntManager manager)
		{
			this.Manager = manager;
		}

		public TreasureHuntManager Manager { get; private set; }
	}
}
