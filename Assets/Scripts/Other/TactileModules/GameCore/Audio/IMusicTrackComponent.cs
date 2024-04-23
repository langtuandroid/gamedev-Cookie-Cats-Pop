using System;

namespace TactileModules.GameCore.Audio
{
	public interface IMusicTrackComponent
	{
		SoundDefinition SoundDefinition { get; }

		MusicTrackData Data { get; }
	}
}
