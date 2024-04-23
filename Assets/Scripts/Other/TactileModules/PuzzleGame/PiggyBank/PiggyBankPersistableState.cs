using System;
using Tactile;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank
{
	[SettingsProvider("pb", false, new Type[]
	{

	})]
	public class PiggyBankPersistableState : IPersistableState<PiggyBankPersistableState>, IPersistableState
	{
		public PiggyBankPersistableState()
		{
			this.Reset();
		}

		[JsonSerializable("sl", null)]
		public int StartingLevel { get; set; }

		[JsonSerializable("nf", null)]
		public int FreeOpenLevel { get; set; }

		[JsonSerializable("cc", null)]
		public int CollectedCoins { get; set; }

		[JsonSerializable("ca", null)]
		public int Capacity { get; set; }

		[JsonSerializable("ts", null)]
		public bool TutorialShown { get; set; }

		[JsonSerializable("lu", null)]
		public bool UnlockVisualsHandled { get; set; }

		public void Reset()
		{
			this.StartingLevel = -1;
			this.FreeOpenLevel = -1;
			this.CollectedCoins = 0;
			this.Capacity = 0;
			this.TutorialShown = false;
			this.UnlockVisualsHandled = false;
		}

		public void MergeFromOther(PiggyBankPersistableState newest, PiggyBankPersistableState last)
		{
			this.MergeOtherIntoThis(newest);
		}

		public void MergeOtherIntoThis(PiggyBankPersistableState other)
		{
			if (this.FreeOpenLevel == other.FreeOpenLevel)
			{
				this.StartingLevel = Mathf.Max(this.StartingLevel, other.StartingLevel);
				this.FreeOpenLevel = Mathf.Max(this.FreeOpenLevel, other.FreeOpenLevel);
				this.CollectedCoins = Mathf.Max(this.CollectedCoins, other.CollectedCoins);
			}
			else if (this.FreeOpenLevel < other.FreeOpenLevel)
			{
				this.StartingLevel = other.StartingLevel;
				this.FreeOpenLevel = other.FreeOpenLevel;
				this.CollectedCoins = other.CollectedCoins;
			}
			this.Capacity = Mathf.Max(this.Capacity, other.Capacity);
			this.UnlockVisualsHandled = (this.UnlockVisualsHandled || other.UnlockVisualsHandled);
			this.TutorialShown = (this.TutorialShown || other.TutorialShown);
		}

		public bool DataEqual(PiggyBankPersistableState other)
		{
			return this.StartingLevel == other.StartingLevel && this.FreeOpenLevel == other.FreeOpenLevel && this.CollectedCoins == other.CollectedCoins && this.Capacity == other.Capacity && this.TutorialShown == other.TutorialShown && this.UnlockVisualsHandled == other.UnlockVisualsHandled;
		}
	}
}
