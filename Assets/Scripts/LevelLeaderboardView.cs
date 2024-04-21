using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

public class LevelLeaderboardView : LeaderboardView
{
	private void SetScores(Dictionary<int, LevelLeaderboardView.CloudScoreWithOldPosition> scores)
	{
		this.itemList.DestroyAllContent();
		this.itemList.BeginAdding();
		this.afterAnimationPositions.Clear();
		foreach (KeyValuePair<int, LevelLeaderboardView.CloudScoreWithOldPosition> keyValuePair in scores)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
			gameObject.SetActive(true);
			LeaderboardItem component = gameObject.GetComponent<LeaderboardItem>();
			if (component == null)
			{
				throw new Exception("Need to set an instance of LevelLeaderboardItem for listview");
			}
			this.afterAnimationPositions.Add(component, keyValuePair.Key);
			component.InitFacebookClient(keyValuePair.Value.oldPosition, keyValuePair.Value.cloudScore);
			this.itemList.AddToContent(component.GetComponent<UIElement>());
		}
		this.itemList.EndAdding();
	}

	private void SetScores(List<CloudScore> scores)
	{
		Dictionary<int, LevelLeaderboardView.CloudScoreWithOldPosition> dictionary = new Dictionary<int, LevelLeaderboardView.CloudScoreWithOldPosition>();
		foreach (CloudScore cloudScore in scores)
		{
			dictionary[scores.IndexOf(cloudScore) + 1] = new LevelLeaderboardView.CloudScoreWithOldPosition(cloudScore, scores.IndexOf(cloudScore) + 1);
		}
		this.SetScores(dictionary);
	}

	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length != 1 || !(parameters[0] is LevelLeaderboardView.LevelLeaderboardData))
		{
			throw new Exception("Must pass LevelLeaderboardData to the LevelLeaderboardView");
		}
		UICamera.DisableInput();
		LevelLeaderboardView.LevelLeaderboardData levelLeaderboardData = (LevelLeaderboardView.LevelLeaderboardData)parameters[0];
		this.level = levelLeaderboardData.Level;
		this.oldScores = levelLeaderboardData.OldScores;
		if (this.oldScores != null)
		{
			this.updateFiber.Start(this.UpdateScore());
		}
		else
		{
			UICamera.EnableInput();
			base.Close(0);
		}
	}

	protected override void ViewWillDisappear()
	{
		this.updateFiber.Terminate();
	}

	private IEnumerator UpdateScore()
	{
		yield return new Fiber.OnExit(delegate()
		{
			UICamera.EnableInput();
		});
		DialogFrame dialog = this.dialogInstantiator.GetComponentInChildren<DialogFrame>();
		dialog.HasCloseButton = false;
		List<CloudScore> newScores = ManagerRepository.Get<LeaderboardManager>().GetCachedCloudScores(this.level.Index);
		int oldPositionIndex = CloudScoreHelper.MyPosition(this.oldScores);
		int newPositionIndex = CloudScoreHelper.MyPosition(newScores);
		CloudScore myNewScore = CloudScoreHelper.MyScore(newScores);
		if (oldPositionIndex == -1)
		{
			oldPositionIndex = newScores.Count - 1;
		}
		if (oldPositionIndex > newPositionIndex)
		{
			Dictionary<int, LevelLeaderboardView.CloudScoreWithOldPosition> startingPositions = new Dictionary<int, LevelLeaderboardView.CloudScoreWithOldPosition>();
			for (int i = 0; i < newScores.Count; i++)
			{
				int num = i + 1;
				if (i > oldPositionIndex || i < newPositionIndex)
				{
					startingPositions[num] = new LevelLeaderboardView.CloudScoreWithOldPosition(newScores[i], num);
				}
				else if (i == oldPositionIndex)
				{
					startingPositions[newPositionIndex + 1] = new LevelLeaderboardView.CloudScoreWithOldPosition(myNewScore, oldPositionIndex + 1);
				}
				else
				{
					startingPositions[num + 1] = new LevelLeaderboardView.CloudScoreWithOldPosition(newScores[i + 1], num);
				}
			}
			this.SetScores(startingPositions);
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
			this.confetti.SetActive(true);
			yield return this.itemList.AnimateCellToPosition(oldPositionIndex, newPositionIndex, 0.5f, 0.5f, SingletonAsset<CommonCurves>.Instance.easeInOut, SingletonAsset<CommonCurves>.Instance.easeInOut);
			foreach (KeyValuePair<LeaderboardItem, int> keyValuePair in this.afterAnimationPositions)
			{
				keyValuePair.Key.SetPosition(keyValuePair.Value);
			}
		}
		else
		{
			this.SetScores(newScores);
		}
		dialog.HasCloseButton = true;
		yield break;
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private UIListPanel itemList;

	[SerializeField]
	private GameObject elementPrefab;

	[SerializeField]
	private UIInstantiator dialogInstantiator;

	[SerializeField]
	private GameObject confetti;

	private Fiber updateFiber = new Fiber();

	private LevelProxy level;

	private List<CloudScore> oldScores = new List<CloudScore>();

	private Dictionary<LeaderboardItem, int> afterAnimationPositions = new Dictionary<LeaderboardItem, int>();

	public class LevelLeaderboardData
	{
		public LevelProxy Level { get; set; }

		public List<CloudScore> OldScores { get; set; }
	}

	private class CloudScoreWithOldPosition
	{
		public CloudScoreWithOldPosition(CloudScore cloudScore, int oldPosition)
		{
			this.cloudScore = cloudScore;
			this.oldPosition = oldPosition;
		}

		public CloudScore cloudScore;

		public int oldPosition;
	}
}
