using System;

public static class PuzzleGameData
{
	public static IPlayerState PlayerState
	{
		get
		{
			return PuzzleGameData.implementor.PlayerState;
		}
	}

	public static IDialogViewProvider DialogViews
	{
		get
		{
			return PuzzleGameData.implementor.DialogViews;
		}
	}

	public static PuzzleGameData.ILevelFlow LevelFlow
	{
		get
		{
			return PuzzleGameData.implementor.LevelFlow;
		}
	}

	public static PuzzleGameData.IUserSettings UserSettings
	{
		get
		{
			return PuzzleGameData.implementor.UserSettings;
		}
	}

	public static PuzzleGameData.ISyncPoints SyncPoints
	{
		get
		{
			return PuzzleGameData.implementor.SyncPoints;
		}
	}

	public static void Initialize<T>() where T : PuzzleGameData.IPuzzleGame
	{
        PuzzleGameData.implementor = (PuzzleGameData.IPuzzleGame)Activator.CreateInstance(typeof(T), true);
        UnityEngine.Debug.Log("PuzzleGameData.implementor " + PuzzleGameData.implementor.PlayerState); 
	}

	private static PuzzleGameData.IPuzzleGame implementor;

	public interface IPuzzleGame
	{
		IPlayerState PlayerState { get; }

		IDialogViewProvider DialogViews { get; }

        PuzzleGameData.ILevelFlow LevelFlow { get; }

        PuzzleGameData.IUserSettings UserSettings { get; }

        PuzzleGameData.ISyncPoints SyncPoints { get; }
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
