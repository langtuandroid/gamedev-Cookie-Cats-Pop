using System;
using TactileModules.FeatureManager;
using UnityEngine;

public class OneLifeChallengeLevelDot : MapDotBase
{
	public OneLifeChallengeManager OneLifeChallengeManager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<OneLifeChallengeManager>();
		}
	}

	[Instantiator.SerializeProperty]
	public override int LevelId
	{
		get
		{
			return this.levelId;
		}
		set
		{
			this.levelId = value;
			if (Application.isPlaying)
			{
				this.UpdateUI();
			}
		}
	}

	protected LevelProxy LevelProxy
	{
		get
		{
			return this.OneLifeChallengeManager.OneLifeChallengeLevelDatabase.GetLevel(this.LevelId);
		}
	}

	public override bool IsUnlocked
	{
		get
		{
			return this.levelId <= this.OneLifeChallengeManager.FarthestCompletedLevel + 1;
		}
	}

	public override bool IsCompleted
	{
		get
		{
			return this.levelId <= this.OneLifeChallengeManager.FarthestCompletedLevel;
		}
	}

	public override void Initialize()
	{
		this.UpdateUI();
	}

	public override void UpdateUI()
	{
		this.lvlNumber.text = this.LevelProxy.HumanNumber.ToString();
		this.lvlNumber.gameObject.SetActive(!this.IsCompleted);
		this.enabledRoot.SetActive(this.IsUnlocked && !this.IsCompleted);
		this.completedRoot.SetActive(this.IsCompleted);
		this.disabledRoot.SetActive(!this.IsUnlocked);
	}

	[Instantiator.SerializeProperty]
	public float Scale
	{
		get
		{
			return this.scalePivot.localScale.x;
		}
		set
		{
			this.scalePivot.localScale = new Vector3(value, value, 1f);
		}
	}

	public UILabel lvlNumber;

	public GameObject enabledRoot;

	public GameObject disabledRoot;

	public GameObject completedRoot;

	public Transform scalePivot;

	private int levelId;
}
