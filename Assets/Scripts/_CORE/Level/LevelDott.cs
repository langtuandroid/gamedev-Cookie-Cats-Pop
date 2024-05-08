using System;
using System.Collections.Generic;
using NinjaUI;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.ReengagementRewards;
using UnityEngine;

public class LevelDott : MapDot
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	protected override LevelDatabase LevelContext
	{
		get
		{
			return this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		}
	}

	private void Awake()
	{
		this.reengagementManager = ManagerRepository.Get<ReengagementRewardManager>();
		this.reengagementManager.OnReengagementActivated += this.OnReengagementActivated;
		this.objectiveNormalColorPulsator = this.normalObjects[0].gameObject.GetComponent<ColorPulsator>();
		this.objectiveHardColorPulsator = this.hardObjects[0].gameObject.GetComponent<ColorPulsator>();
	}

	private void OnDestroy()
	{
		this.reengagementManager.OnReengagementActivated -= this.OnReengagementActivated;
	}

	private void OnReengagementActivated()
	{
		this.TryToActivateReengagementEffect(this.LevelId);
	}

	private void TryToActivateReengagementEffect(int levelId)
	{
		if (this.reengagementManager != null)
		{
			bool enabled = this.reengagementManager.IsActiveOnLevel(levelId);
			this.objectiveNormalColorPulsator.enabled = enabled;
			this.objectiveHardColorPulsator.enabled = enabled;
		}
	}

	protected override void UpdateUI(bool gameIsRunning)
	{
		this.TryToActivateReengagementEffect(this.LevelId);
		if (!base.Level.IsValid)
		{
			this.nameLabel.text = "???";
			return;
		}
		if (this.nameLabel != null)
		{
			this.nameLabel.text = base.Level.DisplayName;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (GameObject gameObject in this.normalObjects)
		{
			gameObject.SetActive(base.Level.LevelDifficulty != LevelDifficulty.Hard);
		}
		foreach (GameObject gameObject2 in this.hardObjects)
		{
			gameObject2.SetActive(base.Level.LevelDifficulty == LevelDifficulty.Hard);
		}
		this.enabledRoot.SetActive(base.Level.IsUnlocked); 
		this.disabledRoot.SetActive(!this.enabledRoot.activeSelf);
		this.stars.SetStars(base.Level.Stars, base.Level.GetHardStars());
	}

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private MapDotStars stars;

	[SerializeField]
	private UISprite objectiveIcon;

	[SerializeField]
	private Color disabledIconColor;

	[SerializeField]
	private GameObject enabledRoot;

	[SerializeField]
	private GameObject disabledRoot;

	[SerializeField]
	private List<GameObject> hardObjects;

	[SerializeField]
	private List<GameObject> normalObjects;

	private ReengagementRewardManager reengagementManager;

	private ColorPulsator objectiveNormalColorPulsator;

	private ColorPulsator objectiveHardColorPulsator;
}
