using System;
using System.Collections.Generic;
using UnityEngine;

public class AimingState
{
	public AimingState()
	{
		this.aims = new List<AimInfo>
		{
			new AimInfo(0f)
		};
		this.Clear();
	}

	public List<AimInfo> Aims
	{
		get
		{
			return this.aims;
		}
	}

	public AimInfo Main
	{
		get
		{
			return this.aims[0];
		}
	}

	public Ray2D[] ValidAimRays
	{
		get
		{
			int num = 0;
			for (int i = 0; i < this.aims.Count; i++)
			{
				if (this.aims[i].IsValidForShot)
				{
					num++;
				}
			}
			Ray2D[] array = new Ray2D[num];
			int num2 = 0;
			for (int j = 0; j < this.aims.Count; j++)
			{
				if (this.aims[j].IsValidForShot)
				{
					array[num2++] = new Ray2D(this.aims[j].aimOriginInBoardSpace, this.aims[j].direction);
				}
			}
			return array;
		}
	}

	public void Clear()
	{
		while (this.aims.Count > 1)
		{
			this.aims.RemoveAt(this.aims.Count - 1);
		}
	}

	private List<AimInfo> aims;
}
