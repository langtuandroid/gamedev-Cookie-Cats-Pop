using System;
using JetBrains.Annotations;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushPresentInfoView : ExtensibleView<LevelRushPresentInfoView.IExtension>
	{
		public LevelRushConfig.Reward Reward { get; private set; }

		public int RewardIndex { get; private set; }

		public void Initialize(LevelRushConfig.Reward reward, int rewardIndex)
		{
			this.RewardIndex = rewardIndex;
			this.Reward = reward;
			LevelRushPresentInfoView.IExtension component = base.GetComponent<LevelRushPresentInfoView.IExtension>();
			if (component != null)
			{
				component.Initialize(this);
			}
		}

		[UsedImplicitly]
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		public interface IExtension
		{
			void Initialize(LevelRushPresentInfoView view);
		}
	}
}
