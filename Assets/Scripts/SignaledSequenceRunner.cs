using System;
using System.Collections;

public class SignaledSequenceRunner
{
	public SignaledSequenceRunner(Func<IEnumerator> sequence, Func<bool> prerequisiteFunction)
	{
		this.sequence = sequence;
		this.prerequisiteFunction = prerequisiteFunction;
	}

	public void SetSignal()
	{
		this.signalSet = true;
	}

	public IEnumerator TryRun()
	{
		if (this.signalSet && this.prerequisiteFunction())
		{
			this.signalSet = false;
			yield return this.sequence();
		}
		yield break;
	}

	private readonly Func<IEnumerator> sequence;

	private readonly Func<bool> prerequisiteFunction;

	private bool signalSet;
}
