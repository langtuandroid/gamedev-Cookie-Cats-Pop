using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.Surveys.Data;

public class SurveyManager
{
	public SurveyManager(FeatureManager featureManager, MapPopupManager mapPopupManager, SurveyManager.ISurveyProvider provider)
	{
		if (featureManager == null)
		{
			throw new ArgumentNullException("featureManager");
		}
		if (mapPopupManager == null)
		{
			throw new ArgumentNullException("mapPopupManager");
		}
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		foreach (Type type in this.FeatureHandlerTypes)
		{
			IFeatureTypeHandler item = (IFeatureTypeHandler)Activator.CreateInstance(type, new object[]
			{
				featureManager
			});
			this.featureHandlers.Add(item);
		}
		this.provider = provider;
		SurveyManagerPopups.SurveyNormalPopup popup = new SurveyManagerPopups.SurveyNormalPopup(this, new SurveyLocalizationData(), featureManager, this.GetHandler<SurveyNormalFeatureTypeHandler>());
		mapPopupManager.RegisterPopupObject(popup);
	}

	public T GetHandler<T>()
	{
		foreach (IFeatureTypeHandler featureTypeHandler in this.GetSurveyHandlers())
		{
			if (featureTypeHandler is T)
			{
				return (T)((object)featureTypeHandler);
			}
		}
		throw new Exception("FeatureHandler not found!");
	}

	public IEnumerable<IFeatureTypeHandler> GetSurveyHandlers()
	{
		foreach (IFeatureTypeHandler handler in this.featureHandlers)
		{
			yield return handler;
		}
		yield break;
	}

	public ActivatedFeatureInstanceData GetSurveyToShow(IFeatureTypeHandler featureTypeHandler)
	{
		using (List<ActivatedFeatureInstanceData>.Enumerator enumerator = featureTypeHandler.GetActivatedFeatures().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public void SendIntroAnswer(string question, string answer)
	{
		SurveyResultData surveyResultData = this.CreateSurveyResultData(0);
		AnswerResultData item = new AnswerResultData
		{
			Question = question,
			Answer = answer
		};
		surveyResultData.AnswerResults = new List<AnswerResultData>
		{
			item
		};
		SurveyAnalytics.LogSurveyResultsEvent(surveyResultData.SurveyId, surveyResultData.SurveyName, surveyResultData.PageNumber, surveyResultData.GetAnswersAsJSON());
	}

	public void SendPageAnswers(int pageNumber, List<KeyValuePair<string, string>> answers)
	{
		SurveyResultData surveyResultData = this.CreateSurveyResultData(pageNumber);
		List<AnswerResultData> list = new List<AnswerResultData>();
		surveyResultData.AnswerResults = list;
		foreach (KeyValuePair<string, string> keyValuePair in answers)
		{
			list.Add(new AnswerResultData
			{
				Question = keyValuePair.Key,
				Answer = keyValuePair.Value
			});
		}
		SurveyAnalytics.LogSurveyResultsEvent(surveyResultData.SurveyId, surveyResultData.SurveyName, surveyResultData.PageNumber, surveyResultData.GetAnswersAsJSON());
	}

	private SurveyResultData CreateSurveyResultData(int pageNumber)
	{
		return new SurveyResultData
		{
			SurveyId = this.currentSurveyData.Id,
			SurveyName = this.currentSurveyData.GetMetaData<SurveyMetaData>().Name,
			PageNumber = pageNumber
		};
	}

	private readonly List<Type> FeatureHandlerTypes = new List<Type>
	{
		typeof(SurveyNormalFeatureTypeHandler)
	};

	private readonly List<IFeatureTypeHandler> featureHandlers = new List<IFeatureTypeHandler>();

	public readonly SurveyManager.ISurveyProvider provider;

	public ActivatedFeatureInstanceData currentSurveyData;

	public interface ISurveyProvider
	{
		IEnumerator ShowRewards(SurveyRewardData reward);
	}
}
