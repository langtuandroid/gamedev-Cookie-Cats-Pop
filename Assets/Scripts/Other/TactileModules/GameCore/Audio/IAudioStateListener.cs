using System;

namespace TactileModules.GameCore.Audio
{
	public interface IAudioStateListener
	{
		bool SoundEffectsActive { get; set; }

		bool MusicActive { get; set; }
	}
}
