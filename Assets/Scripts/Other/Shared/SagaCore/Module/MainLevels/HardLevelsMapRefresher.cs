using System;
using TactileModules.SagaCore;

namespace Shared.SagaCore.Module.MainLevels
{
	public class HardLevelsMapRefresher : IMapPlugin
	{
		public HardLevelsMapRefresher(HardLevelsManager hardLevelsManager)
		{
			hardLevelsManager.OnHardLevelsChanged += this.HandleHardLevelsChanged;
		}

		private void HandleHardLevelsChanged()
		{
			if (this.mapContentController != null)
			{
				this.mapContentController.Refresh();
			}
		}

		public void ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow)
		{
			if (mapId == "Main")
			{
				this.mapContentController = mapContent;
			}
		}

		public void ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent)
		{
			if (mapId == "Main")
			{
				this.mapContentController = null;
			}
		}

		private MapContentController mapContentController;
	}
}
