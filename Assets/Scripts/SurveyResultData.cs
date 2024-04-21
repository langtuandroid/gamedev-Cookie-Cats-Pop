using System;
using System.Collections.Generic;

public class SurveyResultData
{
	[JsonSerializable("surveyId", null)]
	public string SurveyId { get; set; }

	[JsonSerializable("surveyName", null)]
	public string SurveyName { get; set; }

	[JsonSerializable("page", null)]
	public int PageNumber { get; set; }

	[JsonSerializable("answers", typeof(AnswerResultData))]
	public List<AnswerResultData> AnswerResults { get; set; }

	public string GetAnswersAsJSON()
	{
		return JsonSerializer.GenericListToArrayList<AnswerResultData>(this.AnswerResults).toJson();
	}
}
