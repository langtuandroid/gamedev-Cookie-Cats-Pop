using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Providers
{
	public interface ILevelDashViewProvider
	{
		IEnumerator ShowRewardView(LevelDashConfig.Reward reward);

		IEnumerator ShowRewardViewForPreviousLevelDash(int rank, LevelDashConfig.Reward reward);

		void InitializeLeaderboardItem(Entry entry, GameObject go);
	}
}
