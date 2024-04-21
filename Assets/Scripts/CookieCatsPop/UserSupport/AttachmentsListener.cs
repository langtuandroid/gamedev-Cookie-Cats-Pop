using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.UserSupport;

namespace CookieCatsPop.UserSupport
{
	public class AttachmentsListener : IAttachmentsListener
	{
		public AttachmentsListener(UIViewManager uiViewManager, InventoryManager inventoryManager, UserSettingsManager userSettings)
		{
			this.uiViewManager = uiViewManager;
			this.inventoryManager = inventoryManager;
			this.userSettings = userSettings;
		}

		public void AttachmentClaimed(List<ItemAmount> attachments, Action claimedCelebrationCompleteCallback)
		{
			FiberCtrl.Pool.Run(this.DoShowCelebration(attachments, claimedCelebrationCompleteCallback), false);
		}

		private IEnumerator DoShowCelebration(List<ItemAmount> attachments, Action claimedCelebreationCompleteCallback)
		{
			UIViewManager.UIViewStateGeneric<GiveRewardsView> viewState = this.uiViewManager.ShowView<GiveRewardsView>(new object[0]);
			viewState.View.Initialize(L.Get("Thank you for your patience!"));
			yield return viewState.View.GiveRewardsAndClose(attachments);
			yield return viewState.WaitForClose();
			foreach (ItemAmount itemAmount in attachments)
			{
				this.inventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, "UserSupport");
			}
			claimedCelebreationCompleteCallback();
			yield break;
		}

		public void AdsOffClaimed()
		{
			this.userSettings.GetSettings<InAppPurchaseManager.PersistableState>().IsPayingUser = true;
		}

		private readonly UIViewManager uiViewManager;

		private readonly InventoryManager inventoryManager;

		private readonly IUserSettings userSettings;
	}
}
