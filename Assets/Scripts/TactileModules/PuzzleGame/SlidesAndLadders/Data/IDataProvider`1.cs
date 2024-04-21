using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	public interface IDataProvider<T>
	{
		T Get();
	}
}
