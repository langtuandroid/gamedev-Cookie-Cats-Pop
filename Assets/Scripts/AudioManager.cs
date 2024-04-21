using System;
using System.Collections;
using UnityEngine;

public class AudioManager
{
	protected AudioManager()
	{
		GameObject gameObject = new GameObject("[AudioManager]");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<AudioListener>();
		this.backgroundMusic = gameObject.AddComponent<AudioSource>();
		SoundDefinition.SetSoundDisablerFunction(() => !this.SoundEffectsActive);
		this.Load();
	}

	public static AudioManager Instance { get; private set; }

	public bool SoundEffectsActive
	{
		get
		{
			return this.soundEffectsActive;
		}
		set
		{
			this.soundEffectsActive = value;
			this.RefreshMusicState();
			this.Save();
		}
	}

	public bool MusicActive
	{
		get
		{
			return this.musicActive;
		}
		set
		{
			this.musicActive = value;
			this.RefreshMusicState();
			this.Save();
		}
	}

	public static AudioManager CreateInstance()
	{
		AudioManager.Instance = new AudioManager();
		return AudioManager.Instance;
	}

	private void Load()
	{
		this.soundEffectsActive = TactilePlayerPrefs.GetBool("AudioManager_sfx", true);
		this.musicActive = TactilePlayerPrefs.GetBool("AudioManager_music", true);
	}

	private void Save()
	{
		TactilePlayerPrefs.SetBool("AudioManager_sfx", this.soundEffectsActive);
		TactilePlayerPrefs.SetBool("AudioManager_music", this.musicActive);
	}

	private void RefreshMusicState()
	{
		if (false || !this.musicActive)
		{
			this.backgroundMusic.Stop();
		}
		else if (!this.backgroundMusic.isPlaying)
		{
			this.backgroundMusic.Play();
		}
	}

	public IEnumerator FadeMusicDown()
	{
		float orgVol = this.backgroundMusic.volume;
		yield return FiberAnimation.Animate(0.3f, delegate(float v)
		{
			this.backgroundMusic.volume = orgVol * (1f - v);
		});
		yield break;
	}

	public void SetMusic(SoundDefinition music, bool loop = true)
	{
		if (music != null)
		{
			SoundDefinition.SoundData randomSoundData = music.GetRandomSoundData();
			this.backgroundMusic.clip = randomSoundData.Clip;
			this.backgroundMusic.volume = randomSoundData.Volume;
			this.backgroundMusic.loop = loop;
		}
		else
		{
			this.backgroundMusic.clip = null;
			this.backgroundMusic.volume = 0f;
		}
		this.RefreshMusicState();
	}

	public void PauseMusic(bool pause)
	{
		if (!this.musicActive)
		{
			return;
		}
		if (pause)
		{
			this.backgroundMusic.Pause();
		}
		else
		{
			this.backgroundMusic.Play();
		}
	}

	private const string KEY_SFX = "AudioManager_sfx";

	private const string KEY_MUSIC = "AudioManager_music";

	private bool soundEffectsActive;

	private bool musicActive;

	private AudioSource backgroundMusic;
}
