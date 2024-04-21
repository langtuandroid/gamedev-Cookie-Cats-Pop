using System;
using System.Collections;
using Tactile;

public class ReviewManager : ReviewManagerBase
{
	public ReviewManager(ConfigurationManager configurationManager) : base(Constants.ITUNES_ID, ConfigurationManager.Get<ReviewConfig>().AskForReviewEveryNthRun, ConfigurationManager.Get<ReviewConfig>().AskForReviewMinInterval)
	{
		configurationManager.ConfigurationUpdated += this.HandleConfigurationUpdated;
	}

	private static bool PersistedGotReward
	{
		get
		{
			return TactilePlayerPrefs.GetBool("ReviewManagerGotReward", false);
		}
		set
		{
			TactilePlayerPrefs.SetBool("ReviewManagerGotReward", value);
		}
	}

	protected override void FeedbackNow()
	{
	}

	private void HandleConfigurationUpdated()
	{
		base.UpdateConfig(ConfigurationManager.Get<ReviewConfig>().AskForReviewEveryNthRun, ConfigurationManager.Get<ReviewConfig>().AskForReviewMinInterval);
	}

	protected override IEnumerator ShowAskForReviewView(Action<ReviewManagerBase.AskForReviewViewResult, bool> callback)
	{
		UIViewManager.UIViewStateGeneric<AskForReviewView> vs = UIViewManager.Instance.ShowView<AskForReviewView>(new object[0]);
		yield return vs.WaitForClose();
		AskForReviewView.Result result = (AskForReviewView.Result)vs.ClosingResult;
		bool reviewResult = result.askForReviewViewResult != ReviewManagerBase.AskForReviewViewResult.EXIT;
		Analytics.Instance.LogReviewViewInteraction(reviewResult, result.dontAskAgain);
		callback(result.askForReviewViewResult, result.dontAskAgain);
		yield break;
	}

	protected override IEnumerator ShowFeedbackView(Action<ReviewManagerBase.FeedbackResult> callback)
	{
		UIViewManager.UIViewStateGeneric<MessageBoxView> vs = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
		{
			L.Get("Thank you!"),
			L.Get("Thank you for your feedback!"),
			L.Get("Ok")
		});
		yield return vs.WaitForClose();
		callback(ReviewManagerBase.FeedbackResult.NEVER);
		yield break;
	}

	protected override IEnumerator ShowRewardForReviewView()
	{
		if (!ReviewManager.PersistedGotReward)
		{
			UIViewManager.UIViewStateGeneric<ReviewRewardView> rewardView = UIViewManager.Instance.ShowView<ReviewRewardView>(new object[0]);
			yield return rewardView.WaitForClose();
			ReviewManager.PersistedGotReward = true;
		}
		yield break;
	}

	private const string PREFS_GOT_REWARD = "ReviewManagerGotReward";
}
