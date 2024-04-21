using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.SpecialOffers.Views
{
	public class SpecialOfferView : UIView, ISpecialOfferView, IUIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnAcceptButtonClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnDismissButtonClicked;



		public void SetTexture(Texture2D texture)
		{
			this.texture.SetTexture(texture);
		}

		public void SetTimeLeft(string time)
		{
			this.timeLeft.text = time;
		}

		[UsedImplicitly]
		public void AcceptButtonClicked(UIEvent e)
		{
			this.OnAcceptButtonClicked();
		}

		[UsedImplicitly]
		public void DismissButtonClicked(UIEvent e)
		{
			this.OnDismissButtonClicked();
		}

		[SerializeField]
		private UITextureQuad texture;

		[SerializeField]
		private UILabel timeLeft;
	}
}
