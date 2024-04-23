using System;
using JetBrains.Annotations;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament.Views
{
	public class StarTournamentStartView : UIView
	{
		public void Initialize(StarTournamentManager manager, IMainProgression mainProgression, CloudClientBase cloudClient, FacebookClient facebookClient, bool isReminder)
		{
			this.manager = manager;
			this.mainProgression = mainProgression;
			this.cloudClient = cloudClient;
			this.facebookClient = facebookClient;
			this.isReminder = isReminder;
			if (isReminder)
			{
				UILabel componentInChildren = this.buttonInstantiator.GetInstance<UIElement>().GetComponentInChildren<UILabel>();
				componentInChildren.text = L.Get(this.reminderButtonText);
			}
		}

		private void Update()
		{
			this.timerLabel.text = this.manager.GetTimeRemainingForStarTournamentAsString();
		}

		[UsedImplicitly]
		private void BeginClicked(UIEvent e)
		{
			if (this.isReminder)
			{
				base.Close(0);
			}
			else
			{
				UIViewManager.UIViewStateGeneric<StarTournamentLeaderboardView> uiviewStateGeneric = UIViewManager.Instance.ShowView<StarTournamentLeaderboardView>(new object[0]);
				uiviewStateGeneric.View.Initialize(this.manager, this.mainProgression, this.cloudClient, this.facebookClient);
				base.Close(uiviewStateGeneric);
			}
		}

		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UILabel timerLabel;

		[SerializeField]
		private UIInstantiator buttonInstantiator;

		[SerializeField]
		private string reminderButtonText = "Continue";

		private bool isReminder;

		private StarTournamentManager manager;

		private IMainProgression mainProgression;

		private CloudClientBase cloudClient;

		private FacebookClient facebookClient;
	}
}
