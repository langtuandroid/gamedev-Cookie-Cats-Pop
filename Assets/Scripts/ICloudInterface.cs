using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;

public interface ICloudInterface
{
	IEnumerator ReportPurchase(string base64EncodedTransactionReceipt, Dictionary<string, string> eventParams, Response result);

	IEnumerator GetOtherUserSettings(string otherUserId, Response result);

	IEnumerator GetConfiguration(int version, string woogaId, Response result);

	IEnumerator CreateUserSettings(string userId, Hashtable PrivateUserSettings, Hashtable publicSettings, Response result);

	IEnumerator UpdateUserSettings(string userId, Hashtable PrivateUserSettings, Hashtable publicSettings, int version, Response result);

	IEnumerator GetUserSettings(string userId, Response result);

	IEnumerator GetFriendsAndUserSettings(string userId, Response result);

	IEnumerator LeaderboardsSubmitScore(string userId, int score, int leaderboard, int videoId, Response result);

	IEnumerator LeaderboardsGetScores(string userId, int leaderboard, Response result);

	IEnumerator TournamentStatus(string userId, Response result);

	IEnumerator TournamentJoin(string userId, TournamentCloudManager.Type tournamentType, Response result);

	IEnumerator TournamentSubmitScore(string userId, int periodId, int tournamentId, int leaderboard, int score, Response result);

	IEnumerator TournamentGetEntries(string userId, int periodId, int tournamentId, Response result);

	IEnumerator TournamentPresent(string userId, int periodId, int tournamentId, Response result);

	IEnumerator PatchUserSettings(string userId, Hashtable objPathsToSet, Hashtable objPathsToUnset, int version, Response result);
}
