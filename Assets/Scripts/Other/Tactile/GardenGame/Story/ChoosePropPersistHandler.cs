using System;
using Tactile.GardenGame.MapSystem;

namespace Tactile.GardenGame.Story
{
	public class ChoosePropPersistHandler
	{
		public ChoosePropPersistHandler(PropsManager propsManager, StoryManager storyManager, MapInformation mapInformation)
		{
			this.propsManager = propsManager;
			this.storyManager = storyManager;
			this.mapInformation = mapInformation;
			this.storyManager.TaskCompleted += this.StoryManagerOnTaskCompleted;
		}

		public void Destroy()
		{
			this.storyManager.TaskCompleted -= this.StoryManagerOnTaskCompleted;
		}

		private void StoryManagerOnTaskCompleted(MapTask task)
		{
			bool flag = false;
			foreach (MapActionChoose mapActionChoose in task.IterateActionTypes<MapActionChoose>())
			{
				MapProp mapComponent = this.mapInformation.GetMapComponent<MapProp>(mapActionChoose.MapPropId);
				if (mapComponent != null)
				{
					MapProp.Variation firstPickableVariation = mapComponent.FirstPickableVariation;
					if (firstPickableVariation != null)
					{
						flag |= this.propsManager.SetPropSkinNoSave(mapActionChoose.MapPropId, firstPickableVariation.ID);
					}
				}
			}
			if (flag)
			{
				this.propsManager.Save();
			}
		}

		private readonly PropsManager propsManager;

		private readonly StoryManager storyManager;

		private readonly MapInformation mapInformation;
	}
}
