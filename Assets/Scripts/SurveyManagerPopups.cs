using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGame.Surveys.Data;

public class SurveyManagerPopups
{
	public class SurveyNormalPopup : MapPopupManager.IMapPopup
	{
		public SurveyNormalPopup(SurveyManager surveyManager, SurveyLocalizationData localizations, FeatureManager featureManager, SurveyNormalFeatureTypeHandler featureTypeHandler)
		{
			this.localizations = localizations;
			this.surveyManager = surveyManager;
			this.featureManager = featureManager;
			this._featureTypeHandler = featureTypeHandler;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			ActivatedFeatureInstanceData surveyToShow = this.surveyManager.GetSurveyToShow(this._featureTypeHandler);
			if (surveyToShow != null)
			{
				popupFlow.AddPopup(this.StartSurvey(surveyToShow));
			}
		}

		private IEnumerator StartSurvey(ActivatedFeatureInstanceData instanceData)
		{
			this.surveyManager.currentSurveyData = instanceData;
			SurveyMetaData metaData = instanceData.GetMetaData<FeatureInstanceCustomData, SurveyMetaData, FeatureTypeCustomData>(this._featureTypeHandler);
			if (metaData.Groups.Count == 0 || metaData.Groups[0].Questions.Count == 0)
			{
				yield break;
			}
			UIViewManager.UIViewState vs = UIViewManager.Instance.ShowView<SurveyIntroView>(new object[0]);
			SurveyIntroView surveyIntroView = vs.View as SurveyIntroView;
			surveyIntroView.Initialize(metaData.Intro.Text, metaData.Intro.Buttons, this.surveyManager);
			yield return vs.WaitForClose();
			if ((int)vs.ClosingResult > 0)
			{
				vs = UIViewManager.Instance.ShowView<SurveyQuestionnaireView>(new object[0]);
				SurveyQuestionnaireView surveyQuestionnaireView = vs.View as SurveyQuestionnaireView;
				surveyQuestionnaireView.Initialize(metaData.Groups, this.surveyManager, this.localizations);
				yield return vs.WaitForClose();
				if ((int)vs.ClosingResult > 0)
				{
					yield return this.surveyManager.provider.ShowRewards(metaData.Reward);
				}
			}
			this.surveyManager.currentSurveyData = null;
			this.featureManager.DeactivateFeature(this._featureTypeHandler, instanceData);
			yield break;
		}

		private readonly SurveyManager surveyManager;

		private readonly FeatureManager featureManager;

		private readonly SurveyNormalFeatureTypeHandler _featureTypeHandler;

		private readonly SurveyLocalizationData localizations;
	}
}
