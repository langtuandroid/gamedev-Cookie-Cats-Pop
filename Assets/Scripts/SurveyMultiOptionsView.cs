using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.PuzzleGame.Surveys.Data;
using UnityEngine;

public class SurveyMultiOptionsView : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private event Action<SurveyQuestionData, string> onChooseOption;

	public void Initialize(SurveyQuestionData pQuestion, SurveyLocalizationData localizations, Action<SurveyQuestionData, string> onChooseOptionCallback)
	{
		this.statementLabel.font = UIProjectSettings.Get().defaultFont;
		this.question = pQuestion;
		this.localizations = localizations;
		this.onChooseOption = onChooseOptionCallback;
		this.statementLabel.text = this.question.Question;
		this.surveyButtons.Clear();
		this.surveyButtons.AddRange(this.surveyButtonsLayout.GetComponentsInChildren<SurveyButton>());
		for (int i = 0; i < this.surveyButtons.Count; i++)
		{
			this.surveyButtons[i].SetActiveVisiualsState(false);
			this.surveyButtons[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < this.question.Answers.Count; j++)
		{
			if (j >= this.surveyButtons.Count)
			{
				SurveyButton item = this.CreateSurveyButton();
				this.surveyButtons.Add(item);
			}
		}
		for (int k = 0; k < this.question.Answers.Count; k++)
		{
			SurveyAnswerData surveyAnswerData = this.question.Answers[k];
			this.surveyButtons[k].gameObject.SetActive(true);
			this.surveyButtons[k].gameObject.name = "SurveyButton" + k.ToString();
			this.surveyButtons[k].Initialize(localizations.GetAnswerButtonText(surveyAnswerData), base.gameObject, "OnOptionClick");
		}
		this.surveyButtonsLayout.Layout();
	}

	private SurveyButton CreateSurveyButton()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.surveyButtonPrefab);
		gameObject.transform.SetParent(this.surveyButtonsLayout.transform, false);
		gameObject.SetLayerRecursively(base.gameObject.layer);
		UIElement component = gameObject.GetComponent<UIElement>();
		component.Size = new Vector2(component.Size.x, this.surveyButtonsLayout.GetElementSize().y);
		SurveyButton component2 = gameObject.GetComponent<SurveyButton>();
		component2.SetActiveVisiualsState(false);
		return component2;
	}

	private void OnOptionClick(UIEvent e)
	{
		if (this.currentActiveButton != null)
		{
			this.currentActiveButton.SetActiveVisiualsState(false);
		}
		this.currentActiveButton = (SurveyButton)e.payload;
		this.currentActiveButton.SetActiveVisiualsState(true);
		if (this.onChooseOption != null)
		{
			this.onChooseOption(this.question, this.currentActiveButton.LabelText);
		}
	}

	[SerializeField]
	private UILabel statementLabel;

	[SerializeField]
	private UIFlowLayout surveyButtonsLayout;

	[SerializeField]
	private GameObject surveyButtonPrefab;

	private SurveyQuestionData question;

	private List<SurveyButton> surveyButtons = new List<SurveyButton>();

	private SurveyButton currentActiveButton;

	private SurveyLocalizationData localizations;
}
