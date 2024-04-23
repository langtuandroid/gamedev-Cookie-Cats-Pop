using System;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea.Assets
{
	public class ButtonAreaSetup : ScriptableObject
	{
		public AnimationCurve FadeInCurve
		{
			get
			{
				return this.fadeInCurve;
			}
		}

		public AnimationCurve FadeOutCurve
		{
			get
			{
				return this.fadeOutCurve;
			}
		}

		[SerializeField]
		private AnimationCurve fadeInCurve;

		[SerializeField]
		private AnimationCurve fadeOutCurve;
	}
}
