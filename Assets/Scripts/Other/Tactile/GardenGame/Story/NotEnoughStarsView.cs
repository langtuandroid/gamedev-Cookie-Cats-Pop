using System;

namespace Tactile.GardenGame.Story
{
	public class NotEnoughStarsView : UIView
	{
		private void CloseClicked(UIEvent e)
		{
			base.Close(0);
		}

		private void OkClicked(UIEvent e)
		{
			base.Close(1);
		}
	}
}
