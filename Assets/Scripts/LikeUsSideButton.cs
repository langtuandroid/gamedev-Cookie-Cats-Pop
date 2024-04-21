using System;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class LikeUsSideButton : SideMapButton
{
	public override SideMapButton.AreaSide Side
	{
		get
		{
			return SideMapButton.AreaSide.Right;
		}
	}

	public override bool VisibilityChecker(object data)
	{
		LikeUsManager.PersistableState persistableState = UserSettingsManager.Get<LikeUsManager.PersistableState>();
		SocialConfig socialConfig = ConfigurationManager.Get<SocialConfig>();
		return socialConfig.RequiredLevel <= MainProgressionManager.Instance.GetFarthestUnlockedLevelHumanNumber() && (!persistableState.FacebookFreeGiftConsumed || !persistableState.TwitterFreeGiftConsumed || !persistableState.InstagramFreeGiftConsumed || !persistableState.YouTubeFreeGiftConsumed);
	}

	public override Vector2 Size
	{
		get
		{
			return Vector2.zero;
		}
	}

	public override object Data
	{
		get
		{
			return null;
		}
	}

	private void LikeUsClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<LikeUsView>(new object[0]);
	}
}
