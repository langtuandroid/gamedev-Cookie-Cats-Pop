using System;
using System.Diagnostics;

public class GameEventManager
{
	private GameEventManager()
	{
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<GameEvent> OnGameEvent;

	public static GameEventManager Instance { get; private set; }

	public static GameEventManager CreateInstance()
	{
		GameEventManager.Instance = new GameEventManager();
		return GameEventManager.Instance;
	}

	public void EmitSafe(GameEvent e)
	{
		if (this.OnGameEvent != null)
		{
			this.OnGameEvent(e);
		}
	}

	public void Emit(GameEventType eventType)
	{
		this.Emit(eventType, null, 1);
	}

	public void Emit(GameEventType eventType, object context, int value = 1)
	{
		if (this.sharedEventInProgress)
		{
		}
		this.sharedEventInProgress = true;
		this.reusableEvent.type = eventType;
		this.reusableEvent.context = context;
		this.reusableEvent.value = value;
		this.EmitSafe(this.reusableEvent);
		this.sharedEventInProgress = false;
	}

	private GameEvent reusableEvent = new GameEvent();

	private bool sharedEventInProgress;
}
