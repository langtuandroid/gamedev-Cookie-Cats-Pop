using System;

public abstract class LogicModule
{
	public virtual void Begin(LevelSession session)
	{
	}

	public virtual void TurnCompleted(LevelSession session)
	{
	}

	public virtual void End(LevelSession session)
	{
	}
}
