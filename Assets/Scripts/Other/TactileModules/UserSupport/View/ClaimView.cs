using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class ClaimView : UIView, IClaimView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Closed;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Claimed;



		public void Show()
		{
		}

		[UsedImplicitly]
		public void Claim(UIEvent e)
		{
			this.Claimed();
			base.Close(1);
		}

		[UsedImplicitly]
		public void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private GameObject claimButton;
	}
}
