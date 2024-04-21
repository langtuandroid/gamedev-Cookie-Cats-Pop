using System;
using JetBrains.Annotations;
using Shared.OneLifeChallenge;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

public class OneLifeChallengeMapButton : SideMapButton
{
	private OneLifeChallengeManager Manager
	{
		get
		{
			OneLifeChallengeManager result;
			if ((result = this.manager) == null)
			{
				result = (this.manager = FeatureManager.GetFeatureHandler<OneLifeChallengeManager>());
			}
			return result;
		}
	}

	protected override void UpdateOncePerSecond()
	{
		if (!this.VisibilityChecker(null))
		{
			return;
		}
		this.timerLabel.text = this.Manager.TimeLeftAsText;
	}

	[UsedImplicitly]
	private new void Clicked(UIEvent e)
	{
		if (this.Manager.SecondsLeft > 0)
		{
			OneLifeChallengeSystem oneLifeChallengeSystem = ManagerRepository.Get<OneLifeChallengeSystem>();
			OneLifeChallengeMapFlow c = oneLifeChallengeSystem.ControllerFactory.CreateMapFlow();
			FlowStack flowStack = ManagerRepository.Get<FlowStack>();
			flowStack.Push(c);
		}
	}

	public override SideMapButton.AreaSide Side
	{
		get
		{
			return SideMapButton.AreaSide.Left;
		}
	}

	public override bool VisibilityChecker(object data)
	{
		return this.Manager.HasActiveFeature() && this.Manager.Config.FeatureEnabled && !this.Manager.GetFeatureInstanceCustomData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>().RewardClaimed && this.Manager.SecondsLeft > 0;
	}

	public override Vector2 Size
	{
		get
		{
			return this.GetElementSize();
		}
	}

	public override object Data
	{
		get
		{
			return null;
		}
	}

	[SerializeField]
	private UILabel timerLabel;

	private OneLifeChallengeManager manager;
}
