using System;
using System.Collections;
using Fibers;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankOfferController : IFiberRunnable
	{
		public PiggyBankOfferController(IIAPProvider iapProvider, IPiggyBankViewFactory viewFactory, IPiggyBankRewards rewards, PiggyBankControllerFactory controllerFactory)
		{
			this.iapProvider = iapProvider;
			this.viewFactory = viewFactory;
			this.rewards = rewards;
			this.controllerFactory = controllerFactory;
		}

		public IEnumerator Run()
		{
			UIViewManager.UIViewState vs = this.viewFactory.ShowView<PiggyBankOfferView>(false);
			this.view = (PiggyBankOfferView)vs.View;
			this.view.BuyOfferClicked += this.BuyOfferClickedHandler;
			this.view.Initialize(this.iapProvider, this.rewards.Capacity, this.rewards.MaxCapacity, this.rewards.AvailableCapacityIncrease);
			yield return vs.WaitForClose();
			yield break;
		}

		private void HandleInAppPurchaseStatusSuccess()
		{
			this.ClaimOffer();
			this.animationFiber.Start(this.view.AnimateItemsToInventory(this.iapProvider.GetOfferItems()));
		}

		private void PurchaseCancelledHandler()
		{
			this.view.PurchaseCancelled();
		}

		private void BuyOfferClickedHandler()
		{
			PiggyBankIAPController piggyBankIAPController = this.controllerFactory.CreateIAPController();
			InAppProduct offerInAppProduct = this.iapProvider.GetOfferInAppProduct();
			PiggyBankIAPController piggyBankIAPController2 = piggyBankIAPController;
			piggyBankIAPController2.OnClaimedOffer = (PiggyBankIAPController.ClaimedOfferEvent)Delegate.Combine(piggyBankIAPController2.OnClaimedOffer, new PiggyBankIAPController.ClaimedOfferEvent(this.HandleInAppPurchaseStatusSuccess));
			PiggyBankIAPController piggyBankIAPController3 = piggyBankIAPController;
			piggyBankIAPController3.OnInAppPurchaseCancelled = (PiggyBankIAPController.InAppPurchaseCancelledEvent)Delegate.Combine(piggyBankIAPController3.OnInAppPurchaseCancelled, new PiggyBankIAPController.InAppPurchaseCancelledEvent(this.PurchaseCancelledHandler));
			this.fiber.Start(piggyBankIAPController.Buy(offerInAppProduct));
		}

		public void OnExit()
		{
			this.fiber.Terminate();
			this.animationFiber.Terminate();
		}

		private void ClaimOffer()
		{
			this.rewards.IncreaseCapacity();
			this.rewards.GiveOfferItemsToPlayer(this.iapProvider.GetOfferItems());
			this.rewards.SavePersistabelState();
		}

		private readonly PiggyBankControllerFactory controllerFactory;

		private readonly InAppPurchaseManager inAppPurchaseManager;

		private readonly IIAPProvider iapProvider;

		private readonly IPiggyBankViewFactory viewFactory;

		private readonly IPiggyBankRewards rewards;

		private PiggyBankOfferView view;

		private Fiber fiber = new Fiber();

		private Fiber animationFiber = new Fiber();
	}
}
