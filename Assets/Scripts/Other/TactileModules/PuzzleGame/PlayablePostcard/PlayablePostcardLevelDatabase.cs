using System;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.PlayablePostcard.Model;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class PlayablePostcardLevelDatabase : LevelDatabase
	{
		public IPlayablePostcardProvider PlayablePostcardProvider
		{
			get
			{
				return ManagerRepository.Get<PlayablePostcardSystem>().Provider;
			}
		}

		public override void RemoveLevelData(LevelProxy levelProxy)
		{
		}

		public override double GetGateProgress(LevelProxy levelProxy)
		{
			return 0.0;
		}

		public override LevelProxy GetLevel(int levelIndex)
		{
			if (levelIndex < 0)
			{
				return LevelProxy.Invalid;
			}
			return new LevelProxy(this, new int[]
			{
				levelIndex
			});
		}

		public override string GetAnalyticsDescriptor()
		{
			return "playablePostcard";
		}

		public override MapIdentifier GetMapAndLevelsIdentifier()
		{
			return "PlayablePostcard";
		}

		public override string GetPersistedKey(LevelProxy levelProxy)
		{
			return "DummyEntryPlayablePostcard";
		}

		public override void Save()
		{
			this.PlayablePostcardProvider.Save();
		}

		public override ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
		{
			return null;
		}
	}
}
