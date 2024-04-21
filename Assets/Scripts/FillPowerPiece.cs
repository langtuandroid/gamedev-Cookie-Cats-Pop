using System;
using UnityEngine;

public class FillPowerPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return this.matchColor;
		}
	}

	[SerializeField]
	private MatchFlag matchColor;
}
