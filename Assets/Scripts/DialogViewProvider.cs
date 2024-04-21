using System;
using System.Collections;

public class DialogViewProvider : IDialogViewProvider
{
	object IDialogViewProvider.ShowProgressView(string message)
	{
		return UIViewManager.Instance.ShowView<ProgressView>(new object[]
		{
			message
		});
	}

	object IDialogViewProvider.ShowMessageBox(string title, string message, string buttonName0, string buttonName1)
	{
		return UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
		{
			title,
			message,
			buttonName0,
			buttonName1
		});
	}

	object IDialogViewProvider.GetViewResult(object viewState)
	{
		return ((UIViewManager.UIViewState)viewState).ClosingResult;
	}

	IEnumerator IDialogViewProvider.WaitForClosingView(object viewHandle)
	{
		UIViewManager.UIViewState vs = viewHandle as UIViewManager.UIViewState;
		yield return vs.WaitForClose();
		yield break;
	}

	void IDialogViewProvider.CloseView(object viewHandle)
	{
		UIViewManager.UIViewState uiviewState = viewHandle as UIViewManager.UIViewState;
		uiviewState.View.Close(0);
	}
}
