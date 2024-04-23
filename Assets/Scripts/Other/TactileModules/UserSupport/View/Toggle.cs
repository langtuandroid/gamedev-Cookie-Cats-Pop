using System;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class Toggle : MonoBehaviour
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Toggle> Toggled;



		public bool IsOn
		{
			get
			{
				return this.on;
			}
			set
			{
				this.on = value;
				this.Render();
			}
		}

		private void Start()
		{
			this.button = base.GetComponent<UIButton>();
			if (this.button != null)
			{
				this.button.Clicked += this.ButtonOnClicked;
			}
			this.Render();
		}

		private void ButtonOnClicked(UIButton obj)
		{
			this.SetMode();
			this.Toggled(this);
		}

		private void SetMode()
		{
			this.on = !this.on;
			this.Render();
		}

		private void Render()
		{
			this.onImage.SetActive(this.on);
			this.offImage.SetActive(!this.on);
		}

		[SerializeField]
		private GameObject onImage;

		[SerializeField]
		private GameObject offImage;

		private UIButton button;

		private bool on;
	}
}
