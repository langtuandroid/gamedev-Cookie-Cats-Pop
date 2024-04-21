using System;
using System.Collections;

public interface IDialogViewProvider
{
	object ShowProgressView(string message);

	object ShowMessageBox(string title, string message, string buttonName0, string buttonName1);

	object GetViewResult(object viewHandle);

	void CloseView(object viewHandle);

	IEnumerator WaitForClosingView(object viewHandle);
}
