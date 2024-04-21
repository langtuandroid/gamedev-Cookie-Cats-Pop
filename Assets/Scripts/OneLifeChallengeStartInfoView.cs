using System;
using TactileModules.FeatureManager;
using UnityEngine;

public class OneLifeChallengeStartInfoView : UIView
{
	private OneLifeChallengeManager OneLifeChallengeManager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<OneLifeChallengeManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		bool flag = false;
		if (parameters != null && parameters.Length > 0)
		{
			flag = (bool)parameters[0];
		}
		if (flag)
		{
			this.buttonInstantiator.GetInstance<ButtonWithTitle>().Title = L.Get("Continue!");
		}
		else
		{
			this.buttonInstantiator.GetInstance<ButtonWithTitle>().Title = L.Get("Begin!");
		}
		OneLifeChallengeConfig config = this.OneLifeChallengeManager.Config;
		this.rewardGrid.Initialize(config.Rewards, true);
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - this.lastUpdated >= 1f)
		{
			this.timeLeftLabel.text = this.OneLifeChallengeManager.TimeLeftAsText;
			this.lastUpdated = Time.realtimeSinceStartup;
		}
	}

	private void Dismiss(UIEvent e)
	{
		base.Close(0);
	}

	private void Continue(UIEvent e)
	{
		base.Close(1);
	}

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private UILabel timeLeftLabel;

	[SerializeField]
	private UIInstantiator buttonInstantiator;

	private float lastUpdated;
}
