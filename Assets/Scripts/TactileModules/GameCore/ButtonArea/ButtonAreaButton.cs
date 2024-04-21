using System;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public class ButtonAreaButton : MonoBehaviour
	{
		protected void ButtonClicked(UIEvent e)
		{
			if (this.Clicked != null)
			{
				this.Clicked();
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Clicked;
	}
}
