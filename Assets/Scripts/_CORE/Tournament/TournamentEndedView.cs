using System;
using UnityEngine;

public class TournamentEndedView : UIView
{
	protected override void ViewWillAppear()
	{
		UIButton componentInChildren = this.claimButton.GetInstance().GetComponentInChildren<UIButton>();
		componentInChildren.Clicked += this.ClaimButtonClicked;
		this.rank = TournamentManager.Instance.GetCurrentRank();
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(this.rank);
		UIFontStyle fontStyle = rankSetup.fontStyle;
		if (fontStyle)
		{
			this.rankName.fontStyle = fontStyle;
		}
		this.rankName.text = L.Get(rankSetup.displayName);
		this.rankIcon.TextureResource = rankSetup.bigIconResourcePath;
	}

	private void ClaimButtonClicked(UIButton button)
	{
		base.Close(0);
	}

	[SerializeField]
	private UILabel rankName;

	[SerializeField]
	private UIResourceQuad rankIcon;

	[SerializeField]
	[InstantiatorRequires(typeof(UIButton))]
	private UIInstantiator claimButton;

	private TournamentRank rank = TournamentRank.Bronze;
}
