using System;
using System.Diagnostics;

namespace TactileModules.GameCore.Rewards
{
	public class RewardView : UIView
	{
		private void OkClickedHandler(UIEvent e)
		{
			this.OkClicked();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OkClicked;



		public RewardGrid RewardGrid;
	}
}
