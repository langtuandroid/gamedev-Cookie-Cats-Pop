using System;
using ConfigSchema;

public class CustomLimitedBoosterConfig
{
	[JsonSerializable("PaintbrushRows", null)]
	[Description("How many rows should the paintbrush be applied to")]
	public int PaintbrushRows { get; set; }
}
