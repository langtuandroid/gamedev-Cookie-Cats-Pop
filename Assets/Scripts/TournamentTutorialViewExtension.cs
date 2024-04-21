using System;
using System.Collections;
using UnityEngine;

public class TournamentTutorialViewExtension : MonoBehaviour, TournamentTutorialView.IExtension
{
	public IEnumerator AnimateExit()
	{
		yield break;
	}

	public void SetTutorialText(string text)
	{
		this.bubble.GetInstance<TutorialSpeechBubble>().Message = text;
	}

	[InstantiatorRequires(typeof(TutorialSpeechBubble))]
	[SerializeField]
	private UIInstantiator bubble;
}
