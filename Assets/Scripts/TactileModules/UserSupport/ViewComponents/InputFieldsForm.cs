using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.UserSupport.View;
using UnityEngine;

namespace TactileModules.UserSupport.ViewComponents
{
	[RequireComponent(typeof(KeyboardHandler))]
	public class InputFieldsForm : MonoBehaviour
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Selected;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> Submit;



		private void Start()
		{
			this.SetEventHandlers();
			this.SetupKeyboard();
		}

		private void SetEventHandlers()
		{
			foreach (InputField inputField in this.fields)
			{
				InputField inputField2 = inputField;
				inputField2.Selected = (Action<InputField>)Delegate.Combine(inputField2.Selected, new Action<InputField>(this.FieldSelected));
			}
		}

		private void SetupKeyboard()
		{
			this.keyboardHandler = base.GetComponent<KeyboardHandler>();
			this.keyboardHandler.Multiline = false;
			this.keyboardHandler.Done += this.KeyboardHandlerOnDone;
			this.keyboardHandler.Input += this.KeyboardHandlerOnInput;
			this.keyboardHandler.Cancelled += this.KeyboardHandlerOnCancelled;
		}

		private void KeyboardHandlerOnCancelled(string input)
		{
			this.SetCurrentSelectedInputText(input);
		}

		private void KeyboardHandlerOnInput(string input)
		{
			this.SetCurrentSelectedInputText(input);
		}

		private void KeyboardHandlerOnDone(string input)
		{
			this.Submit(input);
		}

		private void SetCurrentSelectedInputText(string input)
		{
			if (this.currentSelected != null)
			{
				this.currentSelected.Text = input;
			}
		}

		private void FieldSelected(InputField inputField)
		{
			if (this.currentSelected != null)
			{
				this.currentSelected.Deselect();
			}
			this.currentSelected = inputField;
			this.currentSelected.Select();
			string input = string.Empty;
			if (this.currentSelected.Text != this.currentSelected.DefaultValue)
			{
				input = this.currentSelected.Text;
			}
			this.keyboardHandler.Show(input);
		}

		public void SetFieldValues(Dictionary<string, string> fieldsAndValues)
		{
			foreach (InputField inputField in this.fields)
			{
				if (fieldsAndValues.ContainsKey(inputField.Id))
				{
					inputField.Text = fieldsAndValues[inputField.Id];
				}
			}
		}

		public bool IsInputValid()
		{
			foreach (InputField inputField in this.fields)
			{
				InputValidator[] components = inputField.GetComponents<InputValidator>();
				foreach (InputValidator inputValidator in components)
				{
					if (!inputValidator.IsValid())
					{
						return false;
					}
				}
			}
			return true;
		}

		public string GetErrorMessage()
		{
			string text = string.Empty;
			foreach (InputField inputField in this.fields)
			{
				InputValidator[] components = inputField.GetComponents<InputValidator>();
				foreach (InputValidator inputValidator in components)
				{
					if (!inputValidator.IsValid())
					{
						text = text + inputValidator.GetErrorMessage() + Environment.NewLine;
					}
				}
			}
			return text;
		}

		public Dictionary<string, string> GetValues()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (InputField inputField in this.fields)
			{
				dictionary[inputField.Id] = inputField.Text;
			}
			return dictionary;
		}

		[SerializeField]
		private List<InputField> fields = new List<InputField>();

		private KeyboardHandler keyboardHandler;

		private InputField currentSelected;
	}
}
