using System;
using System.Collections.Generic;
using ConfigSchema;

[RequireAll]
public class SurveyGroupData
{
	[Description("Italic header shown on each questions page")]
	[JsonSerializable("Header", null)]
	public string Header { get; set; }

	[Description("Bold subheader shown on each questions page")]
	[JsonSerializable("Subheader", null)]
	public string Subheader { get; set; }

	[Description("List of questions in survey")]
	[JsonSerializable("Questions", typeof(SurveyQuestionData))]
	public List<SurveyQuestionData> Questions { get; set; }
}
