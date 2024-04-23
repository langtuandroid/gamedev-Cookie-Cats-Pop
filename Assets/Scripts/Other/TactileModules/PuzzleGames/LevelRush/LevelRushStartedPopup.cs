using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushStartedPopup : MapPopupManager.IMapPopup
	{
		public LevelRushStartedPopup(ILevelRushActivation levelRushActivation, MainMapPlugin mainMapPlugin, IAssetModel assetModel)
		{
			this.levelRushActivation = levelRushActivation;
			this.mainMapPlugin = mainMapPlugin;
			this.assetModel = assetModel;
		}

		private IEnumerator ShowPopup()
		{
			if (!this.CanShowPopup())
			{
				yield break;
			}
			this.levelRushActivation.ActivateLevelRush();
			yield return this.ShowLevelRushStartedView(false);
			yield return this.mainMapPlugin.DropPresents();
			yield break;
		}

		private IEnumerator ShowLevelRushStartedView(bool isReminder)
		{
			LevelRushStartView view = UnityEngine.Object.Instantiate<LevelRushStartView>(this.assetModel.LevelRushStartView);
			UIViewManager.IUIViewStateGeneric<LevelRushStartView> vs = UIViewManager.Instance.ShowViewInstance<LevelRushStartView>(view, new object[]
			{
				isReminder
			});
			yield return vs.WaitForClose();
			yield break;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.CanShowPopup())
			{
				return;
			}
			popupFlow.AddPopup(this.ShowPopup());
		}

		private bool CanShowPopup()
		{
			return this.levelRushActivation.FeatureEnabled() && this.levelRushActivation.ShouldActivateLevelRush() && !this.levelRushActivation.HasActiveFeature();
		}

		private readonly ILevelRushActivation levelRushActivation;

		private readonly MainMapPlugin mainMapPlugin;

		private readonly IAssetModel assetModel;
	}
}
