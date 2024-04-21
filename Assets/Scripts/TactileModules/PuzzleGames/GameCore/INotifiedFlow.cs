using System;
using Fibers;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface INotifiedFlow : IFlow, IFiberRunnable
	{
		void Enter(IFlow previousFlow);

		void Leave(IFlow nextFlow);
	}
}
