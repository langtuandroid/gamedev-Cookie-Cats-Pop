using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public class PiggyBankOfferView : ExtensibleView<IPiggyBankOfferViewExtension>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action BuyOfferClicked;



		public void Initialize(int capacity, int maxCapacity, int availableCapacityIncrease)
		{
			IPiggyBankOfferViewText component = base.GetComponent<IPiggyBankOfferViewText>();
			GameObject instance = this.closeButtonUIInstantiator.GetInstance();
			this.closeButton = instance.GetComponentInChildren<UIButton>();
			this.buyButton = this.buyButtonUIInstantiator.GetInstance<UIButton>();
			this.closeButton.Clicked += this.CloseButtonClickedHandler;
			this.buyButton.Clicked += this.BuyButtonClickedHandler;
			this.DisableBuyButton(false);
			if (base.Extension != null)
			{
				base.Extension.Initialize(new List<ItemAmount>());
			}
			this.SetBuyButtonLabelText("0");
			this.capacityIncreaseBadge.SetActive(capacity < maxCapacity);
			this.capacityIncreaseLabel.text = "+" + availableCapacityIncrease;
			bool flag = component != null;
			this.descriptionLabel.text = ((!flag) ? this.GenericDescriptionLabelText(new List<ItemAmount>(), availableCapacityIncrease) : component.GetDescriptionLabelText(new List<ItemAmount>(), availableCapacityIncrease));
		}

		private void CloseButtonClickedHandler(UIButton uiButton)
		{
			base.Close(0);
		}

		private void BuyButtonClickedHandler(UIButton uiButton)
		{
			this.DisableBuyButton(true);
			this.buyButton.Clicked -= this.BuyButtonClickedHandler;
			this.BuyOfferClicked();
		}

		private void DisableBuyButton(bool disable)
		{
			this.buyButton.gameObject.SetActive(!disable);
			this.buyButtonDisabled.gameObject.SetActive(disable);
		}

		private void SetBuyButtonLabelText(string price)
		{
			UILabel componentInChildren = this.buyButton.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = string.Format(L.Get("Buy {0}"), price);
			}
		}

		public IEnumerator AnimateItemsToInventory(List<ItemAmount> offerItems)
		{
			if (base.Extension != null)
			{
				yield return new Fiber.OnExit(delegate()
				{
					UICamera.EnableInput();
				});
				UICamera.DisableInput();
				yield return base.Extension.AnimateItemsToInventory(offerItems);
			}
			base.Close(0);
			yield break;
		}

		public void PurchaseCancelled()
		{
			this.DisableBuyButton(false);
			this.buyButton.Clicked += this.BuyButtonClickedHandler;
		}

		protected override void ViewWillDisappear()
		{
			this.buyFiber.Terminate();
			this.animationFiber.Terminate();
		}

		public string GenericDescriptionLabelText(List<ItemAmount> offerItems, int capacityIncrease)
		{
			int num = 0;
			foreach (ItemAmount itemAmount in offerItems)
			{
				num += itemAmount.Amount;
			}
			string result = string.Format(L.Get("Increase Piggy Bank coin capacity by {0} coins + {1} boosters."), capacityIncrease, num);
			if (capacityIncrease <= 0)
			{
				result = string.Format(L.Get("Get {0} boosters."), num);
			}
			return result;
		}

		[SerializeField]
		private UIInstantiator closeButtonUIInstantiator;

		[SerializeField]
		private UIInstantiator buyButtonUIInstantiator;

		[SerializeField]
		private UISprite buyButtonDisabled;

		[SerializeField]
		private GameObject capacityIncreaseBadge;

		[SerializeField]
		private UILabel descriptionLabel;

		[SerializeField]
		private UILabel capacityIncreaseLabel;

		private Fiber buyFiber = new Fiber();

		private Fiber animationFiber = new Fiber();

		private UIButton closeButton;

		private UIButton buyButton;
	}
}
