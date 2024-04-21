using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.ReengagementRewards
{
	public interface IReengagementDataProvider
	{
		event Action<int> OnLevelComplete;

		PersistableState PersistableState { get; }

		Config Config { get; }

		void Save();

		void Reset();

		void ClaimReward(List<ItemAmount> configReward);

		IEnumerator UnlockCurrentGate();

		void UpdateGate(int levelIndex);

		LevelProxy GetCurrentGate();
	}
}
