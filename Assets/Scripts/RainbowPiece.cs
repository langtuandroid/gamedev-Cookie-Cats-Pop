using System;

public class RainbowPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return string.Empty;
		}
	}

	protected override bool RemovableAfterClusters()
	{
		return true;
	}
}
