using System;
using System.Collections;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushRewardPopup : MapPopupManager.IMapPopup
	{
		public LevelRushRewardPopup(IMainMapPlugin mainMapPlugin, ILevelRushActivation levelRushActivation, ILevelRushProgression progression, IAssetModel assetModel)
		{
			this.mainMapPlugin = mainMapPlugin;
			this.levelRushActivation = levelRushActivation;
			this.progression = progression;
			this.assetModel = assetModel;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.levelRushActivation.FeatureEnabled())
			{
				return;
			}
			if (this.progression.HasUnclaimedRewards)
			{
				popupFlow.AddPopup(this.TryClaimReward());
			}
		}

		private IEnumerator TryClaimReward()
		{
			yield return new ClaimRewardFlow(this.levelRushActivation, this.progression, this.mainMapPlugin, this.assetModel);
			yield break;
		}

		private readonly IMainMapPlugin mainMapPlugin;

		private readonly ILevelRushActivation levelRushActivation;

		private readonly ILevelRushProgression progression;

		private readonly IAssetModel assetModel;
	}
}
