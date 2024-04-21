using System;

namespace TactileModules.PuzzleGames.Configuration
{
	public interface IConfigGetter<T>
	{
		T Get();
	}
}
