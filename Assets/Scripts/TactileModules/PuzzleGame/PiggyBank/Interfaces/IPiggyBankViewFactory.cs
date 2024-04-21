using System;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankViewFactory
	{
		UIViewManager.UIViewState ShowView<T>(bool mock = false) where T : UIView;
	}
}
