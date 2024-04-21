using System;
using System.Collections;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface IMainMapPlugin : IMapPlugin
	{
		IEnumerator DropPresents();

		void RebuildPresents();
	}
}
