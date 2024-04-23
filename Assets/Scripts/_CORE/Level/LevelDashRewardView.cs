using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGames.LevelDash;
using UnityEngine;

public class LevelDashRewardView : UIView
{
	private ButtonWithTitle claimButtonEnabled
	{
		get
		{
			return this.claimButton.GetInstance<ButtonWithTitle>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		base.ViewLoad(parameters);
		this.reward = (parameters[0] as LevelDashConfig.Reward);
		this.descriptionLabel.text = string.Format(L.Get("You achieved rank {0} in the Level Dash!"), this.reward.Rank);
		this.items = this.reward.Items;
		string arg = string.Empty;
		int rank = this.reward.Rank;
		if (rank != 1)
		{
			if (rank != 2)
			{
				if (rank == 3)
				{
					arg = "Bronze";
				}
			}
			else
			{
				arg = "Silver";
			}
		}
		else
		{
			arg = "Gold";
		}
		this.cupSprite.TextureResource = string.Format("StarTournament/StarTrophys_{0}", arg);
		this.rewardGrid.Initialize(this.items, true);
	}

	private void ClaimClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.AnimateClaimSuccess(), false);
	}

	private IEnumerator AnimateClaimSuccess()
	{
		this.claimButtonEnabled.Disabled = true;
		yield return this.rewardGrid.Animate(false, true);
		base.Close(0);
		yield break;
	}

	[Header("UI Elements")]
	[SerializeField]
	private GameObject sparkle;

	[SerializeField]
	private UIResourceQuad cupSprite;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private UIInstantiator claimButton;

	private LevelDashConfig.Reward reward;

	private List<ItemAmount> items;
}
