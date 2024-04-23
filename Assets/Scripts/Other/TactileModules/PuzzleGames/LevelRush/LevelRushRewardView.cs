using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushRewardView : ExtensibleView<LevelRushRewardView.IExtension>
	{
		public LevelRushConfig.Reward Reward { get; private set; }

		public int RewardIndex { get; private set; }

		public void Initialize(LevelRushConfig.Reward reward, int rewardIndex)
		{
			this.Reward = reward;
			this.RewardIndex = rewardIndex;
			if (base.Extension != null)
			{
				base.Extension.Initialize(this);
			}
		}

		[UsedImplicitly]
		private void Claim(UIEvent e)
		{
			this.claimFiber.Start(this.AnimateClaim());
		}

		private IEnumerator AnimateClaim()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateClaim(this.Reward);
			}
			base.Close(0);
			yield break;
		}

		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		protected override void ViewDidDisappear()
		{
			this.claimFiber.Terminate();
		}

		private readonly Fiber claimFiber = new Fiber();

		public interface IExtension
		{
			void Initialize(LevelRushRewardView view);

			IEnumerator AnimateClaim(LevelRushConfig.Reward reward);
		}
	}
}
