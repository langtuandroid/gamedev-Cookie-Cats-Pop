using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGames.EndlessChallenge.Data;
using UnityEngine;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	public class EndlessChallengeLeaderboardView : UIView
	{
		private EndlessChallengeHandler Handler
		{
			get
			{
				return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
			}
		}

		public void UpdateTable(bool showOthers)
		{
			this.itemList.DestroyAllContent();
			this.itemList.BeginAdding();
			this.notConnectedPivot.gameObject.SetActive(!showOthers);
			if (showOthers)
			{
				List<Entry> allParticipants = this.Handler.GetAllParticipants();
				for (int i = 0; i < allParticipants.Count; i++)
				{
					this.InitializeLeaderboardItem(i, allParticipants[i]);
				}
				this.HandleViewSize(allParticipants.Count);
			}
			else
			{
				Entry entry = new Entry
				{
					DeviceId = this.Handler.DeviceId,
					Score = new Score
					{
						MaxRows = this.Handler.HighestRow
					},
					UserId = this.Handler.UserId
				};
				this.InitializeLeaderboardItem(0, entry);
				this.HandleViewSize(3);
			}
			this.itemList.EndAdding();
		}

		private void InitializeLeaderboardItem(int i, Entry entry)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.leaderboardItemPrefab);
			gameObject.SetActive(true);
			gameObject.GetComponent<BoxCollider>().enabled = false;
			UIElement component = gameObject.GetComponent<UIElement>();
			this.itemList.AddToContent(component);
			EndlessChallengeLeaderboardItem component2 = gameObject.GetComponent<EndlessChallengeLeaderboardItem>();
			bool flag = entry.IsOwnedByDeviceOrUser(this.Handler.DeviceId, this.Handler.UserId);
			component2.Initialize(i + 1, entry, flag);
			if (flag)
			{
				component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, -8f);
			}
		}

		private void HandleViewSize(int scoreEntries)
		{
			int num = Mathf.Clamp(scoreEntries, 0, 5) - 3;
			float y = this.leaderboardItemPrefab.GetComponent<UIElement>().Size.y;
			float max = base.GetElementSize().y * 0.9f;
			float num2 = Mathf.Clamp(y * ((float)num - 0.4f), float.MinValue, max);
			this.itemList.GetElement().Size += new Vector2(0f, num2);
			this.itemList.GetElement().LocalPosition -= new Vector2(0f, num2 / 2f);
			this.frame.Size = new Vector2(this.frame.Size.x, Mathf.Clamp(this.frame.Size.y + num2, 0f, max));
		}

		[UsedImplicitly]
		private void OnPlayClicked(UIEvent e)
		{
			this.Handler.StartFlow();
			base.Close(0);
		}

		[UsedImplicitly]
		private void OnCloseClicked(UIEvent e)
		{
			this.itemList.DestroyAllContent();
			base.Close(0);
		}

		private const int DEFAULT_SCORE_ENTRIES = 3;

		private const int MAX_SCORE_ENTRIES = 5;

		[SerializeField]
		private UIElement frame;

		[SerializeField]
		private UIListPanel itemList;

		[SerializeField]
		private GameObject leaderboardItemPrefab;

		[SerializeField]
		private UIElement notConnectedPivot;
	}
}
