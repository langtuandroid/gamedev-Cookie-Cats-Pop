using System;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public class PiggyBankTutorialView : UIView
	{
		public void Initialize(int nextFreeOpenLevelHumanNumber, int nextFreeOpenInterval, int coinsRequiredForPaidOpening)
		{
			IPiggyBankTutorialViewText component = base.GetComponent<IPiggyBankTutorialViewText>();
			GameObject instance = this.closeButtonUIInstantiator.GetInstance();
			this.closeButton = instance.GetComponentInChildren<UIButton>();
			this.okButton = this.okButtonUIInstantiator.GetInstance<UIButton>();
			bool flag = component != null;
			this.nextFreeOpenLabel.text = ((!flag) ? this.GenericNextFreeOpenText(nextFreeOpenLevelHumanNumber) : component.NextFreeOpenText(nextFreeOpenLevelHumanNumber));
			this.howItWorksLabel.text = ((!flag) ? this.GenericHowItWorksText(nextFreeOpenInterval) : component.HowItWorksText(nextFreeOpenInterval));
			this.buyOpenDescriptionLabel.text = ((!flag) ? this.GenericBuyToOpenDescriptionText(coinsRequiredForPaidOpening) : component.BuyToOpenDescriptionText(coinsRequiredForPaidOpening));
			this.closeButton.Clicked += this.CloseButtonClickedHandler;
			this.okButton.Clicked += this.CloseButtonClickedHandler;
		}

		private void CloseButtonClickedHandler(UIButton uiButton)
		{
			base.Close(0);
		}

		private string GenericNextFreeOpenText(int nextFreeOpenHumanNumber)
		{
			return string.Format(L.Get("Next free open at level {0}"), nextFreeOpenHumanNumber);
		}

		private string GenericHowItWorksText(int nextFreeOpenInterval)
		{
			return string.Format(L.Get("The Piggy collects coins when you play. Claim the coins for free at every {0}th level!"), nextFreeOpenInterval);
		}

		private string GenericBuyToOpenDescriptionText(int coinsRequiredForPaidOpening)
		{
			return string.Format(L.Get("Can't wait? Collect at least {0} coins to unlock the Piggy and increase its max capacity for a small fee!"), coinsRequiredForPaidOpening);
		}

		[SerializeField]
		private UIInstantiator closeButtonUIInstantiator;

		[SerializeField]
		private UIInstantiator okButtonUIInstantiator;

		[SerializeField]
		private UILabel nextFreeOpenLabel;

		[SerializeField]
		private UILabel howItWorksLabel;

		[SerializeField]
		private UILabel buyOpenDescriptionLabel;

		private UIButton closeButton;

		private UIButton okButton;
	}
}
