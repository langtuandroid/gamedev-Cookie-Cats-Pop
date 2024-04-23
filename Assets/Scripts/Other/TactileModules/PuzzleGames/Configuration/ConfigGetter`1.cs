using System;

namespace TactileModules.PuzzleGames.Configuration
{
	public class ConfigGetter<T> : IConfigGetter<T>
	{
		public ConfigGetter(IConfigurationManager configurationManager)
		{
			this.configurationManager = configurationManager;
		}

		public T Get()
		{
			return this.configurationManager.GetConfig<T>();
		}

		private readonly IConfigurationManager configurationManager;
	}
}
