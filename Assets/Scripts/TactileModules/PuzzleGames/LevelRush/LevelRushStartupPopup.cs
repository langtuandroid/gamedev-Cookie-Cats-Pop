using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushStartupPopup : MapPopupManager.IMapPopup
	{
		public LevelRushStartupPopup(ILevelRushActivation levelRushActivation, IAssetModel assetModel)
		{
			this.levelRushActivation = levelRushActivation;
			this.assetModel = assetModel;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.levelRushActivation.FeatureEnabled())
			{
				return;
			}
			if (this.levelRushActivation.HasActiveFeature() && !this.levelRushActivation.ShouldDeactivateLevelRush())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		private IEnumerator ShowPopup()
		{
			LevelRushStartView view = UnityEngine.Object.Instantiate<LevelRushStartView>(this.assetModel.LevelRushStartView);
			UIViewManager.IUIViewStateGeneric<LevelRushStartView> vs = UIViewManager.Instance.ShowViewInstance<LevelRushStartView>(view, new object[]
			{
				true
			});
			yield return vs.WaitForClose();
			yield break;
		}

		private readonly ILevelRushActivation levelRushActivation;

		private readonly IAssetModel assetModel;
	}
}
