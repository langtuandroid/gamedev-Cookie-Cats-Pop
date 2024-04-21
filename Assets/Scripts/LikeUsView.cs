using System;
using UnityEngine;

public class LikeUsView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.facebookButton.SetActive(false);
		this.twitterButton.SetActive(false);
		this.instagramButton.SetActive(false);
		this.youTubeButton.SetActive(false);
		switch (LikeUsManager.Instance.CurrentLikeUsType)
		{
		case LikeUsManager.LikeUsType.Facebook:
			this.message.text = L.Get("Like us on Facebook and get a freebie!");
			this.facebookButton.SetActive(true);
			break;
		case LikeUsManager.LikeUsType.Twitter:
			this.message.text = L.Get("Follow us on Twitter and get a freebie!");
			this.twitterButton.SetActive(true);
			break;
		case LikeUsManager.LikeUsType.Instagram:
			this.message.text = L.Get("Follow us on Instagram and get a freebie!");
			this.instagramButton.SetActive(true);
			break;
		case LikeUsManager.LikeUsType.YouTube:
			this.message.text = L.Get("Subscribe to our YouTube channel and get a freebie!");
			this.youTubeButton.SetActive(true);
			break;
		}
		this.rewardGrid.Initialize(LikeUsManager.Instance.CurrentSocialRewards.Rewards, true);
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(LikeUsView.ClosingType.Closed);
	}

	private void LikeClicked(UIEvent e)
	{
		LikeUsManager.Instance.GiveRewardAndUpdateNextSocialNetwork();
		FiberCtrl.Pool.Run(this.rewardGrid.Animate(true, true), false);
		base.Close(LikeUsView.ClosingType.Liked);
	}

	[SerializeField]
	private GameObject facebookButton;

	[SerializeField]
	private GameObject twitterButton;

	[SerializeField]
	private GameObject instagramButton;

	[SerializeField]
	private GameObject youTubeButton;

	[SerializeField]
	private UILabel message;

	public RewardGrid rewardGrid;

	private enum ClosingType
	{
		Closed,
		Liked
	}
}
