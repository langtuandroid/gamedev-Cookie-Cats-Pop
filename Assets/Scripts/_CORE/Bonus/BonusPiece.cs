using System;
using UnityEngine;

public class BonusPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return this.matchColor;
		}
	}

	[SerializeField]
	protected MatchFlag matchColor;
}
