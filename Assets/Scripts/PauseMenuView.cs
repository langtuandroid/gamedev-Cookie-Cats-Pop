using System;
using UnityEngine;

public class PauseMenuView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.session = (LevelSession)parameters[0];
		LevelProxy level = this.session.Level;
		this.levelDisplayName.text = "Level " + level.DisplayName;
		this.levelInternalName.gameObject.SetActive(false);
	}

	protected override void ViewWillAppear()
	{
		this.musicToggleButton.GetInstance<ButtonToggle>().IsOn = AudioManager.Instance.MusicActive;
		this.soundToggleButton.GetInstance<ButtonToggle>().IsOn = AudioManager.Instance.SoundEffectsActive;
	}

	public void ButtonCloseClicked(UIEvent e)
	{
		base.Close(PauseMenuView.Result.Dismiss);
	}

	private void MusicClicked(UIEvent e)
	{
		ButtonToggle component = e.sender.GetComponent<ButtonToggle>();
		AudioManager.Instance.MusicActive = component.IsOn;
	}

	private void SoundClicked(UIEvent e)
	{
		ButtonToggle component = e.sender.GetComponent<ButtonToggle>();
		AudioManager.Instance.SoundEffectsActive = component.IsOn;
	}

	private void GoToMapClicked(UIEvent e)
	{
		base.Close(PauseMenuView.Result.GoToMap);
	}

	private void ContinueButton(UIEvent e)
	{
		base.Close(PauseMenuView.Result.Dismiss);
	}

	private void CheatClickedDevMenu(UIEvent e)
	{
	}

	private void CheatClickedComplete(UIEvent e)
	{
	}

	private void CheatClickedFail(UIEvent e)
	{
	}

	[SerializeField]
	private UILabel levelDisplayName;

	[SerializeField]
	private UILabel levelInternalName;

	[SerializeField]
	private UILabel objectiveText;

	[SerializeField]
	private UIInstantiator musicToggleButton;

	[SerializeField]
	private UIInstantiator soundToggleButton;

	private LevelSession session;

	public enum Result
	{
		Dismiss,
		GoToMap
	}
}
