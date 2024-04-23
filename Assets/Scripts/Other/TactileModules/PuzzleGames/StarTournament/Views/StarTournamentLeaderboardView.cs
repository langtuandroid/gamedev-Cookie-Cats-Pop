using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament.Views
{
	public class StarTournamentLeaderboardView : UIView
	{
		private StarTournamentLeaderboardItem myItem
		{
			get
			{
				return this.myItemInstantiator.GetInstance<StarTournamentLeaderboardItem>();
			}
		}

		public void Initialize(StarTournamentManager starTournamentManager, IMainProgression mainProgression, CloudClientBase cloudClient, FacebookClient facebookClient)
		{
			this.manager = starTournamentManager;
			this.facebookClient = facebookClient;
			this.cloudClient = cloudClient;
			this.mainProgression = mainProgression;
			UIListPanel uilistPanel = this.itemList;
			uilistPanel.OnCellVisibilityChanged = (Action<UIElement, bool>)Delegate.Combine(uilistPanel.OnCellVisibilityChanged, new Action<UIElement, bool>(this.OnCellVisibilityChanged));
			this.UpdateTable();
		}

		protected override void ViewWillDisappear()
		{
			UIListPanel uilistPanel = this.itemList;
			uilistPanel.OnCellVisibilityChanged = (Action<UIElement, bool>)Delegate.Remove(uilistPanel.OnCellVisibilityChanged, new Action<UIElement, bool>(this.OnCellVisibilityChanged));
		}

		private void UpdateTable()
		{
			this.itemList.DestroyAllContent();
			this.itemList.BeginAdding();
			string deviceId = (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
			string userId = (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
			List<Entry> allParticipants = this.manager.GetAllParticipants();
			for (int i = 0; i < allParticipants.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.elementPrefab);
				gameObject.SetActive(true);
				gameObject.GetComponent<BoxCollider>().enabled = false;
				UIElement component = gameObject.GetComponent<UIElement>();
				this.itemList.AddToContent(component);
				StarTournamentLeaderboardItem component2 = gameObject.GetComponent<StarTournamentLeaderboardItem>();
				bool flag = allParticipants[i].IsOwnedByDeviceOrUser(deviceId, userId);
				component2.Init(this.manager, this.mainProgression, this.facebookClient, this.cloudClient, i + 1, allParticipants[i], flag);
				if (flag)
				{
					component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, -8f);
					this.focusPosition = new Vector2(gameObject.transform.localPosition.x, component2.GetElementSize().y * (float)i);
					this.focusSize = component2.GetElementSize();
					this.myCellElement = component;
					this.myItem.Init(this.manager, this.mainProgression, this.facebookClient, this.cloudClient, i + 1, allParticipants[i], true);
					this.myItem.gameObject.SetActive(false);
				}
			}
			this.itemList.EndAdding();
			this.loadingSpinner.SetActive(false);
		}

		private void OnCellVisibilityChanged(UIElement element, bool enable)
		{
			if (this.myCellElement == element)
			{
				this.myItem.gameObject.SetActive(!enable);
			}
		}

		private void Update()
		{
			if (this.manager != null)
			{
				this.timerLabel.text = this.manager.GetTimeRemainingForStarTournamentAsString();
			}
		}

		private void OnMyItemClicked(UIEvent e)
		{
			this.itemList.SetScrollAnimated(new Vector2(0f, -(this.itemList.TotalContentSize.y - (this.focusPosition.y - this.focusSize.y * 0.5f))), 0.25f, null);
		}

		private void OnCloseClicked(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UIListPanel itemList;

		[SerializeField]
		private GameObject elementPrefab;

		[SerializeField]
		private GameObject loadingSpinner;

		[SerializeField]
		private UILabel timerLabel;

		[SerializeField]
		private UIInstantiator myItemInstantiator;

		private UIElement myCellElement;

		private Vector2 focusPosition;

		private Vector2 focusSize;

		private StarTournamentManager manager;

		private CloudClientBase cloudClient;

		private FacebookClient facebookClient;

		private IMainProgression mainProgression;
	}
}
