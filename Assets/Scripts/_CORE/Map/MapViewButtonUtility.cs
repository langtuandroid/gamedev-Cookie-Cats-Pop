using System;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.PlayablePostcard.Views;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

public class MapViewButtonUtility : MonoBehaviour, IMapButtonView, IPostcardMapButtonView
{
	public void ObtainOverlay(UIView view)
	{
		this.livesOverlay = UIViewManager.Instance.ObtainOverlay<LivesOverlay>(view);
	}

	public void ReleaseOverlay()
	{
		UIViewManager.Instance.ReleaseOverlay<LivesOverlay>();
	}

	public void SetupOnLifeButtonClicked()
	{
		this.livesOverlay.OnLifeButtonClicked = delegate()
		{
			if (ManagerRepository.Get<InventoryManager>().Lives > 0)
			{
				UIViewManager.Instance.ShowView<GetMoreLivesView>(new object[0]);
			}
			else
			{
				UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[0]);
			}
		};
	}

	public void ReleaseOnLifeButtonClicked()
	{
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = null;
		}
	}

	public void FadeAndSwitchToMainMapView()
	{
	}

	private LivesOverlay livesOverlay;
}
