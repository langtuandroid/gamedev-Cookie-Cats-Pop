using System;
using UnityEngine;

public class TutorialSpeechBubble : MonoBehaviour
{
	[Instantiator.SerializeLocalizableProperty]
	public string Message
	{
		get
		{
			return (!(this.message != null)) ? string.Empty : this.message.text;
		}
		set
		{
			base.gameObject.SetActive(!string.IsNullOrEmpty(value));
			if (this.message == null)
			{
				return;
			}
			this.message.typingProgress = 0f;
			this.message.text = value;
		}
	}

	public UILabel message;
}
