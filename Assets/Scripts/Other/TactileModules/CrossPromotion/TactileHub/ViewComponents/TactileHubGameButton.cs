using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.CrossPromotion.TactileHub.Models;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.ViewComponents
{
	public class TactileHubGameButton : MonoBehaviour
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnClick;



		public void Initialize(IHubGame tactileHubGame)
		{
			this.hubGame = tactileHubGame;
			this.SetTitle();
			this.SetIcon();
		}

		private void SetTitle()
		{
			this.titleLabel.text = this.hubGame.GetGameTitle();
		}

		private void SetIcon()
		{
			this.icon.SetTexture(this.hubGame.GetIconTexture());
		}

		[UsedImplicitly]
		private void ClickButton(UIEvent e)
		{
			this.OnClick();
		}

		[SerializeField]
		private UITextureQuad icon;

		[SerializeField]
		private UILabel titleLabel;

		private IHubGame hubGame;
	}
}
