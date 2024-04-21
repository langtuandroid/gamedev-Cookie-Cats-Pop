using System;
using System.Collections;
using TactileModules.FacebookExtras;
using TactileModules.Placements;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;

public class PlayWithFriendsPlacement : IPlacementRunnableNoBreak, IPlacementRunnable
{
	public PlayWithFriendsPlacement(IConfigGetter<FacebookNotification> configGetter, FacebookLoginManager facebookLoginManager, IMainProgression mainProgression)
	{
		this.configGetter = configGetter;
		this.facebookLoginManager = facebookLoginManager;
		this.mainProgression = mainProgression;
	}

	public string ID
	{
		get
		{
			return "PlayWithFriends";
		}
	}

	public IEnumerator Run(IPlacementViewMediator placementViewMediator)
	{
		if (this.ShouldShowPopup())
		{
			yield return this.ShowPopupAndWaitForClose();
		}
		yield break;
	}

	private IEnumerator ShowPopupAndWaitForClose()
	{
		UIViewManager.UIViewStateGeneric<PlayWithFriendsView> vs = UIViewManager.Instance.ShowView<PlayWithFriendsView>(new object[0]);
		yield return vs.WaitForClose();
		yield break;
	}

	private bool ShouldShowPopup()
	{
		int levelRequiredForPlayWithFriendsView = this.configGetter.Get().LevelRequiredForPlayWithFriendsView;
		return !this.facebookLoginManager.IsLoggedInAndUserRegistered && levelRequiredForPlayWithFriendsView > 0 && this.mainProgression.GetFarthestUnlockedLevelIndex() >= levelRequiredForPlayWithFriendsView;
	}

	private readonly IConfigGetter<FacebookNotification> configGetter;

	private readonly FacebookLoginManager facebookLoginManager;

	private readonly IMainProgression mainProgression;
}
