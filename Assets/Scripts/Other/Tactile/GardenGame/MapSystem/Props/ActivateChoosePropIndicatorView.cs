using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class ActivateChoosePropIndicatorView : UIView
	{
		protected override void ViewLoad(object[] parameters)
		{
			this.IndicatorVisible = false;
		}

		public void ShowIndicator(Vector3 worldPosition)
		{
			this.IndicatorVisible = true;
			worldPosition.z = 0f;
			this.indicator.transform.position = worldPosition;
		}

		public float IndicatorProgress
		{
			get
			{
				return this.indicator.fillAmount;
			}
			set
			{
				this.indicator.fillAmount = value;
			}
		}

		public bool IndicatorVisible
		{
			get
			{
				return this.indicator.gameObject.activeSelf;
			}
			set
			{
				this.indicator.gameObject.SetActive(value);
			}
		}

		[SerializeField]
		private UIFilledSprite indicator;
	}
}
