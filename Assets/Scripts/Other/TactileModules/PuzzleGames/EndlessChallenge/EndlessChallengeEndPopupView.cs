using System;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
	public class EndlessChallengeEndPopupView : UIView
	{
		private EndlessChallengeHandler Handler
		{
			get
			{
				return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
			}
		}

		protected override void ViewLoad(object[] parameters)
		{
			this.yourBestLabel.text = string.Format(L.Get("Your best:{0}"), this.Handler.HighestRow);
		}

		[UsedImplicitly]
		private void OnCloseClicked(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UILabel yourBestLabel;
	}
}
