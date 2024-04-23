using System;
using System.Collections;

namespace TactileModules.GameCore.Rewards
{
	public class GiftRewardsView : ExtensibleView<IGiftRewardsViewExtension>
	{
		public bool OpenGift { get; private set; }

		public void Initialize()
		{
			this.OpenGift = false;
		}

		public IEnumerator InitializeVisualGift(int giftType)
		{
			if (base.Extension != null)
			{
				yield return base.Extension.InitializeVisualGift(giftType);
			}
			yield break;
		}

		public IEnumerator AnimateOpenGift()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateOpenGift();
			}
			yield break;
		}

		public IEnumerator AnimateRewardsIn()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateRewardsIn();
			}
			yield break;
		}

		public IEnumerator AnimateRewardsOut()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateRewardsOut();
			}
			yield break;
		}

		public void HideButtons()
		{
			if (base.Extension != null)
			{
				base.Extension.HideButtons();
			}
		}

		public void Clicked(UIEvent e)
		{
			this.OpenGift = true;
		}
	}
}
