using System;
using System.Collections;

public class AchievementsHelper : AchievementsManager.IAchievementsHelper
{
	public static void HandleMissionComplete(AchievementAsset mission)
	{
	}

	IEnumerator AchievementsManager.IAchievementsHelper.QueueNotificationView(AchievementAsset definition)
	{
		while (this.currentlyShowingNotification)
		{
			yield return null;
		}
		this.currentlyShowingNotification = true;
		UIViewManager.UIViewStateGeneric<AchievementsNotificationView> vs = UIViewManager.Instance.ShowView<AchievementsNotificationView>(new object[]
		{
			definition
		});
		yield return vs.WaitForClose();
		this.currentlyShowingNotification = false;
		yield break;
	}

	public const string SCOPE_PLAY_LEVEL = "PlayLevel";

	public const string SCOPE_SINGLE_MOVE = "SingleMove";

	public const string SCOPE_AFTERMATH = "AfterMath";

	private bool currentlyShowingNotification;
}
