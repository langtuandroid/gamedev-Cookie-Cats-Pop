using System;

public class GameEvent
{
	public override string ToString()
	{
		return string.Format("[GameEvent: {0}]", this.type);
	}

	public GameEventType type;

	public int value;

	public object context;
}
