using System;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	public class AudioStateListener : IAudioStateListener
	{
		public AudioStateListener(IMusicTrackStack musicTrackStack, MusicListener musicListener)
		{
			this.musicTrackStack = musicTrackStack;
			this.musicListener = musicListener;
			GameObject gameObject = new GameObject("[AudioListener]");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<AudioListener>();
			SoundDefinition.SetSoundDisablerFunction(() => !this.SoundEffectsActive);
			this.Load();
		}

		public bool SoundEffectsActive
		{
			get
			{
				return this.soundEffectsActive;
			}
			set
			{
				this.soundEffectsActive = value;
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
				if (this.musicActive != value)
				{
					if (value)
					{
						this.musicTrackStack.TurnMusicOn();
						this.musicListener.TurnMusicOn();
					}
					else
					{
						this.musicTrackStack.TurnMusicOff();
						this.musicListener.TurnMusicOff();
					}
				}
				this.musicActive = value;
				this.Save();
			}
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

		private const string KEY_SFX = "AudioManager_sfx";

		private const string KEY_MUSIC = "AudioManager_music";

		private readonly IMusicTrackStack musicTrackStack;

		private readonly MusicListener musicListener;

		private bool soundEffectsActive;

		private bool musicActive;
	}
}
