using System;

namespace TactileModules.GameCore.Audio
{
	public interface IMusicTrackStack
	{
		void Push(MusicTrack musicTrack);

		void Pop();

		void TurnMusicOn();

		void TurnMusicOff();
	}
}
