using System;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IDataProvider<T>
	{
		T Get();
	}
}
