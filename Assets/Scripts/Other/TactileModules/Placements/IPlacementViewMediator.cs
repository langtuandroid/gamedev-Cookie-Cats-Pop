using System;

namespace TactileModules.Placements
{
	public interface IPlacementViewMediator : IViewPresenter
	{
		int GetViewShownCount();

		void ResetViewShownCount();
	}
}
