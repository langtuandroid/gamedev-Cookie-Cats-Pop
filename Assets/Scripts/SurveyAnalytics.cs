using System;

public class SurveyAnalytics
{
	public static void LogSurveyResultsEvent(string surveyId, string surveyName, int surveyPageNumber, string surveyAnswersJson)
	{
		SurveyAnalytics.SurveyResults eventObject = new SurveyAnalytics.SurveyResults
		{
			surveyId = surveyId,
			surveyName = surveyName,
			surveyPage = surveyPageNumber,
			surveyAnswers = surveyAnswersJson
		};
		TactileAnalytics.Instance.LogEvent(eventObject, -1.0, null);
	}

	[TactileAnalytics.EventAttribute("surveyResults", true)]
	private class SurveyResults : BasicEvent
	{
		public TactileAnalytics.RequiredParam<string> surveyId { get; set; }

		public TactileAnalytics.RequiredParam<string> surveyName { get; set; }

		public TactileAnalytics.RequiredParam<int> surveyPage { get; set; }

		public TactileAnalytics.RequiredParam<string> surveyAnswers { get; set; }
	}
}
