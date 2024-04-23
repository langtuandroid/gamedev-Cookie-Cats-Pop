using System;
using System.Collections.Generic;
using ConfigSchema;

[RequireAll]
public class SurveyQuestionData
{
	public int Id { get; set; }

	[Description("The question")]
	[JsonSerializable("Question", null)]
	public string Question { get; set; }

	[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
	[Description("Available answers")]
	[JsonSerializable("Answers", typeof(SurveyAnswerData))]
	public List<SurveyAnswerData> Answers { get; set; }
}
