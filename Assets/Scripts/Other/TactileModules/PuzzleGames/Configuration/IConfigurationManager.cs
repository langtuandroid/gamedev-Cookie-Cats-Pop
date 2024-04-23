using System;

namespace TactileModules.PuzzleGames.Configuration
{
	public interface IConfigurationManager
	{
		event Action ConfigurationUpdated;

		T GetConfig<T>();

		int GetVersion();
	}
}
