using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankOfferController : IFiberRunnable
	{
		public PiggyBankOfferController(IPiggyBankViewFactory viewFactory, IPiggyBankRewards rewards, PiggyBankControllerFactory controllerFactory)
		{

			this.viewFactory = viewFactory;
			this.rewards = rewards;
			this.controllerFactory = controllerFactory;
		}

		public IEnumerator Run()
		{
			UIViewManager.UIViewState vs = this.viewFactory.ShowView<PiggyBankOfferView>(false);
			this.view = (PiggyBankOfferView)vs.View;
			this.view.Initialize( this.rewards.Capacity, this.rewards.MaxCapacity, this.rewards.AvailableCapacityIncrease);
			yield return vs.WaitForClose();
			yield break;
		}

		private void HandleInAppPurchaseStatusSuccess()
		{
			this.ClaimOffer();
			this.animationFiber.Start(this.view.AnimateItemsToInventory(new List<ItemAmount>()));
		}

		private void PurchaseCancelledHandler()
		{
			this.view.PurchaseCancelled();
		}
		

		public void OnExit()
		{
			this.fiber.Terminate();
			this.animationFiber.Terminate();
		}

		private void ClaimOffer()
		{
			this.rewards.IncreaseCapacity();
			//this.rewards.GiveOfferItemsToPlayer(this.iapProvider.GetOfferItems());
			this.rewards.SavePersistabelState();
		}

		private readonly PiggyBankControllerFactory controllerFactory;

		private readonly IPiggyBankViewFactory viewFactory;

		private readonly IPiggyBankRewards rewards;

		private PiggyBankOfferView view;

		private Fiber fiber = new Fiber();

		private Fiber animationFiber = new Fiber();
	}
}
