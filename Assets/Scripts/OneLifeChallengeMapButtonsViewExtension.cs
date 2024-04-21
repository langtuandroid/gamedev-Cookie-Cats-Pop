using System;
using Shared.OneLifeChallenge;
using Tactile;
using UnityEngine;

[RequireComponent(typeof(OneLifeChallengeMapButtonsView))]
public class OneLifeChallengeMapButtonsViewExtension : MonoBehaviour
{
	private void Awake()
	{
		this.view = base.GetComponent<OneLifeChallengeMapButtonsView>();
	}

	protected void ViewWillAppear()
	{
		this.livesOverlay = UIViewManager.Instance.ObtainOverlay<LivesOverlay>(this.view);
		this.livesOverlay.OffsetLivesButton(new Vector2(0f, -this.topBarElement.Size.y));
	}

	protected void ViewDidDisappear()
	{
		this.livesOverlay.OffsetLivesButton(new Vector2(0f, this.topBarElement.Size.y));
		UIViewManager.Instance.ReleaseOverlay<LivesOverlay>();
	}

	protected void ViewGotFocus()
	{
		this.livesOverlay.OnLifeButtonClicked = delegate()
		{
			if (InventoryManager.Instance.Lives > 0)
			{
				UIViewManager.Instance.ShowView<GetMoreLivesView>(new object[0]);
			}
			else
			{
				UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[0]);
			}
		};
	}

	protected void ViewLostFocus()
	{
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = null;
		}
	}

	[SerializeField]
	private UIElement topBarElement;

	private LivesOverlay livesOverlay;

	private OneLifeChallengeMapButtonsView view;
}
