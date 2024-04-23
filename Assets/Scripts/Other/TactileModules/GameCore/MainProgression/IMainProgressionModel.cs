using System;

namespace TactileModules.GameCore.MainProgression
{
	public interface IMainProgressionModel
	{
		event Action ProgressChanged;

		int Progress { get; }
	}
}
