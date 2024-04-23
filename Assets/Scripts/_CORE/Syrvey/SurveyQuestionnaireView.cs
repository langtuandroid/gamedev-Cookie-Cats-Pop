using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleGame.Surveys.Data;
using UnityEngine;

public class SurveyQuestionnaireView : UIView
{
	public void Initialize(List<SurveyGroupData> surveyGroups, SurveyManager pSurveyManager, SurveyLocalizationData localizations)
	{
		this.localizations = localizations;
		this.nextButton = this.nextButtonInstantiator.GetInstance<SurveyButton>();
		this.nextButton.Initialize(localizations.GetNextButtonText(), base.gameObject, "OnNextClicked");
		this.ChangeNextButtonState(false);
		this.titleLabel.font = UIProjectSettings.Get().defaultFont;
		this.descriptionLabel.font = UIProjectSettings.Get().defaultFont;
		this.surveyManager = pSurveyManager;
		this.answeredQuestions = new Dictionary<int, KeyValuePair<string, string>>();
		this.isPageCompleted = false;
		for (int i = 0; i < surveyGroups.Count; i++)
		{
			this.totalPagesCount += Mathf.CeilToInt((float)surveyGroups[i].Questions.Count / (float)this.maxQuestionsPerPage);
		}
		this.InitPagesIndicator(this.totalPagesCount);
		this.fiber.Start(this.ShowSurvey(surveyGroups));
	}

	private void InitPagesIndicator(int pagesCount)
	{
		if (pagesCount < 2)
		{
			return;
		}
		this.pageIndicators = new List<SpriteStateHandler>(pagesCount);
		for (int i = 0; i < pagesCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.pageIndicatorPrefab);
			gameObject.transform.SetParent(this.pageIndicatorLayout.transform, false);
			gameObject.SetLayerRecursively(base.gameObject.layer);
			SpriteStateHandler component = gameObject.GetComponent<SpriteStateHandler>();
			component.SetState(false);
			this.pageIndicators.Add(component);
		}
		this.pageIndicatorLayout.Layout();
		this.pageIndicators.Sort((SpriteStateHandler g1, SpriteStateHandler g2) => (int)(g1.transform.position.x - g2.transform.position.x));
	}

	protected override void ViewWillDisappear()
	{
		this.fiber.Terminate();
	}

	private IEnumerator ShowSurvey(List<SurveyGroupData> surveyGroups)
	{
		List<SurveyMultiOptionsView> multiOptions = new List<SurveyMultiOptionsView>();
		this.currentPage = 0;
		for (int group = 0; group < surveyGroups.Count; group++)
		{
			SurveyGroupData surveyGroup = surveyGroups[group];
			surveyGroup.Questions.Shuffle<SurveyQuestionData>();
			int pagesCount = Mathf.CeilToInt((float)surveyGroup.Questions.Count / (float)this.maxQuestionsPerPage);
			int currentQuestion = 0;
			this.questionsPerPage = 0;
			for (int i = 0; i < pagesCount; i++)
			{
				for (int j = 0; j < multiOptions.Count; j++)
				{
					multiOptions[j].gameObject.SetActive(false);
				}
				this.nextButton.LabelText = ((this.currentPage == this.totalPagesCount - 1) ? this.localizations.GetFinishButtonText() : this.localizations.GetNextButtonText());
				if (this.pageIndicators != null && this.currentPage < this.pageIndicators.Count)
				{
					this.pageIndicators[this.currentPage].SetState(true);
				}
				this.answeredQuestions.Clear();
				this.nextButton.enabled = false;
				this.ChangeNextButtonState(false);
				this.isPageCompleted = false;
				this.titleLabel.text = surveyGroup.Header;
				this.descriptionLabel.text = surveyGroup.Subheader;
				this.questionsPerPage = Math.Min(this.maxQuestionsPerPage, surveyGroup.Questions.Count - currentQuestion);
				for (int k = 0; k < this.questionsPerPage; k++)
				{
					SurveyQuestionData surveyQuestionData = surveyGroup.Questions[currentQuestion];
					surveyQuestionData.Id = currentQuestion;
					SurveyMultiOptionsView surveyMultiOptionsView;
					if (k < multiOptions.Count)
					{
						surveyMultiOptionsView = multiOptions[k];
					}
					else
					{
						surveyMultiOptionsView = UnityEngine.Object.Instantiate<SurveyMultiOptionsView>(this.surveyMultipleOptionPrefab);
						surveyMultiOptionsView.gameObject.SetLayerRecursively(base.gameObject.layer);
						surveyMultiOptionsView.transform.SetParent(this.surveyMultipleOptionContainer, false);
						multiOptions.Add(surveyMultiOptionsView);
					}
					currentQuestion++;
					surveyMultiOptionsView.gameObject.SetActive(true);
					surveyMultiOptionsView.Initialize(surveyQuestionData, this.localizations, new Action<SurveyQuestionData, string>(this.OnChooseOption));
				}
				while (!this.isPageCompleted)
				{
					yield return null;
				}
				this.currentPage++;
			}
		}
		base.Close(1);
		yield break;
	}

	private void OnChooseOption(SurveyQuestionData question, string answer)
	{
		this.answeredQuestions[question.Id] = new KeyValuePair<string, string>(question.Question, answer);
		this.nextButton.enabled = (this.answeredQuestions.Count == this.questionsPerPage);
		this.ChangeNextButtonState(this.answeredQuestions.Count == this.questionsPerPage);
	}

	private void OnNextClicked(UIEvent e)
	{
		this.nextButton.enabled = false;
		this.ChangeNextButtonState(false);
		this.isPageCompleted = true;
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		foreach (KeyValuePair<string, string> item in this.answeredQuestions.Values)
		{
			list.Add(item);
		}
		this.surveyManager.SendPageAnswers(this.currentPage + 1, list);
	}

	private void ChangeNextButtonState(bool isActive)
	{
		this.nextButton.SetActiveButtonState(isActive);
		this.nextButton.SetActiveVisiualsState(isActive);
	}

	private void OnCloseClicked(UIEvent e)
	{
		this.fiber.Terminate();
		base.Close(0);
	}

	[SerializeField]
	private int maxQuestionsPerPage = 4;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private Transform surveyMultipleOptionContainer;

	[SerializeField]
	private SurveyMultiOptionsView surveyMultipleOptionPrefab;

	[SerializeField]
	private UIInstantiator nextButtonInstantiator;

	[SerializeField]
	private UIFlowLayout pageIndicatorLayout;

	[SerializeField]
	private GameObject pageIndicatorPrefab;

	private SurveyButton nextButton;

	private bool isPageCompleted;

	private int questionsPerPage;

	private Dictionary<int, KeyValuePair<string, string>> answeredQuestions;

	private List<SpriteStateHandler> pageIndicators;

	private Fiber fiber = new Fiber();

	private int currentPage;

	private SurveyManager surveyManager;

	private SurveyLocalizationData localizations;

	private int totalPagesCount;
}
