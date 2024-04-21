using System;

public static class PuzzleGame
{
	public static IPlayerState PlayerState
	{
		get
		{
			return PuzzleGame.implementor.PlayerState;
		}
	}

	public static IDialogViewProvider DialogViews
	{
		get
		{
			return PuzzleGame.implementor.DialogViews;
		}
	}

	public static PuzzleGame.ILevelFlow LevelFlow
	{
		get
		{
			return PuzzleGame.implementor.LevelFlow;
		}
	}

	public static PuzzleGame.IUserSettings UserSettings
	{
		get
		{
			return PuzzleGame.implementor.UserSettings;
		}
	}

	public static PuzzleGame.ISyncPoints SyncPoints
	{
		get
		{
			return PuzzleGame.implementor.SyncPoints;
		}
	}

	public static void Initialize<T>() where T : PuzzleGame.IPuzzleGame
	{
		PuzzleGame.implementor = (PuzzleGame.IPuzzleGame)Activator.CreateInstance(typeof(T), true);
	}

	private static PuzzleGame.IPuzzleGame implementor;

	public interface IPuzzleGame
	{
		IPlayerState PlayerState { get; }

		IDialogViewProvider DialogViews { get; }

		PuzzleGame.ILevelFlow LevelFlow { get; }

		PuzzleGame.IUserSettings UserSettings { get; }

		PuzzleGame.ISyncPoints SyncPoints { get; }
	}

	public interface ILevelFlow
	{
		event Action<LevelProxy> LevelUnlocked;
	}

	public interface IUserSettings
	{
		void SaveLocalAndSyncCloud();

		void SaveLocal();
	}

	public interface ISyncPoints
	{
		event Action OnApplicationWillEnterForeground;

		event Action OnFadedToBlack;
	}
}
