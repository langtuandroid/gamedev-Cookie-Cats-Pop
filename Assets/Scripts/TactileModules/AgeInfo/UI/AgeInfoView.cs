using System;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.AgeInfo.UI
{
	public class AgeInfoView : UserConsentView
	{
		protected override void ViewLoad(object[] parameters)
		{
			base.ViewLoad(parameters);
			AgeInfoIncrementButton ageInfoIncrementButton = this.buttonIncrement;
			ageInfoIncrementButton.OnValueIncrement = (Action<int>)Delegate.Combine(ageInfoIncrementButton.OnValueIncrement, new Action<int>(this.OnValueIncrement));
			AgeInfoIncrementButton ageInfoIncrementButton2 = this.buttonDecrement;
			ageInfoIncrementButton2.OnValueIncrement = (Action<int>)Delegate.Combine(ageInfoIncrementButton2.OnValueIncrement, new Action<int>(this.OnValueIncrement));
		}

		public override void OnConfirm(UIEvent e)
		{
			int ageThreshold = ManagerRepository.Get<AgeInfoManager>().GetAgeThreshold();
			if (this.selectedAge < ageThreshold)
			{
				this.provider.ShowInvalidAgeView(ageThreshold);
				return;
			}
			base.Close(this.selectedAge);
		}

		public void OnValueIncrement(int value)
		{
			this.selectedAge += value;
			this.selectedAge = Mathf.Clamp(this.selectedAge, 0, 100);
			this.labelAge.text = ((this.selectedAge < 100) ? this.selectedAge.ToString() : (100 + "+"));
		}

		public override string GetViewName()
		{
			return "AgeInfoView";
		}

		[SerializeField]
		private UILabel labelAge;

		[SerializeField]
		private AgeInfoIncrementButton buttonIncrement;

		[SerializeField]
		private AgeInfoIncrementButton buttonDecrement;

		private int selectedAge;

		private const int maxAge = 100;
	}
}
