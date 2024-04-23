using System;

namespace TactileModules.GameCore.Audio
{
	public class AudioSystem
	{
		public AudioSystem(IMusicTrackStack musicTrackStack, IAudioStateListener audioStateListener, MusicListener musicListener, SoundEffectListener soundEffectListener, AudioDatabaseInjector audioDatabaseInjector)
		{
			this.MusicTrackStack = musicTrackStack;
			this.AudioStateListener = audioStateListener;
			this.MusicListener = musicListener;
			this.SoundEffectListener = soundEffectListener;
			this.AudioDatabaseInjector = audioDatabaseInjector;
		}

		public IMusicTrackStack MusicTrackStack { get; private set; }

		public IAudioStateListener AudioStateListener { get; private set; }

		public MusicListener MusicListener { get; private set; }

		public SoundEffectListener SoundEffectListener { get; private set; }

		public AudioDatabaseInjector AudioDatabaseInjector { get; private set; }
	}
}
