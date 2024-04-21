using System;
using System.Collections;
using UnityEngine;

public abstract class ReviewManagerBase
{
	public ReviewManagerBase(string appleID, int askEveryNthRun, int minIntervalSeconds)
	{
		this.appleID = appleID;
		this.askEveryNthRun = askEveryNthRun;
		this.minIntervalSeconds = minIntervalSeconds;
		if (ReviewManagerBase.PersistedLastVersion != int.Parse(SystemInfoHelper.BundleVersion))
		{
			ReviewManagerBase.PersistedRunsCount = 0;
			ReviewManagerBase.PersistedShouldAskAgain = true;
			ReviewManagerBase.PersistedLastVersion = int.Parse(SystemInfoHelper.BundleVersion);
		}
	}

	protected abstract IEnumerator ShowAskForReviewView(Action<ReviewManagerBase.AskForReviewViewResult, bool> callback);

	protected abstract IEnumerator ShowRewardForReviewView();

	protected abstract IEnumerator ShowFeedbackView(Action<ReviewManagerBase.FeedbackResult> callback);

	protected abstract void FeedbackNow();

	protected void UpdateConfig(int askEveryNthRun, int minIntervalSeconds)
	{
		this.askEveryNthRun = askEveryNthRun;
		this.minIntervalSeconds = minIntervalSeconds;
	}

	private bool MarkRunInternally()
	{
		if (this.askEveryNthRun < 0)
		{
			return false;
		}
		ReviewManagerBase.PersistedRunsCount++;
		if (ReviewManagerBase.PersistedShouldAskAgain && ReviewManagerBase.PersistedRunsCount >= this.askEveryNthRun && DateTime.UtcNow - ReviewManagerBase.PersistedLastAskTime > TimeSpan.FromSeconds((double)this.minIntervalSeconds))
		{
			ReviewManagerBase.PersistedRunsCount = 0;
			ReviewManagerBase.PersistedLastAskTime = DateTime.UtcNow;
			return true;
		}
		return false;
	}

	public IEnumerator MarkRun(Action<bool> didShow)
	{
		if (this.MarkRunInternally())
		{
			yield return this.AskSequenceCr();
			didShow(true);
		}
		else
		{
			didShow(false);
		}
		yield break;
	}

	private IEnumerator AskSequenceCr()
	{
		ReviewManagerBase.AskForReviewViewResult askForReviewViewResult = ReviewManagerBase.AskForReviewViewResult.EXIT;
		bool dontAskAgain = false;
		yield return this.ShowAskForReviewView(delegate(ReviewManagerBase.AskForReviewViewResult askForReviewViewResultCb, bool dontAskAgainCb)
		{
			askForReviewViewResult = askForReviewViewResultCb;
			dontAskAgain = dontAskAgainCb;
		});
		if (askForReviewViewResult != ReviewManagerBase.AskForReviewViewResult.FIVE_STARS)
		{
			if (askForReviewViewResult != ReviewManagerBase.AskForReviewViewResult.FOUR_OR_LESS)
			{
				ReviewManagerBase.PersistedShouldAskAgain = !dontAskAgain;
			}
			else
			{
				yield return this.AskFeedback();
			}
		}
		else
		{
			this.ReviewNow();
			yield return this.ShowRewardForReviewView();
		}
		yield break;
	}

	private IEnumerator AskFeedback()
	{
		ReviewManagerBase.FeedbackResult feedbackResult = ReviewManagerBase.FeedbackResult.NOT_NOW;
		yield return this.ShowFeedbackView(delegate(ReviewManagerBase.FeedbackResult res)
		{
			feedbackResult = res;
		});
		if (feedbackResult != ReviewManagerBase.FeedbackResult.NOW)
		{
			if (feedbackResult != ReviewManagerBase.FeedbackResult.NEVER)
			{
				this.MaybeLater();
			}
			else
			{
				this.NeverReview();
			}
		}
		else
		{
			this.FeedbackNowInternal();
		}
		yield break;
	}

	public void ReviewNow()
	{
		string text = string.Empty;
		string bundleIdentifier = SystemInfoHelper.BundleIdentifier;
		text = "market://details?id=" + bundleIdentifier;
		if (!string.IsNullOrEmpty(text))
		{
			Application.OpenURL(text);
		}
		ReviewManagerBase.PersistedShouldAskAgain = false;
	}

	public void MaybeLater()
	{
		ReviewManagerBase.PersistedRunsCount = 0;
	}

	public void NeverReview()
	{
		ReviewManagerBase.PersistedShouldAskAgain = false;
	}

	private void FeedbackNowInternal()
	{
		this.FeedbackNow();
		ReviewManagerBase.PersistedShouldAskAgain = false;
	}

	private static int PersistedRunsCount
	{
		get
		{
			return TactilePlayerPrefs.GetInt("ReviewManagerRunsCount", 0);
		}
		set
		{
			TactilePlayerPrefs.SetInt("ReviewManagerRunsCount", value);
		}
	}

	private static DateTime PersistedLastAskTime
	{
		get
		{
			long dateData = Convert.ToInt64(TactilePlayerPrefs.GetString("ReviewManagerLastAskTime", DateTime.MinValue.ToBinary().ToString()));
			return DateTime.FromBinary(dateData);
		}
		set
		{
			TactilePlayerPrefs.SetString("ReviewManagerLastAskTime", value.ToBinary().ToString());
		}
	}

	private static bool PersistedShouldAskAgain
	{
		get
		{
			return TactilePlayerPrefs.GetBool("ReviewManagerShouldAskAgain", true);
		}
		set
		{
			TactilePlayerPrefs.SetBool("ReviewManagerShouldAskAgain", value);
		}
	}

	private static int PersistedLastVersion
	{
		get
		{
			return TactilePlayerPrefs.GetInt("ReviewManagerShouldAskAgain", int.Parse(SystemInfoHelper.BundleVersion));
		}
		set
		{
			TactilePlayerPrefs.SetInt("ReviewManagerShouldAskAgain", value);
		}
	}

	private string appleID;

	private string emailAddress;

	private string emailSubject;

	private string emailBody;

	private int askEveryNthRun;

	private int minIntervalSeconds;

	private const string PREFS_PERSISTED_RUNS_COUNT = "ReviewManagerRunsCount";

	private const string PREFS_LAST_ASK_TIME = "ReviewManagerLastAskTime";

	private const string PREFS_SHOULD_ASK_AGAIN = "ReviewManagerShouldAskAgain";

	private const string PREFS_LAST_VERSION = "ReviewManagerLastVersion";

	public enum AskForReviewViewResult
	{
		FIVE_STARS,
		FOUR_OR_LESS,
		EXIT
	}

	public enum FeedbackResult
	{
		NOW,
		NOT_NOW,
		NEVER
	}
}
