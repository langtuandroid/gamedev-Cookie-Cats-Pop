using System;
using TactileModules.PuzzleGame.PlayablePostcard.Model;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class FeatureDataProvider<T> : IDataProvider<T>
	{
		public FeatureDataProvider(PlayablePostcardHandler featureHandler)
		{
			this.featureHandler = featureHandler;
		}

		public T Get()
		{
			if (this.featureHandler.InstanceCustomData.GetType() == typeof(T))
			{
				return (T)((object)this.featureHandler.InstanceCustomData);
			}
			if (this.featureHandler.MetaData.GetType() == typeof(T))
			{
				return (T)((object)this.featureHandler.MetaData);
			}
			return default(T);
		}

		private PlayablePostcardHandler featureHandler;
	}
}
