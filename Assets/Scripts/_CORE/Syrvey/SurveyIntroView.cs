using System;
using System.Collections.Generic;
using UnityEngine;

public class SurveyIntroView : UIView
{
	public void Initialize(string questionText, List<SurveyButtonData> answerButtons, SurveyManager pSurveyManager)
	{
		this.surveyManager = pSurveyManager;
		this.questionLabel.text = questionText;
		this.positiveButtonInstantiator.gameObject.SetActive(false);
		this.negativeButtonInstantiator.gameObject.SetActive(false);
		int count = answerButtons.Count;
		this.grid.numColums = count;
		for (int i = 0; i < count; i++)
		{
			SurveyButtonData surveyButtonData = answerButtons[i];
			UIButton instance;
			if (surveyButtonData.Action == "continue")
			{
				this.positiveButtonInstantiator.gameObject.SetActive(true);
				this.positiveButtonInstantiator.gameObject.name = string.Format("{0}_{1}", i, surveyButtonData.Label);
				instance = this.positiveButtonInstantiator.GetInstance<UIButton>();
				instance.methodName = "OnPositiveOptionClick";
			}
			else
			{
				this.negativeButtonInstantiator.gameObject.SetActive(true);
				this.negativeButtonInstantiator.gameObject.name = string.Format("{0}_{1}", i, surveyButtonData.Label);
				instance = this.negativeButtonInstantiator.GetInstance<UIButton>();
				instance.methodName = "OnNegativeOptionClick";
			}
			instance.receiver = base.gameObject;
			instance.Payload = surveyButtonData.Label;
			instance.GetComponentInChildren<UILabel>().text = surveyButtonData.Label;
		}
		this.grid.Layout();
	}

	private void OnPositiveOptionClick(UIEvent e)
	{
		string answer = (string)e.payload;
		this.surveyManager.SendIntroAnswer(this.questionLabel.text, answer);
		base.Close(1);
	}

	private void OnNegativeOptionClick(UIEvent e)
	{
		string answer = (string)e.payload;
		this.surveyManager.SendIntroAnswer(this.questionLabel.text, answer);
		base.Close(0);
	}

	private void OnCloseClicked(UIEvent e)
	{
		this.surveyManager.SendIntroAnswer(this.questionLabel.text, "Cancel");
		base.Close(0);
	}

	[SerializeField]
	private UILabel questionLabel;

	[SerializeField]
	private UIGridLayout grid;

	[SerializeField]
	private UIInstantiator positiveButtonInstantiator;

	[SerializeField]
	private UIInstantiator negativeButtonInstantiator;

	private SurveyManager surveyManager;
}
