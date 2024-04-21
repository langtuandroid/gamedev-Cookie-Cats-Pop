using System;

namespace TactileModules.PuzzleGame.Surveys.Data
{
	public class SurveyLocalizationData
	{
		public string GetAnswerButtonText(SurveyAnswerData surveyAnswerData)
		{
			string answer = surveyAnswerData.Answer;
			if (answer == "Disagree")
			{
				return L.Get("Disagree");
			}
			if (answer == "Neither agree nor disagree")
			{
				return L.Get("Neither agree nor disagree");
			}
			if (answer == "Agree")
			{
				return L.Get("Agree");
			}
			return answer;
		}

		public string GetNextButtonText()
		{
			return L.Get("Next");
		}

		public string GetFinishButtonText()
		{
			return L.Get("Finish");
		}
	}
}
