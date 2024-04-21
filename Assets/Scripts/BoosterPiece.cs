using System;
using UnityEngine;

public class BoosterPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return this.matchColor;
		}
	}

	public override bool IsBasicPiece
	{
		get
		{
			return true;
		}
	}

	[SerializeField]
	protected MatchFlag matchColor;
}
