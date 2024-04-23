using System;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	[Serializable]
	public class FullScreenSoundDefinition : IMusicTrackComponent
	{
		public FullScreenSoundDefinition(string type = "")
		{
			this.soundDefinition = new SoundDefinition();
			this.musicTrackData = new MusicTrackData(true, false, 0.3f, 0.3f);
			this.type = type;
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

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

		[SerializeField]
		private SoundDefinition soundDefinition;

		[SerializeField]
		private MusicTrackData musicTrackData;

		[SerializeField]
		private string type;
	}
}
