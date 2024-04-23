using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class AchievementItem : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<AchievementItem> OnClaimClick;

	public AchievementAsset Achievement { get; private set; }

	public AchievementReward Reward { get; private set; }

	private AchievementsManager AchievementsManager
	{
		get
		{
			return ManagerRepository.Get<AchievementsManager>();
		}
	}

	private void OnEnable()
	{
		this.StampEffect.SetActive(false);
	}

	public void Initialize(AchievementAsset achievement)
	{
		this.Achievement = achievement;
		this.Reward = this.AchievementsManager.GetRewardForAchievement(achievement);
		if (this.Reward != null && this.Reward.Rewards.Count > 0)
		{
			this.rewardGrid.Initialize(this.Reward.Rewards, true);
			this.amountLabel.text = this.Reward.Rewards[0].Amount.ToString();
		}
		this.title.text = achievement.FormattedTitle;
		this.description.text = achievement.FormattedDescription;
		float num = (float)this.AchievementsManager.GetAchievementProgress(achievement);
		bool flag = this.AchievementsManager.IsAchievementCompleted(achievement);
		bool flag2 = this.AchievementsManager.IsAchievementClaimed(achievement);
		if (flag)
		{
			this.progress.text = L.Get("Completed");
			this.progressBar.FillAmount = 1f;
		}
		else if (achievement.thresholdType == AchievementAsset.ThresholdType.ValueInSingleEventExact)
		{
			num = (float)((num < (float)achievement.objectiveThreshold) ? 0 : 1);
			this.progress.text = num + "/1";
			this.progressBar.FillAmount = num;
		}
		else
		{
			this.progress.text = Mathf.Min(num, (float)achievement.objectiveThreshold) + "/" + achievement.objectiveThreshold;
			this.progressBar.FillAmount = Mathf.Min(1f, num / (float)achievement.objectiveThreshold);
		}
		this.RewardPivot.SetActive(!flag || !flag2);
		this.CheckMark.SetActive(flag2);
		this.ClaimButton.SetActive(flag && !flag2);
	}

	private void OnDestroy()
	{
		this.checkMarkFiber.Terminate();
	}

	public void ClaimClicked(UIEvent e)
	{
		if (this.OnClaimClick != null)
		{
			this.OnClaimClick(this);
		}
	}

	private void ShowCheckMark()
	{
		this.checkMarkFiber.Start(this.AnimCheckMark());
	}

	private IEnumerator AnimCheckMark()
	{
		this.CheckMark.SetActive(true);
		this.StampEffect.SetActive(true);
		this.RewardPivot.SetActive(false);
		this.ClaimButton.SetActive(false);
		yield return FiberAnimation.ScaleTransform(this.CheckMark.transform, Vector3.one * 0.7f, Vector3.one, this.stampCurve, 0.3f);
		yield break;
	}

	public IEnumerator AnimateRewards()
	{
		this.CheckMark.SetActive(false);
		this.ClaimButton.SetActive(false);
		yield return this.rewardGrid.Animate(false, false);
		this.ShowCheckMark();
		yield break;
	}

	public UILabel title;

	public UILabel description;

	public UILabel progress;

	public GameObject CheckMark;

	public RewardGrid rewardGrid;

	public GameObject RewardPivot;

	public GameObject ClaimButton;

	public UIFillModifier progressBar;

	public GameObject StampEffect;

	public AnimationCurve stampCurve;

	public UILabel amountLabel;

	private Fiber checkMarkFiber = new Fiber();
}
