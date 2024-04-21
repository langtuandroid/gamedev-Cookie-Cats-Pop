using System;
using TactileModules.Ads;

public class MusicControlProvider : IMusicControlProvider
{
	public MusicControlProvider(AudioManager audioManager)
	{
		this.audioManager = audioManager;
	}

	public void PauseMusic()
	{
		this.audioManager.PauseMusic(true);
	}

	public void ResumeMusic()
	{
		this.audioManager.PauseMusic(false);
	}

	private readonly AudioManager audioManager;
}
