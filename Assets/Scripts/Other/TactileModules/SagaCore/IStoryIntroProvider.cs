using System;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	public interface IStoryIntroProvider
	{
		IFlow CreateStoryFlow();
	}
}
