using System;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament
{
	public interface IStarTournamentProvider
	{
		event Action OnNewSessionStarted;

		SideButtonsArea GetSideButtonsArea();

		GameObject GetStarPrefab();

		int GetStarsForLevel(int levelId);

		string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
	}
}
