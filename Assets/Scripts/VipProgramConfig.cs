using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("VipProgramConfig")]
public class VipProgramConfig
{
	[JsonSerializable("LevelRequiredForVip", null)]
	public int LevelRequiredForVip { get; set; }

	[JsonSerializable("StartReward", null)]
	public VipProgramConfigDailyReward StartReward { get; set; }

	[JsonSerializable("DailyRewards", typeof(VipProgramConfigDailyReward))]
	public List<VipProgramConfigDailyReward> DailyRewards { get; set; }
}
