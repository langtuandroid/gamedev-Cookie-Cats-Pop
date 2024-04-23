using System;
using System.Diagnostics;
using Tactile;
using TactileModules.ComponentLifecycle;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class CurrencyOverlayLifecycleHandler : ComponentLifecycleHandler<StoryMapEventCurrencyOverlay>
	{
		public CurrencyOverlayLifecycleHandler(CurrencyAvailability currencyAvailability, InventoryManager inventoryManager) : base(ComponentLifecycleHandler<StoryMapEventCurrencyOverlay>.InitializationTiming.Awake)
		{
			this.currencyAvailability = currencyAvailability;
			this.inventoryManager = inventoryManager;
			inventoryManager.InventoryChanged += this.HandleInventoryChanged;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> CurrencyAmountChanged;



		protected override void InitializeComponent(StoryMapEventCurrencyOverlay component)
		{
			this.CurrencyAmountChanged += component.AmountChanged;
			component.AmountChanged(this.inventoryManager.GetAmount("Star"));
		}

		protected override void TeardownComponent(StoryMapEventCurrencyOverlay component)
		{
			this.CurrencyAmountChanged -= component.AmountChanged;
		}

		private void HandleInventoryChanged(InventoryManager.ItemChangeInfo info)
		{
			if (info.Item == "Star")
			{
				this.CurrencyAmountChanged(this.inventoryManager.GetAmount("Star"));
			}
		}

		private readonly CurrencyAvailability currencyAvailability;

		private readonly InventoryManager inventoryManager;
	}
}
