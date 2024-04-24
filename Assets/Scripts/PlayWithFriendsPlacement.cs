using System.Collections;
using TactileModules.Placements;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.Configuration;

public class PlayWithFriendsPlacement : IPlacementRunnableNoBreak, IPlacementRunnable
{
	public PlayWithFriendsPlacement(IMainProgression mainProgression)
	{
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
		return false;
	}

	private readonly IMainProgression mainProgression;
}
