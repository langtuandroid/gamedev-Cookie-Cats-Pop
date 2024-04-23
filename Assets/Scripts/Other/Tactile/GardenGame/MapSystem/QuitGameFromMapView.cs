using System;
using System.Diagnostics;

namespace Tactile.GardenGame.MapSystem
{
	public class QuitGameFromMapView : UIView
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnQuitGame;

		public bool Dismissed
		{
			get
			{
				return this.dismissed;
			}
		}

		public void QuitGame(UIEvent e)
		{
			if (this.OnQuitGame != null)
			{
				this.OnQuitGame();
			}
		}

		public void Clicked(UIEvent e)
		{
			this.dismissed = true;
		}

		private bool dismissed;
	}
}
