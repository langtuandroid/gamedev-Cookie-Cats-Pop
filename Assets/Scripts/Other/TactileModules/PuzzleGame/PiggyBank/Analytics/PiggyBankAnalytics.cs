using System;

namespace TactileModules.PuzzleGame.PiggyBank.Analytics
{
	public static class PiggyBankAnalytics
	{
		public static void LogContentClaimed(int content, int capacity)
		{
			TactileAnalytics.Instance.LogEvent(new PiggyBankAnalytics.PiggyBankContentClaimedEvent(content, capacity), -1.0, null);
		}

		public static void LogContentPurchased(int content, int capacity)
		{
			TactileAnalytics.Instance.LogEvent(new PiggyBankAnalytics.PiggyBankContentPurchasedEvent(content, capacity), -1.0, null);
		}

		private class PiggyBankBasicEvent : BasicEvent
		{
			protected PiggyBankBasicEvent(int content, int capacity)
			{
				this.Content = content;
				this.Capacity = capacity;
			}

			private TactileAnalytics.RequiredParam<int> Content { get; set; }

			private TactileAnalytics.RequiredParam<int> Capacity { get; set; }
		}

		[TactileAnalytics.EventAttribute("piggyBankContentClaimed", true)]
		private class PiggyBankContentClaimedEvent : PiggyBankAnalytics.PiggyBankBasicEvent
		{
			public PiggyBankContentClaimedEvent(int content, int capacity) : base(content, capacity)
			{
			}
		}

		[TactileAnalytics.EventAttribute("piggyBankContentPurchased", true)]
		private class PiggyBankContentPurchasedEvent : PiggyBankAnalytics.PiggyBankBasicEvent
		{
			public PiggyBankContentPurchasedEvent(int content, int capacity) : base(content, capacity)
			{
			}
		}
	}
}
