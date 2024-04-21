using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class ClaimRewardFlow : IFiberRunnable
	{
		public ClaimRewardFlow(ILevelRushActivation levelRushActivation, ILevelRushProgression progression, IMainMapPlugin mainMapPlugin, IAssetModel assetModel)
		{
			this.levelRushActivation = levelRushActivation;
			this.progression = progression;
			this.mainMapPlugin = mainMapPlugin;
			this.assetModel = assetModel;
		}

		public IEnumerator Run()
		{
			LevelRushConfig.Reward reward = this.progression.ClaimNextUnclaimedReward();
			if (reward == null)
			{
				yield break;
			}
			this.progression.AddRewardToInventory(reward);
			this.mainMapPlugin.RebuildPresents();
			UIViewManager.IUIViewStateGeneric<LevelRushRewardView> vs = UIViewManager.Instance.ShowViewFromPrefab<LevelRushRewardView>(this.assetModel.LevelRushRewardView, new object[0]);
			int rewardIndex = this.progression.GetRewardIndex(reward);
			vs.View.Initialize(reward, rewardIndex);
			yield return vs.WaitForClose();
			if (this.progression.IsLastReward(reward))
			{
				this.levelRushActivation.DeactivateLevelRush();
				yield break;
			}
			if (this.progression.HasUnclaimedRewards)
			{
				yield return this.Run();
			}
			yield break;
		}

		public void OnExit()
		{
		}

		private readonly ILevelRushActivation levelRushActivation;

		private readonly ILevelRushProgression progression;

		private readonly IMainMapPlugin mainMapPlugin;

		private readonly IAssetModel assetModel;
	}
}
