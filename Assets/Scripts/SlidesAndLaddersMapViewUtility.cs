using System;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

public class SlidesAndLaddersMapViewUtility : MonoBehaviour, ISlidesAndLaddersMapView
{
	public void PlaySlideSound(float triggerTimer)
	{
		SingletonAsset<SoundDatabase>.Instance.climbLadder.Play();
	}

	public void PlayLadderSound(float triggerTimer)
	{
		SingletonAsset<SoundDatabase>.Instance.slideDown.Play();
	}

	public UIViewManager.UIViewState ShowNoMoreLivesView()
	{
		return UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[0]);
	}

	public void FadeAndSwitchToMainMapView()
	{
	}
}
