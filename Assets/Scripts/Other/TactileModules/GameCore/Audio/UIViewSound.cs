using System;
using UnityEngine;

namespace TactileModules.GameCore.Audio
{
	[RequireComponent(typeof(UIView))]
	public abstract class UIViewSound : MonoBehaviour
	{
		public ISoundDefinition SoundDefinition
		{
			get
			{
				return this.soundDefinition;
			}
		}

		[SerializeField]
		protected SoundDefinition soundDefinition;
	}
}
