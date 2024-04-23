using System;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public interface IDataProvider<T>
	{
		T Get();
	}
}
