using System;

namespace TactileModules.PuzzleGames.GameCore
{
	public class ChangeInfo
	{
		public ChangeInfo(IFullScreenOwner previousOwner, IFullScreenOwner nextOwner)
		{
			this.PreviousOwner = previousOwner;
			this.NextOwner = nextOwner;
		}

		public IFullScreenOwner PreviousOwner { get; private set; }

		public IFullScreenOwner NextOwner { get; private set; }
	}
}
