using System;
using System.Diagnostics;

namespace TactileModules.SagaCore
{
	public class MainMapButtonsView : UIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action DidEscape;



		private void Update()
		{
			if (UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
			{
				this.DidEscape();
			}
		}
	}
}
