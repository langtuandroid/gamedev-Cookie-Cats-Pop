using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.UserSupport.ViewComponents
{
	public class SubmitButton : MonoBehaviour
	{
		[UsedImplicitly]
		[Instantiator.SerializeLocalizableProperty]
		public string Title
		{
			get
			{
				return this.title.text;
			}
			set
			{
				this.title.text = value;
			}
		}

		[SerializeField]
		private UILabel title;
	}
}
