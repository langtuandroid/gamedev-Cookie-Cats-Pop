using System;
using ConfigSchema;

[RequireAll]
public class SurveyButtonData
{
	[JsonSerializable("Label", null)]
	public string Label { get; set; }

	[StringEnum(new string[]
	{
		"continue",
		"cancel"
	})]
	[JsonSerializable("Action", null)]
	public string Action { get; set; }

	public const string ACTION_CONTINUE = "continue";

	public const string ACTION_CANCEL = "cancel";
}
