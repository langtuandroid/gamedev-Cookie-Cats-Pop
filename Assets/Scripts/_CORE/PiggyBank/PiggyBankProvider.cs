using System;
using System.Diagnostics;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

public class PiggyBankProvider : IPiggyBankProvider
{
	public PiggyBankProvider(UserSettingsManager userSettingsManager)
	{
		this.userSettingsManager = userSettingsManager;
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action LevelPlayed;



	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action InGameBoosterUsed;



	public void RegisterLevelSession(LevelSession session)
	{
		session.StateChanged += this.SessionStateChangedHandler;
		session.Ended += this.SessionEndedHandler;
		session.InGameBoosterUsed += this.OnInGameBoosterUsed;
	}

	private void OnInGameBoosterUsed()
	{
		this.InGameBoosterUsed();
	}

	private void SessionStateChangedHandler(LevelSession session)
	{
		if (this.LevelCompletedOrFailed(session))
		{
			this.LevelPlayed();
		}
	}

	private bool LevelCompletedOrFailed(ILevelSession session)
	{
		return session.SessionState == LevelSessionState.Completed || session.SessionState == LevelSessionState.Failed;
	}

	private void SessionEndedHandler(LevelSession session)
	{
		session.StateChanged -= this.SessionStateChangedHandler;
		session.Ended -= this.SessionEndedHandler;
		session.InGameBoosterUsed -= this.OnInGameBoosterUsed;
	}

	public void SavePersistableState()
	{
		this.userSettingsManager.SaveLocalSettings();
	}

	private readonly UserSettingsManager userSettingsManager;
}
