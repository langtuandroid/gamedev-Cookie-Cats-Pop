using System;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.Views
{
	public class PlayablePostcardInfoView : UIView
	{
		private MapFeatureHandler MapFeatureHandler
		{
			get
			{
				return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<PlayablePostcardHandler>();
			}
		}

		public void Initialize(bool isReminderView)
		{
			this.startDescription.gameObject.SetActive(!isReminderView);
			this.reminderDescription.gameObject.SetActive(isReminderView);
		}

		private void Update()
		{
			this.timeLeft.text = this.MapFeatureHandler.GetTimeLeftAsText();
		}

		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		[UsedImplicitly]
		private void Continue(UIEvent e)
		{
			base.Close(1);
		}

		[SerializeField]
		private UILabel timeLeft;

		[SerializeField]
		private UILabel startDescription;

		[SerializeField]
		private UILabel reminderDescription;
	}
}
