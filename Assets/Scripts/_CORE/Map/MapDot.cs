using System;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public abstract class MapDot : MapDotBase
{
	[Instantiator.SerializeProperty]
	public override int LevelId
	{
		get
		{
			return this.dotIndex;
		}
		set
		{
			this.dotIndex = value;
			if (Application.isPlaying)
			{
				this.UpdateUI();
			}
			else
			{
				this.UpdateUI(false);
			}
		}
	}

	protected abstract LevelDatabase LevelContext { get; }

	public LevelProxy Level
	{
		get
		{
			return (!(this.LevelContext != null)) ? LevelProxy.Invalid : this.LevelContext.GetLevel(this.dotIndex);
		}
	}

	public override void Initialize()
	{
		if (!this.Level.IsValid)
		{
		}
		this.UpdateUI(true);
	}

	public override bool IsCompleted
	{
		get
		{
			return this.Level.IsCompleted;
		}
	}

	public override bool IsUnlocked
	{
		get
		{
			return this.Level.IsUnlocked;
		}
	}

	protected abstract void UpdateUI(bool isGameRunning);

	public override void UpdateUI()
	{
		this.UpdateUI(true);
	}

	private int dotIndex;
}
