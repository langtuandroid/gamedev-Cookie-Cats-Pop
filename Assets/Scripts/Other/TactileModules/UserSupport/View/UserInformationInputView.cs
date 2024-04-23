using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.UserSupport.ViewComponents;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class UserInformationInputView : UIView, IUserInformationInputView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Closed;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Input;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> Submit;



		protected override void ViewLoad(object[] parameters)
		{
			this.ShowErrorMessage(string.Empty);
			this.form.Selected += this.FormOnSelected;
			this.form.Submit += this.FormOnSubmit;
		}

		private void FormOnSubmit(string input)
		{
			if (!this.form.IsInputValid())
			{
				this.ShowErrorMessage(this.GetErrorMessage());
				return;
			}
			Dictionary<string, string> values = this.form.GetValues();
			this.Submit(values["Name"]);
		}

		private void FormOnSelected()
		{
			this.ShowErrorMessage(string.Empty);
		}

		void IUserInformationInputView.Close(int result)
		{
			base.Close(result);
		}

		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			this.Closed();
		}

		private void ShowErrorMessage(string errorMessage)
		{
			this.errorMessageLabel.text = errorMessage;
		}

		private string GetErrorMessage()
		{
			return this.form.GetErrorMessage();
		}

		[SerializeField]
		private UILabel errorMessageLabel;

		[SerializeField]
		private InputFieldsForm form;
	}
}
