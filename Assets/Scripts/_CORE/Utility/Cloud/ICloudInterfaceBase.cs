using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.TactileCloud;

public interface ICloudInterfaceBase : ICloudResponseEvents
{
	event Action<int> OnServerTimeUpdated;

	string Host { get; set; }

	string UserAuthSecret { set; }

	int LastReceivedServerTimeUnixEpocUTC { get; }

	int ClientAdjustedServerTimeUnixEpocUTC { get; }

	IEnumerator CreateOrUpdateDevice(CloudLocalDevice device, Response result);

	IEnumerator CreateOrUpdateUser(Hashtable userData, Response result);

	IEnumerator LogoutUserFromDevice(string userId, Response result);

	IEnumerator DeleteCloudUser(string cloudId, Response result);

	IEnumerator SendPush(string userId, string receiverId, string message, Dictionary<string, string> payload, Response result);

	IEnumerator GetConfiguration(int version, Response result);

	IEnumerator GetAssetBundles(int version, Response result);

	IEnumerator ReportPurchase(string base64EncodedTransactionReceipt, Dictionary<string, string> eventParams, Response result);

	IEnumerator AdsGetEnabled(Response result);

	IEnumerator AdsGetPriority(Response result);

	IEnumerator AdsReportImpression(string adType, string adLocation, string provider, Response result);

	IEnumerator AdsReportRequest(string adType, string adLocation, string provider, Response result);

	IEnumerator ReportAdjustIOAttribution(Hashtable data, Response result);

	IEnumerator CheckFacebookPayment(long paymentId, Response result);

	IEnumerator UploadBackupUserSettings(string userSettingsJson, string deviceId, Response result);

	IEnumerator DownloadBackupUserSettings(string deviceId, Response result);

	IEnumerator GetFeatures(string userId, Hashtable targetingParams, Dictionary<string, int> metaDataVersions, List<string> activeFeatureIds, Response result);

	IEnumerator StarTournamentJoin(string userId, string featureId, int farthestLevelIndex, Response result);

	IEnumerator StarTournamentStatus(string userId, string featureId, RequestPriority requestPriority, Response result);

	IEnumerator StarTournamentSubmitScore(string userId, string featureId, int farthestUnlockLevel, int stars, RequestPriority requestPriority, Response result);

	IEnumerator StarTournamentPresent(string userId, string featureId, RequestPriority requestPriority, Response result);

	IEnumerator SocialChallengeStatus(string userId, bool isPlaying, int challengeVersion, DateTime expirationDate, RequestPriority requestPriority, Response result);

	IEnumerator SocialChallengeReseat(string userId, int challengeVersion, RequestPriority requestPriority, Response result);

	IEnumerator SocialChallengeSubmitLevelCompleted(string userId, int challengeVersion, int levelId, RequestPriority requestPriority, Response result);

	IEnumerator SocialChallengeSubmitChestOpened(string userId, int challengeVersion, int chestRank, RequestPriority requestPriority, Response result);

	IEnumerator SocialChallengeGetOtherPlayers(string userId, int challengeVersion, RequestPriority requestPriority, Response result);

	IEnumerator LevelDashJoinRequest(string userId, string featureId, int completedLevelHumanNumber, Response result);

	IEnumerator LevelDashStatusRequest(string userId, string featureId, RequestPriority requestPriority, Response result);

	IEnumerator LevelDashSubmitScoreRequest(string userId, string featureId, int completedLevelHumanNumber, RequestPriority requestPriority, Response result);

	IEnumerator LevelDashGetRewardRequest(string userId, string featureId, RequestPriority requestPriority, Response result);

	IEnumerator UserSupportSubmitMessage(string userId, string message, string email, string name, Hashtable metaData, Response result, string messageContext = "", ArrayList files = null);

	IEnumerator UserSupportGetMessages(string userId, Response result);

	IEnumerator UserSupportGetArticles(string userId, Response result);

	IEnumerator UserSupportSetRead(string userId, Response result);

	IEnumerator UserSupportClaimAttachments(string userId, Response result);

	IEnumerator UserSupportClaimMessageAttachments(string userId, string messageId, string[] attachmentNames, Response result);

	IEnumerator UserSupportCheckUnread(string userId, Response result);

	IEnumerator UserSupportDismissedBackup(string messageId, string userId, Response result);

	IEnumerator UserSupportAppliedBackup(string messageId, string userId, Response result);

	IEnumerator UserSupportGetPartialImageUploadRequestParameters(string userId, Response result);

	IEnumerator EndlessChallengeJoin(string userId, string featureId, int farthestLevelIndex, Response result);

	IEnumerator EndlessChallengeStatus(string userId, string featureId, Response result);

	IEnumerator EndlessChallengeSubmitScore(string userId, string featureId, int maxRows, Response result);

	IEnumerator GenericTournamentJoin(string userId, string featureId, Hashtable joinParams, Hashtable score, Response result, RequestPriority requestPriority);

	IEnumerator GenericTournamentStatus(string userId, string featureId, Response result, RequestPriority requestPriority);

	IEnumerator GenericTournamentSubmitScore(string userId, string featureId, Hashtable score, Response result, RequestPriority requestPriority);

	IEnumerator GenericTournamentPresent(string userId, string featureId, Response result, RequestPriority requestPriority);

	IEnumerator GenericTournamentGetPastUnclaimed(string featureType, Response result, RequestPriority requestPriority);

	IEnumerator LevelRatingGetRatings(string[] levelContentHashes, Response result);

	IEnumerator GetAbTests(List<string> activeTestIds, List<string> supportedTestTypeIds, Hashtable targetingParams, ICloudResponse result);
}
