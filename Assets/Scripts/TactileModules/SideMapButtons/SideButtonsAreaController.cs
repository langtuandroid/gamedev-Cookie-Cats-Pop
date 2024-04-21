using System;
using System.Diagnostics;

namespace TactileModules.SideMapButtons
{
	public class SideButtonsAreaController
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ISideMapButtonController> SideMapButtonControllerCreated;



		public void CreateSideMapButtonControllers(SideMapButtonControllerProviderRegistry registry, SideButtonsArea area)
		{
			foreach (ISideMapButtonControllerProvider sideMapButtonControllerProvider in registry.Providers)
			{
				foreach (ISideMapButtonController sideMapButtonController in sideMapButtonControllerProvider.CreateButtonControllers())
				{
					SideMapButton sideMapButton = (SideMapButton)sideMapButtonController.GetSideMapButtonInstance();
					area.InitButton(sideMapButton.transform, (int)sideMapButton.Side, sideMapButton.GetElementSize(), new Func<object, bool>(sideMapButtonController.VisibilityChecker));
					this.SideMapButtonControllerCreated(sideMapButtonController);
				}
			}
		}

		public void Teardown()
		{
		}
	}
}
