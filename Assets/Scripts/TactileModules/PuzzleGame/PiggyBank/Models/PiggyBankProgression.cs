using System;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Models
{
	public class PiggyBankProgression : IPiggyBankProgression
	{
		public PiggyBankProgression(IPiggyBankProvider provider, IMainProgression mainProgression, IDataProvider<PiggyBankPersistableState> persistableData, IDataProvider<PiggyBankConfig> config)
		{
			this.provider = provider;
			this.mainProgression = mainProgression;
			this.persistableData = persistableData;
			this.config = config;
		}

		public int StartingLevel
		{
			get
			{
				return this.persistableData.Get().StartingLevel;
			}
			private set
			{
				this.persistableData.Get().StartingLevel = value;
			}
		}

		public bool TutorialShown
		{
			get
			{
				return this.persistableData.Get().TutorialShown;
			}
			private set
			{
				this.persistableData.Get().TutorialShown = value;
			}
		}

		public void ShowedTutorial()
		{
			this.TutorialShown = true;
		}

		public bool IsAvailableOnLevel(int levelHumanNumber)
		{
			return levelHumanNumber >= this.config.Get().LevelRequired;
		}

		public bool IsNextFreeOpenReady()
		{
			return this.mainProgression.GetFarthestUnlockedLevelHumanNumber() >= this.GetNextFreeOpenLevelHumanNumber();
		}

		public bool IsFeatureEnabled()
		{
			return this.config.Get().Enabled;
		}

		public void SetStartingLevel(int startLevel)
		{
			this.StartingLevel = startLevel;
		}

		public int GetNextFreeOpenLevelHumanNumber()
		{
			if (this.persistableData.Get().FreeOpenLevel < 0)
			{
				return this.StartingLevel + this.config.Get().InitialInterval;
			}
			return this.persistableData.Get().FreeOpenLevel + this.config.Get().Interval;
		}

		public void UpdateToNextFreeLevel()
		{
			this.persistableData.Get().FreeOpenLevel = this.GetNextFreeOpenLevelHumanNumber();
		}

		public bool HasHandledUnlockVisuals()
		{
			return this.persistableData.Get().UnlockVisualsHandled;
		}

		public void UnlockVisualsHandled()
		{
			this.persistableData.Get().UnlockVisualsHandled = true;
		}

		public void ResetUnlockVisualsHandled()
		{
			this.persistableData.Get().UnlockVisualsHandled = false;
		}

		public void SavePersistabelState()
		{
			this.provider.SavePersistableState();
		}

		private readonly IPiggyBankProvider provider;

		private readonly IMainProgression mainProgression;

		private readonly IDataProvider<PiggyBankPersistableState> persistableData;

		private readonly IDataProvider<PiggyBankConfig> config;
	}
}
