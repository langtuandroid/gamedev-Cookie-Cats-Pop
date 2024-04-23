using System;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public class ButtonArea : UIInstantiator
	{
		public override void CreateInstance()
		{
			if (Application.isPlaying)
			{
				return;
			}
			base.CreateInstance();
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Destroyed;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.Destroyed != null)
			{
				this.Destroyed();
			}
		}

		public bool IsInstanceArea
		{
			get
			{
				return this.prefab != null;
			}
		}

		public bool PrefabMatch(ButtonAreaButton button)
		{
			return button.gameObject == this.prefab;
		}

		public void FitButtonSizeToArea(ButtonAreaButton button)
		{
			UIElement component = button.GetComponent<UIElement>();
			UIElement component2 = base.GetComponent<UIElement>();
			if (component != null && component2 != null)
			{
				component.SetSizeFlags(UIAutoSizing.AllCorners);
				if (component2.SizeIsValid)
				{
					component.SetSizeAndDoLayout(component2.Size);
				}
				else
				{
					component2.Size = component.Size;
				}
			}
		}

		public ButtonAreaCategory Category;

		public bool fitButtonSizeToArea;

		public bool stayAtPosition;
	}
}
