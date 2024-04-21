using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using UnityEngine;

public class OneLifeChallengeLevelResultView : UIView
{
	public OneLifeChallengeManager OneLifeChallengeManager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<OneLifeChallengeManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		base.ViewLoad(parameters);
		this.levelCompletedPivot.SetActive(false);
		this.levelFailedPivot.SetActive(false);
		this.failed = this.OneLifeChallengeManager.LevelFailed;
		if (this.failed)
		{
			this.levelFailedPivot.SetActive(true);
			AudioManager.Instance.SetMusic(null, true);
			SingletonAsset<SoundDatabase>.Instance.levelFailed.Play();
		}
		else
		{
			this.levelCompletedPivot.SetActive(true);
		}
		this.dialogInstantiator.GetInstance<DialogFrame>().Title = ((!this.failed) ? L.Get("Well Done!") : L.Get("You Failed!"));
		this.confirmButtonInstatiator.GetInstance<ButtonWithTitle>().Title = ((!this.failed) ? L.Get("Continue!") : L.Get("Try Again!"));
		this.SetUpDotDefaults();
	}

	protected override void ViewDidAppear()
	{
		base.ViewDidAppear();
		this.animFiber.Start(this.AnimateProgress());
	}

	protected override void ViewWillDisappear()
	{
		base.ViewWillDisappear();
		this.beeAnimationFiber.Terminate();
		this.animFiber.Terminate();
	}

	private void OnButtonClicked(UIEvent e)
	{
		base.Close(0);
		AudioManager.Instance.SetMusic(SingletonAsset<SoundDatabase>.Instance.mapMusic, true);
	}

	private OneLifeChallengeLevelResultDot GetLevelResultDot(int index)
	{
		return this.progressDots[index].GetInstance<OneLifeChallengeLevelResultDot>();
	}

	private int GetLevelsCompleted()
	{
		return (!this.failed) ? this.OneLifeChallengeManager.FarthestCompletedLevel : this.OneLifeChallengeManager.FailedLevelProxy.Index;
	}

	private void SetUpDotDefaults()
	{
		int levelsCompleted = this.GetLevelsCompleted();
		for (int i = 0; i < this.progressDots.Length; i++)
		{
			this.GetLevelResultDot(i).SetDotRoot(i + 1, (i <= levelsCompleted && !this.failed) || (i < levelsCompleted && this.failed), i < levelsCompleted);
		}
	}

	private IEnumerator AnimateProgress()
	{
		int levelsCompleted = this.GetLevelsCompleted();
		if (levelsCompleted == -1)
		{
			yield break;
		}
		yield return this.GetLevelResultDot(levelsCompleted).AnimateMark(!this.failed, false);
		yield break;
	}

	public UIInstantiator dialogInstantiator;

	public UIInstantiator confirmButtonInstatiator;

	public GameObject levelCompletedPivot;

	public GameObject levelFailedPivot;

	public Instantiator[] progressDots;

	private Fiber animFiber = new Fiber();

	private Fiber beeAnimationFiber = new Fiber();

	private bool failed;
}
