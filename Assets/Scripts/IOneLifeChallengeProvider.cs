using System;
using System.Collections;
using TactileModules.FeatureManager.DataClasses;

public interface IOneLifeChallengeProvider
{
	IEnumerator ClaimReward();

	IEnumerator ShowEventStartView();

	IEnumerator ShowEventStartSessionView();

	IEnumerator ShowEventEndedView();

	IEnumerator ShowLevelResultView();

	string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
}
