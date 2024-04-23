using System;
using System.Collections.Generic;
using TactileModules.Foundation;

public class CloudScoreHelper
{
	public static CloudScore MyScore(List<CloudScore> scores)
	{
		foreach (CloudScore cloudScore in scores)
		{
			if (CloudScoreHelper.IsMe(cloudScore))
			{
				return cloudScore;
			}
		}
		return null;
	}

	public static CloudScore ScoreForUserId(List<CloudScore> scores, string userId)
	{
		foreach (CloudScore cloudScore in scores)
		{
			if (cloudScore.UserId == userId)
			{
				return cloudScore;
			}
		}
		return null;
	}

	public static int MyPosition(List<CloudScore> scores)
	{
		CloudScore cloudScore = CloudScoreHelper.MyScore(scores);
		if (cloudScore == null)
		{
			return -1;
		}
		return scores.IndexOf(cloudScore);
	}

	public static int PositionForUserId(List<CloudScore> scores, string userId)
	{
		CloudScore cloudScore = CloudScoreHelper.ScoreForUserId(scores, userId);
		if (cloudScore == null)
		{
			return -1;
		}
		return scores.IndexOf(cloudScore);
	}

	public static bool HasProgressed(string userId, List<CloudScore> oldScores, List<CloudScore> newScores)
	{
		int num = CloudScoreHelper.PositionForUserId(oldScores, userId);
		int num2 = CloudScoreHelper.PositionForUserId(newScores, userId);
		return num < 0 || num2 < num;
	}

	public static bool HasProgressed(List<CloudScore> oldScores, List<CloudScore> newScores)
	{
		CloudClient cloudClient = ManagerRepository.Get<CloudClient>();
		string userId = (!cloudClient.HasValidUser) ? string.Empty : cloudClient.CachedMe.CloudId;
		return CloudScoreHelper.HasProgressed(userId, oldScores, newScores);
	}

	public static bool IsMe(CloudScore cloudScore)
	{
		CloudClient cloudClient = ManagerRepository.Get<CloudClient>();
		string b = (!cloudClient.HasValidUser) ? string.Empty : cloudClient.CachedMe.CloudId;
		return cloudScore.UserId == b;
	}

	public const int NO_POSITION = -1;
}
