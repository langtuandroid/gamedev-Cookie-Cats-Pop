using System;
using UnityEngine;

public class CPBoosterMetaData : BoosterMetaData
{
	public BoosterLogic LogicPrefab
	{
		get
		{
			return this.logicPrefab;
		}
	}

	public bool IsThrownAsBall
	{
		get
		{
			return this.isThrownAsBall;
		}
	}

	[SerializeField]
	private BoosterLogic logicPrefab;

	[SerializeField]
	private bool pregame;

	[SerializeField]
	private bool isThrownAsBall;
}
