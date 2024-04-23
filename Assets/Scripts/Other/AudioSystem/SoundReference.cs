using System;
using UnityEngine;

namespace AudioSystem
{
	public class SoundReference : ScriptableObject
	{
		public ISoundDefinition SoundDefinition
		{
			get
			{
				return this.soundDefinition;
			}
		}

		[SerializeField]
		private SoundDefinition soundDefinition;
	}
}
