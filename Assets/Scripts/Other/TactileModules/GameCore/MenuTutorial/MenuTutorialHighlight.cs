using System;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial
{
	public class MenuTutorialHighlight : MonoBehaviour
	{
		[Instantiator.SerializeProperty]
		public bool Round
		{
			get
			{
				return this.roundPivot != null && this.roundPivot.activeSelf;
			}
			set
			{
				if (this.roundPivot != null)
				{
					this.roundPivot.SetActive(value);
				}
				if (this.rectPivot != null)
				{
					this.rectPivot.SetActive(!value);
				}
			}
		}

		[SerializeField]
		private GameObject rectPivot;

		[SerializeField]
		private GameObject roundPivot;
	}
}
