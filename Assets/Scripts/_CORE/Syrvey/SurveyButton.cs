using System;
using UnityEngine;

[RequireComponent(typeof(SpriteStateHandler))]
public class SurveyButton : MonoBehaviour
{
	public string LabelText
	{
		get
		{
			return this.optionLabel.text;
		}
		set
		{
			this.optionLabel.text = value;
		}
	}

	private void Awake()
	{
		this.spriteStateHandler = base.GetComponent<SpriteStateHandler>();
		this.fontStyleStateHandler = base.GetComponentInChildren<FontStyleStateHandler>();
	}

	public void Initialize(string buttonText, GameObject receiver, string methodName)
	{
		this.LabelText = buttonText;
		this.optionButton.receiver = receiver;
		this.optionButton.methodName = methodName;
		this.optionButton.payload = this;
	}

	public void SetActiveVisiualsState(bool isActive)
	{
		this.spriteStateHandler.SetState(isActive);
		this.fontStyleStateHandler.SetState(isActive);
	}

	public void SetActiveButtonState(bool isActive)
	{
		this.optionButton.enabled = isActive;
	}

	private void OnValidate()
	{
		this.fontStyleStateHandler = base.GetComponentInChildren<FontStyleStateHandler>();
		if (this.fontStyleStateHandler == null)
		{
		}
	}

	[SerializeField]
	private UILabel optionLabel;

	[SerializeField]
	private UIButton optionButton;

	private SpriteStateHandler spriteStateHandler;

	private FontStyleStateHandler fontStyleStateHandler;
}
