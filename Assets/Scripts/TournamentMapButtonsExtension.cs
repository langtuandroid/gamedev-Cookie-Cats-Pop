using System;
using UnityEngine;

[RequireComponent(typeof(TournamentViewButtons))]
public class TournamentMapButtonsExtension : MonoBehaviour, TournamentViewButtons.IExtension
{
	private void Awake()
	{
		this.view = base.GetComponent<TournamentViewButtons>();
	}

	protected void ViewWillAppear()
	{
		this.livesOverlay = UIViewManager.Instance.ObtainOverlay<LivesOverlay>(this.view);
		this.livesOverlay.lifeBar.LivesType = "TournamentLife";
	}

	protected void ViewDidDisappear()
	{
		UIViewManager.Instance.ReleaseOverlay<LivesOverlay>();
	}

	protected void ViewGotFocus()
	{
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = delegate()
			{
				if (TournamentManager.Instance.Lives > 0)
				{
					UIViewManager.Instance.ShowView<GetMoreLivesView>(new object[]
					{
						true
					});
				}
				else
				{
					UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[]
					{
						true
					});
				}
			};
			this.livesOverlay.transform.position = this.lifeButtonAnchor.transform.position;
			this.livesOverlay.GetElement().SetSizeAndDoLayout(this.lifeButtonAnchor.Size);
		}
	}

	protected void ViewLostFocus()
	{
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = null;
		}
	}

	public void UpdateRewardButtonIcon(UIInstantiator rewardButton, TournamentSetup.RankSetup rankSetup)
	{
		rewardButton.GetInstance<ButtonWithBadge>().IconSpriteName = rankSetup.iconSpriteName;
		this.tournamentRankLabel.text = rankSetup.displayName;
		this.tournamentRankLabel.fontStyle = rankSetup.fontStyle;
	}

	[SerializeField]
	private UIElement lifeButtonAnchor;

	[SerializeField]
	private UILabel tournamentRankLabel;

	private LivesOverlay livesOverlay;

	private TournamentViewButtons view;
}
