using System;
using System.Collections;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class MainMapPlugin : IMainMapPlugin, IMapPlugin
	{
		public MainMapPlugin(IMainProgression mainProgression, ILevelRushActivation levelRushActivation, ILevelRushProgression progression, IAssetModel assetModel)
		{
			this.mainProgression = mainProgression;
			this.levelRushActivation = levelRushActivation;
			this.progression = progression;
			this.assetModel = assetModel;
			levelRushActivation.FeatureDeactivated += this.HandleFeatureDeactivated;
		}

		public IEnumerator DropPresents()
		{
			if (this.presentsController != null)
			{
				yield return this.presentsController.DropPresents();
			}
			yield break;
		}

		public void RebuildPresents()
		{
			if (this.presentsController != null)
			{
				this.presentsController.RebuildPresents();
			}
		}

		void IMapPlugin.ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow)
		{
			if (mapId == "Main")
			{
				this.presentsController = new PresentsController(mapContent, this.mainProgression, this.levelRushActivation, this.progression, this, this.assetModel);
			}
		}

		void IMapPlugin.ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent)
		{
			if (mapId == "Main")
			{
				this.DestroyController();
			}
		}

		private void HandleFeatureDeactivated(ActivatedFeatureInstanceData data)
		{
			this.DestroyController();
		}

		private void DestroyController()
		{
			if (this.presentsController != null)
			{
				this.presentsController.DestroyObjects();
				this.presentsController = null;
			}
		}

		private readonly IMainProgression mainProgression;

		private readonly ILevelRushActivation levelRushActivation;

		private readonly ILevelRushProgression progression;

		private readonly IAssetModel assetModel;

		private PresentsController presentsController;
	}
}
