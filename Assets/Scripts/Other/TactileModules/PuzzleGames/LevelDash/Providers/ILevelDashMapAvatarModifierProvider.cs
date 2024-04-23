using System;

namespace TactileModules.PuzzleGames.LevelDash.Providers
{
	public interface ILevelDashMapAvatarModifierProvider
	{
		string GetAvatarLeaderIconPrefabPath(int rank);
	}
}
