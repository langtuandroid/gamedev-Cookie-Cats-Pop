using System;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.UserSupport.ViewComponents
{
	[RequireComponent(typeof(UILabel))]
	public class InputField : MonoBehaviour
	{
		private void Awake()
		{
			this.label = base.GetComponent<UILabel>();
			this.defaultValue = this.label.text;
		}

		public void Select()
		{
			if (this.label.text.Equals(this.defaultValue))
			{
				this.label.text = string.Empty;
			}
		}

		public void Deselect()
		{
			if (string.IsNullOrEmpty(this.label.text))
			{
				this.label.text = this.defaultValue;
			}
		}

		[UsedImplicitly]
		private void OnSelected(UIEvent e)
		{
			this.Selected(this);
		}

		public string Text
		{
			get
			{
				return this.label.text;
			}
			set
			{
				this.label.text = value;
			}
		}

		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		public UILabel Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		public Action<InputField> Selected = delegate(InputField inputField)
		{
		};

		private UILabel label;

		private string defaultValue;

		[SerializeField]
		public string Id = string.Empty;
	}
}
