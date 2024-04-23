using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	public class FeatureDataProvider<T> : IDataProvider<T>
	{
		public FeatureDataProvider(ISlidesAndLaddersHandler featureHandler)
		{
			this.featureHandler = featureHandler;
		}

		public T Get()
		{
			return this.featureHandler.GetDataInstance<T>();
		}

		private ISlidesAndLaddersHandler featureHandler;
	}
}
