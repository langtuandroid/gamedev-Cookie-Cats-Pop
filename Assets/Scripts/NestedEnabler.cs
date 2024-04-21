using System;

public struct NestedEnabler
{
	public NestedEnabler(bool enabled)
	{
		this.disabledCounter = ((!enabled) ? 1 : 0);
	}

	private void Push()
	{
		this.disabledCounter++;
	}

	private void Pop()
	{
		if (this.disabledCounter > 0)
		{
			this.disabledCounter--;
		}
	}

	public bool Enabled
	{
		get
		{
			return this.disabledCounter == 0;
		}
	}

	public static implicit operator bool(NestedEnabler nestedBoolean)
	{
		return nestedBoolean.Enabled;
	}

	public static NestedEnabler operator +(NestedEnabler nestedBoolean, bool enable)
	{
		if (enable)
		{
			nestedBoolean.Pop();
		}
		else
		{
			nestedBoolean.Push();
		}
		return nestedBoolean;
	}

	private int disabledCounter;
}
