using System;
using TactileModules.PuzzleGames.LevelDash.Providers;

public class LevelDashMapAvatarModifierProvider : ILevelDashMapAvatarModifierProvider
{
	public string GetAvatarLeaderIconPrefabPath(int rank)
	{
		return string.Format("LevelDash/LevelDashTrophyIcon{0}", rank);
	}
}
