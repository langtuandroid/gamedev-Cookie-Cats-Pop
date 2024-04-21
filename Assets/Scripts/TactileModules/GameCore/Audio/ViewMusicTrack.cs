using System;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	[RequireComponent(typeof(UIView))]
	public class ViewMusicTrack : MonoBehaviour, IMusicTrackComponent
	{
		public SoundDefinition SoundDefinition
		{
			get
			{
				return this.soundDefinition;
			}
		}

		public MusicTrackData Data
		{
			get
			{
				return this.musicTrackData;
			}
		}

		public bool MuteScreenTrackOnExit
		{
			get
			{
				return this.muteScreenTrackOnExit;
			}
		}

		[SerializeField]
		private SoundDefinition soundDefinition;

		[SerializeField]
		private MusicTrackData musicTrackData;

		[SerializeField]
		private bool muteScreenTrackOnExit;
	}
}
