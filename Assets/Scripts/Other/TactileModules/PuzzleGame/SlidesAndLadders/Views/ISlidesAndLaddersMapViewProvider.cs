using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface ISlidesAndLaddersMapViewProvider
	{
		LevelDatabaseCollection LevelDatabaseCollection { get; }

		MapStreamerCollection MapStreamerCollection { get; }

		void SubscribeToFriendsAndSettingsSynced(Action callback);

		void UnsubscribeToFriendsAndSettingsSynced(Action callback);
	}
}
