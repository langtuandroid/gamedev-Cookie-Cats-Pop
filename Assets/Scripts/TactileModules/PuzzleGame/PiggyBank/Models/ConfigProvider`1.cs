using System;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Models
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
