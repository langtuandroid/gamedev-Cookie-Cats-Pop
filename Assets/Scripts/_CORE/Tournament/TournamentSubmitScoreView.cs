using System;
using System.Collections;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class TournamentSubmitScoreView : UIView
{
	private TournamentCloudManager TournamentCloudManager
	{
		get
		{
			return ManagerRepository.Get<TournamentCloudManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.rank = TournamentManager.Instance.GetCurrentRank();
		this.rankIcon.TextureResource = "Tournament/Medal" + this.rank;
		this.rankName.text = TournamentUI.GetRankAsString(this.rank);
		this.updateFiber.Start(this.UpdateState());
	}

	protected override void ViewWillAppear()
	{
		UIFontStyle fontStyle = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(this.rank).fontStyle;
		if (fontStyle)
		{
			this.rankName.fontStyle = fontStyle;
		}
	}

	protected override void ViewWillDisappear()
	{
		this.updateFiber.Terminate();
	}

	public void EnablePivot(GameObject pivot)
	{
		this.waitPivot.SetActive(pivot == this.waitPivot);
		this.retryPivot.SetActive(pivot == this.retryPivot);
	}

	private IEnumerator UpdateState()
	{
		LocalScore unsubmittedScore = TournamentManager.Instance.PersistedUnsubmittedScore;
		if (unsubmittedScore != null)
		{
			this.EnablePivot(this.waitPivot);
			object error = null;
			yield return this.TournamentCloudManager.SubmitScore(unsubmittedScore.Leaderboard, unsubmittedScore.Score, delegate(object err)
			{
				error = err;
			});
			if (error != null)
			{
				this.EnablePivot(this.retryPivot);
			}
			else
			{
				TournamentManager.Instance.PersistedUnsubmittedScore = null;
				base.Close(1);
			}
		}
		else
		{
			base.Close(1);
		}
		yield break;
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void RetryClicked(UIEvent e)
	{
		this.updateFiber.Start(this.UpdateState());
	}

	public GameObject retryPivot;

	public GameObject waitPivot;

	public UILabel rankName;

	public UIResourceQuad rankIcon;

	private TournamentRank rank = TournamentRank.Bronze;

	private Fiber updateFiber = new Fiber();
}
