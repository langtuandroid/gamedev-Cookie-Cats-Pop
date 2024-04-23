using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("BoosterConfig")]
public class BoosterConfig
{
	[JsonSerializable("UnlockAmounts", typeof(BoosterConfig.UnlockAmount))]
	public List<BoosterConfig.UnlockAmount> UnlockAmounts { get; set; }

	[JsonSerializable("UseSuperAimMonocular", typeof(bool))]
	public bool UseSuperAimMonocular { get; set; }

	public int GetUnlockAmount(string boosterId)
	{
		foreach (BoosterConfig.UnlockAmount unlockAmount in this.UnlockAmounts)
		{
			if (unlockAmount.BoosterId == boosterId)
			{
				return unlockAmount.Amount;
			}
		}
		return 0;
	}

	[JsonSerializable("ShowSuggestionOnFail", null)]
	public bool ShowSuggestionOnFail { get; set; }

	[JsonSerializable("SuggestionNeedToFailXTimes", null)]
	public int SuggestionNeedToFailXTimes { get; set; }

	[JsonSerializable("CustomLimitedBoosterConfig", typeof(CustomLimitedBoosterConfig))]
	public CustomLimitedBoosterConfig CustomLimitedBoosterConfig { get; set; }

	public class UnlockAmount
	{
		[JsonSerializable("BoosterId", null)]
		public string BoosterId { get; set; }

		[JsonSerializable("Amount", null)]
		public int Amount { get; set; }
	}
}
