using System;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	[RequireComponent(typeof(UIElement))]
	public class UIProgressBar : MonoBehaviour
	{
		private float FillModifier
		{
			get
			{
				return 1f - this.startFill;
			}
		}

		public float Progress
		{
			get
			{
				return this.progress;
			}
			set
			{
				this.progress = Mathf.Min(value, 1f);
				float fillAmount = 0f;
				if (this.progress > 0f)
				{
					fillAmount = this.startFill + this.progress * this.FillModifier;
				}
				this.fillModifier.FillAmount = fillAmount;
			}
		}

		[SerializeField]
		private UIFillModifier fillModifier;

		[SerializeField]
		private float startFill = 0.1f;

		private float progress;
	}
}
