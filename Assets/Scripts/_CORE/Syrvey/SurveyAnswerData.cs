using System;
using ConfigSchema;

[RequireAll]
public class SurveyAnswerData
{
	[StringEnum(new string[]
	{
		"Disagree",
		"Neither agree nor disagree",
		"Agree"
	})]
	[JsonSerializable("Answer", null)]
	public string Answer { get; set; }

	public const string DISAGREE = "Disagree";

	public const string NEITHER = "Neither agree nor disagree";

	public const string AGREE = "Agree";
}
