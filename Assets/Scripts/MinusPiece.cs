using System;
using UnityEngine;

public class MinusPiece : CPPiece
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
