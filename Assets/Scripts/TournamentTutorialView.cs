using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class TournamentTutorialView : ExtensibleView<TournamentTutorialView.IExtension>
{
	private void Start()
	{
		this.continueButton.GetInstance().GetComponentInChildren<UIButton>().Clicked += this.ContinueButtonClicked;
	}

	public void StartShowingSteps(List<TournamentSetup.TutorialStep> steps)
	{
		this.fiber.Start(this.RunSteps(steps));
	}

	private IEnumerator RunSteps(List<TournamentSetup.TutorialStep> steps)
	{
		foreach (TournamentSetup.TutorialStep step in steps)
		{
			if (base.Extension != null)
			{
				base.Extension.SetTutorialText(step.LocalizedMessage);
			}
			yield return this.WaitForButtonToBeClicked();
		}
		this.continueButton.gameObject.SetActive(false);
		yield return this.AnimateExit();
		base.Close(0);
		yield break;
	}

	private IEnumerator WaitForButtonToBeClicked()
	{
		this.buttonHasBeenClicked = false;
		while (!this.buttonHasBeenClicked)
		{
			yield return null;
		}
		yield break;
	}

	private void ContinueButtonClicked(UIButton button)
	{
		this.buttonHasBeenClicked = true;
	}

	private IEnumerator AnimateExit()
	{
		yield return new Fiber.OnExit(delegate()
		{
			UICamera.EnableInput();
		});
		UICamera.DisableInput();
		if (base.Extension != null)
		{
			yield return base.Extension.AnimateExit();
		}
		yield break;
	}

	[SerializeField]
	private UIInstantiator continueButton;

	private bool buttonHasBeenClicked;

	private readonly Fiber fiber = new Fiber();

	public interface IExtension
	{
		IEnumerator AnimateExit();

		void SetTutorialText(string text);
	}
}
