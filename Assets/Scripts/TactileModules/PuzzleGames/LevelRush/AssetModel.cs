using System;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class AssetModel : IAssetModel
	{
		public LevelRushPresent LevelRushPresent
		{
			get
			{
				return Resources.Load<LevelRushPresent>("LevelRush/LevelRushPresent");
			}
		}

		public LevelRushStartView LevelRushStartView
		{
			get
			{
				return Resources.Load<LevelRushStartView>("LevelRush/LevelRushStartView");
			}
		}

		public LevelRushEndedView LevelRushEndedView
		{
			get
			{
				return Resources.Load<LevelRushEndedView>("LevelRush/LevelRushEndedView");
			}
		}

		public LevelRushPresentInfoView LevelRushPresentInfoView
		{
			get
			{
				return Resources.Load<LevelRushPresentInfoView>("LevelRush/LevelRushPresentInfoView");
			}
		}

		public LevelRushRewardView LevelRushRewardView
		{
			get
			{
				return Resources.Load<LevelRushRewardView>("LevelRush/LevelRushRewardView");
			}
		}
	}
}
