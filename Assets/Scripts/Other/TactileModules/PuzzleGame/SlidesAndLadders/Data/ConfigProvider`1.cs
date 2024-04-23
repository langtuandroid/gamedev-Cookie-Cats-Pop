using System;
using Tactile;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	public class ConfigProvider<T> : IDataProvider<T>
	{
		public ConfigProvider(ConfigurationManager configurationManager)
		{
			this.configurationManager = configurationManager;
		}

		public T Get()
		{
			return this.configurationManager.GetConfig<T>();
		}

		private readonly ConfigurationManager configurationManager;
	}
}
