using System;
using System.Collections.Generic;
using ConfigSchema;

[RequireAll]
public class SurveyIntroData
{
	[Description("Text shown on the survey intro view")]
	[JsonSerializable("Text", null)]
	public string Text { get; set; }

	[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
	[JsonSerializable("Buttons", typeof(SurveyButtonData))]
	public List<SurveyButtonData> Buttons { get; set; }
}
