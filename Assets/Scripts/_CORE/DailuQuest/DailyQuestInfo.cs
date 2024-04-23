using System;
using System.Collections.Generic;
using ConfigSchema;

public class DailyQuestInfo
{
	[JsonSerializable("Title", null)]
	public string Title { get; set; }

	[JsonSerializable("Reward", typeof(ItemPackage))]
	[Obsolete("Use 'Rewards' property instead. This requires change on configuration scheme")]
	public ItemPackage Reward { get; set; }

	[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
	[JsonSerializable("Rewards", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }
}
