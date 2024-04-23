using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public interface ISlidesAndLaddersMapViewProvider
	{
		LevelDatabaseCollection LevelDatabaseCollection { get; }

		MapStreamerCollection MapStreamerCollection { get; }

		void SubscribeToFriendsAndSettingsSynced(Action callback);

		void UnsubscribeToFriendsAndSettingsSynced(Action callback);

		void SubscribeToVIPStateChange(Action<bool> callback);

		void UnsubscribeToVIPStateChange(Action<bool> callback);
	}
}
