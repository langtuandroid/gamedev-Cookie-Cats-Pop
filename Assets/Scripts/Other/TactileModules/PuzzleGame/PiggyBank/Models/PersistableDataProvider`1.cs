using System;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Models
{
	public class PersistableDataProvider<T> : IDataProvider<T> where T : IPersistableState<T>
	{
		public PersistableDataProvider(UserSettingsManager userSettingsManager)
		{
			this.userSettingsManager = userSettingsManager;
		}

		public T Get()
		{
			return this.userSettingsManager.GetSettings<T>();
		}

		private readonly UserSettingsManager userSettingsManager;
	}
}
