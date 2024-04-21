using System;
using UnityEngine;

[Serializable]
public class LevelConnection : ILevelConnection
{
	public int CompletedLevelSteps
	{
		get
		{
			return this.completedLevelSteps;
		}
	}

	public int FailedLevelSteps
	{
		get
		{
			return this.failedLevelSteps;
		}
	}

	public bool IsTreasureLevel
	{
		get
		{
			return this.treasureLevel;
		}
	}

	public int SessionResultSteps(bool isLevelCompleted)
	{
		return (!isLevelCompleted) ? this.FailedLevelSteps : this.CompletedLevelSteps;
	}

	public ILevelProxy NextLevel(ILevelProxy current, bool isLevelCompleted)
	{
		int num = current.Index + this.SessionResultSteps(isLevelCompleted);
		if (num == current.Index)
		{
			return current;
		}
		bool flag = current.Index > num;
		int num2 = Mathf.Abs(num - current.Index);
		ILevelProxy levelProxy = current;
		for (int i = 0; i < num2; i++)
		{
			levelProxy = ((!flag) ? levelProxy.NextLevel : levelProxy.PreviousLevel);
		}
		return levelProxy;
	}

	[SerializeField]
	private int completedLevelSteps;

	[SerializeField]
	private int failedLevelSteps;

	[SerializeField]
	private bool treasureLevel;
}
