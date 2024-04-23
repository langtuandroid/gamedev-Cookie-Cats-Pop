using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.OneLifeChallenge
{
	public class OneLifeChallengeMapButtonsView : UIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ExitClicked;



		public void Initialize(OneLifeChallengeManager manager)
		{
			this.manager = manager;
		}

		[UsedImplicitly]
		private void ExitButtonClicked(UIEvent e)
		{
			this.ExitClicked();
		}

		private void Update()
		{
			if (this.manager == null)
			{
				return;
			}
			this.timeLeftLabel.text = this.manager.TimeLeftAsText;
			if (UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
			{
				this.ExitClicked();
			}
		}

		[SerializeField]
		private UILabel timeLeftLabel;

		private OneLifeChallengeManager manager;
	}
}
