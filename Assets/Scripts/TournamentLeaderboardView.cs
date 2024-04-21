using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class TournamentLeaderboardView : UIView
{
	public bool ReadyToAnimateFocusOfPlayer
	{
		get
		{
			return this.readyToAnimateFocusOfPlayer;
		}
		set
		{
			this.readyToAnimateFocusOfPlayer = value;
			this.TryToToAnimateFocusOfPlayer();
		}
	}

	public bool AnimateFocusOfPlayerOnViewDidAppear
	{
		get
		{
			return this.animateFocusOfPlayerOnViewDidAppear;
		}
		set
		{
			this.animateFocusOfPlayerOnViewDidAppear = value;
			this.TryToToAnimateFocusOfPlayer();
		}
	}

	public void Initialize(bool focusBottom, CloudClientBase cloudClient)
	{
		this.cloudClient = cloudClient;
		this.focusBottom = focusBottom;
		this.elementPrefab.gameObject.SetActive(false);
		this.loadingSpinner.SetActive(false);
		this.UpdateTable();
	}

	private void TryToToAnimateFocusOfPlayer()
	{
		if (this.ReadyToAnimateFocusOfPlayer && this.AnimateFocusOfPlayerOnViewDidAppear)
		{
			this.animationFiber.Start(this.AnimateFocusOfPlayer(750f));
		}
	}

	protected override void ViewWillDisappear()
	{
		this.animationFiber.Terminate();
	}

	protected override void ViewDidDisappear()
	{
		this.ReadyToAnimateFocusOfPlayer = false;
		this.AnimateFocusOfPlayerOnViewDidAppear = false;
	}

	protected override void ViewWillAppear()
	{
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(TournamentManager.Instance.GetCurrentRank());
		this.subtitle.text = L.Get(rankSetup.displayName);
		UIFontStyle fontStyle = rankSetup.fontStyle;
		if (fontStyle)
		{
			this.subtitle.fontStyle = fontStyle;
		}
	}

	protected override void ViewDidAppear()
	{
		this.UpdateTable();
		this.ReadyToAnimateFocusOfPlayer = true;
	}

	private void Update()
	{
		this.title.text = TournamentManager.Instance.GetTimeRemainingForTournamentAsString();
	}

	private void UpdateTable()
	{
		this.itemList.DestroyAllContent();
		this.itemList.BeginAdding();
		List<TournamentCloudManager.Score> sortedOverallScores = ManagerRepository.Get<TournamentCloudManager>().SortedOverallScores;
		for (int i = 0; i < sortedOverallScores.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
			gameObject.SetActive(true);
			this.itemList.AddToContent(gameObject.GetComponent<UIElement>());
			TournamentLeaderboardItem component = gameObject.GetComponent<TournamentLeaderboardItem>();
			component.Init(i + 1, sortedOverallScores[i], this.cloudClient);
			component.AppearInPanel();
			string deviceId = (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
			string userId = (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
			bool flag = sortedOverallScores[i].IsOwnedByDeviceOrUser(deviceId, userId);
			if (flag)
			{
				this.focusPosition = new Vector2(gameObject.transform.localPosition.x, component.GetElementSize().y * (float)i);
				this.focusSize = component.GetElementSize();
				this.playerItem = component;
			}
		}
		this.itemList.EndAdding();
		if (!this.focusBottom)
		{
			this.itemList.SetScroll(new Vector2(0f, -(this.itemList.TotalContentSize.y - (this.focusPosition.y + this.focusSize.y * 0.5f))));
		}
		else
		{
			this.itemList.SetScroll(new Vector2(0f, 0f));
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public IEnumerator AnimateFocusOfPlayer(float speed)
	{
		yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		Vector2 source = new Vector2(0f, 0f);
		Vector2 dest = new Vector2(0f, -(this.itemList.TotalContentSize.y - (this.focusPosition.y + this.focusSize.y * 0.5f)));
		float duration = Mathf.Min(Vector2.Distance(source, dest) / speed, 2f);
		AnimationCurve easeInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		yield return FiberAnimation.Animate(duration, easeInOut, delegate(float t)
		{
			this.itemList.SetScroll(Vector2.Lerp(source, dest, t));
		}, false);
		if (this.playerItem != null)
		{
			yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
			this.playerItem.transform.localPosition += Vector3.back * 40f;
			yield return FiberAnimation.ScaleTransform(this.playerItem.transform, Vector3.zero, Vector3.one, this.attentionCurve, 0f);
		}
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		base.Close(0);
		yield break;
	}

	public UILabel title;

	public UILabel subtitle;

	public UIListPanel itemList;

	public GameObject elementPrefab;

	public GameObject loadingSpinner;

	public AnimationCurve attentionCurve;

	protected Fiber animationFiber = new Fiber();

	private Vector2 focusPosition = new Vector2(0f, 0f);

	private Vector2 focusSize = new Vector2(0f, 0f);

	private bool focusBottom;

	private TournamentLeaderboardItem playerItem;

	private CloudClientBase cloudClient;

	private bool readyToAnimateFocusOfPlayer;

	private bool animateFocusOfPlayerOnViewDidAppear;
}
