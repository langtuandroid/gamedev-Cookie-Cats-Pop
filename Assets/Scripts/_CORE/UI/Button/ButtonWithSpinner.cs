using System;
using UnityEngine;

public class ButtonWithSpinner : AnimatedButton
{
	[Instantiator.SerializeProperty(false)]
	public bool Spinning
	{
		get
		{
			return this.isSpinning;
		}
		set
		{
			this.isSpinning = value;
			this.ReflectState();
		}
	}

	[Instantiator.SerializeProperty]
	public string PrimaryTitle
	{
		get
		{
			return this.primaryButtonLabel.text;
		}
		set
		{
			this.primaryButtonLabel.text = value;
		}
	}

	[Instantiator.SerializeProperty]
	public string LoadingTitle
	{
		get
		{
			return this.loadingButtonLabel.text;
		}
		set
		{
			this.loadingButtonLabel.text = value;
		}
	}

	protected override void ReflectState()
	{
		base.ReflectState();
		this.primaryButton.SetActive(!base.Disabled);
		this.loadingButton.SetActive(base.Disabled);
		this.spinner.SetActive(this.Spinning);
	}

	[SerializeField]
	private UILabel primaryButtonLabel;

	[SerializeField]
	private UILabel loadingButtonLabel;

	[SerializeField]
	private GameObject spinner;

	[SerializeField]
	private GameObject primaryButton;

	[SerializeField]
	private GameObject loadingButton;

	private bool isSpinning;
}
