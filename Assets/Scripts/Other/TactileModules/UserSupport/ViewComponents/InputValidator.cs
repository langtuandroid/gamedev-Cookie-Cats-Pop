using System;
using UnityEngine;

namespace TactileModules.UserSupport.ViewComponents
{
	[RequireComponent(typeof(InputField))]
	public class InputValidator : MonoBehaviour, IInputValidator
	{
		protected InputField GetInputField()
		{
			return base.GetComponent<InputField>();
		}

		protected string Input
		{
			get
			{
				return this.GetInputField().Text.Trim();
			}
		}

		public virtual bool IsValid()
		{
			return true;
		}

		public virtual string GetErrorMessage()
		{
			return string.Empty;
		}
	}
}
