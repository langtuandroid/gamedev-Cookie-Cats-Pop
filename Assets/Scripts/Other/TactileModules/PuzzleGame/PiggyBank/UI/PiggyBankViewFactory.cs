using System;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public class PiggyBankViewFactory : IPiggyBankViewFactory
	{
		public PiggyBankViewFactory()
		{
			this.uiViewManager = UIViewManager.Instance;
		}

		public UIViewManager.UIViewState ShowView<T>(bool mock = false) where T : UIView
		{
			return this.uiViewManager.ShowView<T>(new object[0]);
		}

		private readonly UIViewManager uiViewManager;
	}
}
