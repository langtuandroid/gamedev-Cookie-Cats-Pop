using System;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class KeyboardHandler : MonoBehaviour
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> Done;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> Input;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> Cancelled;



		public bool Multiline
		{
			get
			{
				return this.multiline;
			}
			set
			{
				this.multiline = value;
			}
		}

		public string GetInput()
		{
			return this.inputText;
		}

		public void Show(string input = "")
		{
			this.isDone = false;
			this.inputText = input;
			this.keyboard = TouchScreenKeyboard.Open(this.inputText, TouchScreenKeyboardType.Default, true, this.Multiline);
		}

		public void Hide()
		{
			this.keyboard.active = false;
		}

		private void Update()
		{
			this.HandleSoftwareKeyboard();
		}

		private void OnGUI()
		{
			if (this.keyboard != null)
			{
				this.inputText = this.keyboard.text;
			}
		}

		private void HandleSoftwareKeyboard()
		{
			if (this.keyboard == null)
			{
				return;
			}
			if (this.isDone)
			{
				return;
			}
			this.Input(this.inputText);
			if (this.keyboard.wasCanceled)
			{
				this.inputText = this.keyboard.text;
				this.Cancelled(this.inputText);
				return;
			}
			if (this.keyboard.done && !this.keyboard.wasCanceled)
			{
				this.inputText = this.keyboard.text;
				this.Done(this.keyboard.text);
				this.isDone = true;
			}
		}

		private void HandleHardwareKeyboard()
		{
			foreach (char c in UnityEngine.Input.inputString)
			{
				if (c == '\b')
				{
					if (this.inputText.Length != 0)
					{
						this.inputText = this.inputText.Substring(0, this.inputText.Length - 1);
					}
				}
				else if (c == '\n' || c == '\r')
				{
					this.Done(this.inputText);
				}
				else
				{
					this.inputText += c;
				}
				this.Input(this.inputText);
			}
		}

		private string inputText;

		private TouchScreenKeyboard keyboard;

		private bool isDone;

		private bool multiline = true;
	}
}
