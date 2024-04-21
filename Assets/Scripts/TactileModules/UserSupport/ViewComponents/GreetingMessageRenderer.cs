using System;
using UnityEngine;

namespace TactileModules.UserSupport.ViewComponents
{
	public class GreetingMessageRenderer : MonoBehaviour
	{
		public void GreetUser(string name)
		{
			string text = this.label.text;
			text = text.Replace("[Username]", name);
			this.label.text = text;
		}

		[SerializeField]
		protected UILabel label;
	}
}
