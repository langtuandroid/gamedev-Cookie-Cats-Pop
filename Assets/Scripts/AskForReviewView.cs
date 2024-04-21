using System;
using Tactile;
using UnityEngine;

public class AskForReviewView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.checkBoxOn.gameObject.SetActive(false);
		ReviewConfig reviewConfig = ConfigurationManager.Get<ReviewConfig>();
		this.layoutAcceptanceReview.SetActive(reviewConfig.Layout == 0);
		this.layoutAskForReview.SetActive(reviewConfig.Layout != 0);
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(new AskForReviewView.Result
		{
			askForReviewViewResult = ReviewManagerBase.AskForReviewViewResult.EXIT,
			dontAskAgain = this.checkBoxOn.gameObject.activeSelf
		});
	}

	private void Button14Clicked(UIEvent e)
	{
		base.Close(new AskForReviewView.Result
		{
			askForReviewViewResult = ReviewManagerBase.AskForReviewViewResult.FOUR_OR_LESS,
			dontAskAgain = this.checkBoxOn.gameObject.activeSelf
		});
	}

	private void Button5Clicked(UIEvent e)
	{
		base.Close(new AskForReviewView.Result
		{
			askForReviewViewResult = ReviewManagerBase.AskForReviewViewResult.FIVE_STARS,
			dontAskAgain = this.checkBoxOn.gameObject.activeSelf
		});
	}

	private void CheckboxClicked(UIEvent e)
	{
		this.checkBoxOn.gameObject.SetActive(!this.checkBoxOn.gameObject.activeSelf);
	}

	public UISprite checkBoxOn;

	public GameObject layoutAcceptanceReview;

	public GameObject layoutAskForReview;

	public struct Result
	{
		public ReviewManagerBase.AskForReviewViewResult askForReviewViewResult;

		public bool dontAskAgain;
	}
}
