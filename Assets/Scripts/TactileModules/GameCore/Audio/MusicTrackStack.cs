using System;
using System.Collections.Generic;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.GameCore.Audio
{
	public class MusicTrackStack : IMusicTrackStack
	{
		public MusicTrackStack()
		{
			this.musicTrackStack = new FlowStack();
			this.musicTracks = new List<MusicTrack>();
		}

		public void Push(MusicTrack musicTrack)
		{
			if (!this.muted)
			{
				this.musicTrackStack.Push(musicTrack);
				this.musicTracks.Add(musicTrack);
				musicTrack.Initialize();
				musicTrack.OnTrackExit += this.OnTrackExit;
			}
		}

		public void Pop()
		{
			this.musicTrackStack.TerminateAll();
		}

		public void TurnMusicOn()
		{
			this.muted = false;
			foreach (MusicTrack musicTrack in this.musicTracks)
			{
				musicTrack.TurnMusicOn();
			}
		}

		public void TurnMusicOff()
		{
			this.muted = true;
			foreach (MusicTrack musicTrack in this.musicTracks)
			{
				musicTrack.TurnMusicOff();
			}
		}

		private void OnTrackExit(MusicTrack musicTrack)
		{
			if (this.musicTracks.Contains(musicTrack))
			{
				this.musicTracks.Remove(musicTrack);
			}
		}

		private readonly FlowStack musicTrackStack;

		private readonly List<MusicTrack> musicTracks;

		private bool muted;
	}
}
