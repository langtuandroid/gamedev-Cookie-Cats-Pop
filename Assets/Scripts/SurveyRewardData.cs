using System;
using System.Collections.Generic;
using ConfigSchema;

[RequireAll]
[ObsoleteJsonName("Package")]
public class SurveyRewardData
{
	[Description("Reward text")]
	[JsonSerializable("Text", null)]
	public string Text { get; set; }

	[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
	[JsonSerializable("Items", typeof(ItemAmount))]
	public List<ItemAmount> Items { get; set; }
}
