using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FacebookRequestManager
{
    public FacebookRequestManager(CloudClientBase cloudClient, FacebookClient facebookClient, Func<bool> isPlayingLevel, Action<FacebookRequestData> successfulRequestHandler, FacebookRequestManager.IDataProvider dataProvider, Func<FacebookRequestData, bool> IsRequestAllowed = null)
    {
        this.cloudClient = cloudClient;
        this.facebookClient = facebookClient;
        this.isPlayingLevel = isPlayingLevel;
        this.successfulRequestHandler = successfulRequestHandler;
        this.dataProvider = dataProvider;
        Func<FacebookRequestData, bool> isRequestAllowed;
        if (IsRequestAllowed == null)
        {
            isRequestAllowed = ((FacebookRequestData request) => true);
        }
        else
        {
            isRequestAllowed = IsRequestAllowed;
        }
        this.IsRequestAllowed = isRequestAllowed;
        cloudClient.UserChanged += this.UserChangedHandler;
        this.lastAskForLifeSent = this.GetPersistedLastSent("lastAskForLifeSent");
        this.lastInviteSent = this.GetPersistedLastSent("lastInviteSent");
        this.lastAskForTournamentLifeSent = this.GetPersistedLastSent("lastAskForTournamentLifeSent");
        this.lastSendLifeToUser = this.GetPersistedLastSent("lastSendLifeToUser");
        this.lastAskForKeySent = this.GetPersistedLastSent("askForKeySent");
        FiberCtrl.Pool.Run(this.Update(), false);
    }

    ~FacebookRequestManager()
    {
        this.cloudClient.UserChanged -= this.UserChangedHandler;
    }

    private void UserChangedHandler(CloudUser user)
    {
        FiberCtrl.Pool.Run(this.SyncRequests(), false);
    }

    private IEnumerator Update()
    {
        float syncCooldown = -1f;
        for (; ; )
        {
            if (!this.isPlayingLevel() && syncCooldown < 0f && !this.syncInprogress)
            {
                if (this.CheckValidFacebookSession())
                {
                    yield return this.SyncRequests();
                }
                syncCooldown = 60f;
            }
            syncCooldown -= Time.unscaledDeltaTime;
            yield return null;
        }
        yield break;
    }

    private bool CheckValidFacebookSession()
    {
        if (this.facebookClient.CachedMe == null || !this.facebookClient.IsSessionValid)
        {
            if (this.pendingRequestData.Count > 0)
            {
                this.pendingRequestData.Clear();
                this.EmitPendingRequestDataUpdated();
            }
            return false;
        }
        return true;
    }

    public IEnumerator SyncRequests()
    {
        if (this.syncInprogress)
        {
            while (this.syncInprogress)
            {
                yield return null;
            }
        }
        else
        {
            this.syncInprogress = true;
            yield return this.SyncRequestsInternal();
            this.syncInprogress = false;
        }
        yield break;
    }

    private bool IsRequestAlreadyPending(FacebookRequest r)
    {
        for (int i = 0; i < this.pendingRequestData.Count; i++)
        {
            if (this.pendingRequestData[i].RequestId == r.Id)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator SyncRequestsInternal()
    {
        if (!this.CheckValidFacebookSession())
        {
            yield break;
        }
        bool newRequestsAdded = false;
        List<FacebookRequestData> requestsToDelete = new List<FacebookRequestData>();
        yield return this.facebookClient.QueryPendingRequests(delegate (object err, List<FacebookRequest> requests)
        {
            if (err == null)
            {
                foreach (FacebookRequest facebookRequest in requests)
                {
                    if (!this.IsRequestAlreadyPending(facebookRequest))
                    {
                        if (FacebookRequestData.IsRequestValidAndSupported(facebookRequest))
                        {
                            FacebookRequestData facebookRequestData = null;
                            string text = facebookRequest.Data["t"] as string;
                            if (text != null)
                            {
                                if (!(text == "askforlives"))
                                {
                                    if (!(text == "life"))
                                    {
                                        if (!(text == "askforTournamentlives"))
                                        {
                                            if (!(text == "tournamentLife"))
                                            {
                                                if (!(text == "askforkey"))
                                                {
                                                    if (text == "key")
                                                    {
                                                        facebookRequestData = JsonSerializer.HashtableToObject<FacebookKeyRequestData>(facebookRequest.Data);
                                                    }
                                                }
                                                else
                                                {
                                                    facebookRequestData = JsonSerializer.HashtableToObject<FacebookAskForKeyRequestData>(facebookRequest.Data);
                                                }
                                            }
                                            else
                                            {
                                                facebookRequestData = JsonSerializer.HashtableToObject<FacebookTournamentLifeRequestData>(facebookRequest.Data);
                                            }
                                        }
                                        else
                                        {
                                            facebookRequestData = JsonSerializer.HashtableToObject<FacebookAskForTournamentLifeRequestData>(facebookRequest.Data);
                                        }
                                    }
                                    else
                                    {
                                        facebookRequestData = JsonSerializer.HashtableToObject<FacebookLifeRequestData>(facebookRequest.Data);
                                    }
                                }
                                else
                                {
                                    facebookRequestData = JsonSerializer.HashtableToObject<FacebookAskForLifeRequestData>(facebookRequest.Data);
                                }
                            }
                            IL_133:
                            if (facebookRequestData == null)
                            {
                                continue;
                            }
                            facebookRequestData.SetRequest(facebookRequest);
                            if (this.IsRequestAllowed(facebookRequestData))
                            {
                                foreach (FacebookRequestData facebookRequestData2 in this.pendingRequestData)
                                {
                                    if (facebookRequestData2.RequestSenderFacebookId == facebookRequestData.RequestSenderFacebookId && facebookRequestData2.RequestType == facebookRequestData.RequestType)
                                    {
                                        facebookRequest.IsConsumed = true;
                                        break;
                                    }
                                }
                                this.pendingRequestData.Add(facebookRequestData);
                                newRequestsAdded = true;
                                continue;
                            }
                            requestsToDelete.Add(facebookRequestData);
                            continue;
                            goto IL_133;
                        }
                    }
                }
            }
        });
        if (newRequestsAdded && this.CheckValidFacebookSession())
        {
            this.EmitPendingRequestDataUpdated();
        }
        yield return this.PruneConsumedPendingRequests(requestsToDelete);
        yield break;
    }

    private IEnumerator PruneConsumedPendingRequests(List<FacebookRequestData> requestsToDelete)
    {
        List<FacebookRequestData> requestsCopy = new List<FacebookRequestData>(this.pendingRequestData);
        foreach (FacebookRequestData request in requestsCopy)
        {
            if (request.IsRequestConsumed)
            {
                yield return this.DeleteRequest(request, delegate (object err)
                {
                    if (err != null)
                    {
                    }
                });
            }
        }
        foreach (FacebookRequestData requestData in requestsToDelete)
        {
            yield return this.DeleteRequest(requestData, delegate (object err)
            {
                if (err != null)
                {
                }
            });
        }
        yield break;
    }

    public IEnumerator DeleteRequest(FacebookRequestData requestData, Action<object> callback)
    {
        requestData.IsRequestConsumed = true;
        IEnumerator e = this.facebookClient.ConsumeRequest(requestData.RequestId, delegate (object err)
        {
            if (err == null)
            {
                this.pendingRequestData.Remove(requestData);
            }
            callback(err);
        });
        while (e.MoveNext())
        {
            object obj = e.Current;
            yield return obj;
        }
        this.EmitPendingRequestDataUpdated();
        yield break;
    }




    public void DeveloperReset()
    {
        this.lastAskForLifeSent = new Dictionary<string, DateTime>();
        this.lastInviteSent = new Dictionary<string, DateTime>();
        this.lastAskForKeySent = new Dictionary<string, DateTime>();
        this.SetPersistedLastSent("lastAskForLifeSent", this.lastAskForLifeSent);
        this.SetPersistedLastSent("lastInviteSent", this.lastInviteSent);
        this.SetPersistedLastSent("askForKeySent", this.lastAskForKeySent);
    }

    public IEnumerable<FacebookAskForLifeRequestData> PendingAskForLives
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (data.RequestType == "askforlives")
                {
                    yield return (FacebookAskForLifeRequestData)data;
                }
            }
            yield break;
        }
    }

    public int PendingAskForLivesCount
    {
        get
        {
            return this.GetPendingRequestCount("askforlives");
        }
    }

    public TimeSpan TimeToNextAskForLife(string facebookId)
    {
        return this.TimeToNextRequest(facebookId, this.lastAskForLifeSent, "lastAskForLifeSent");
    }



    private void MarkAskForLifeSent(string facebookId)
    {
        this.MarkRequestSent(facebookId, this.lastAskForLifeSent, "lastAskForLifeSent");
    }

    public IEnumerable<FacebookLifeRequestData> PendingLives
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (data.RequestType == "life")
                {
                    yield return (FacebookLifeRequestData)data;
                }
            }
            yield break;
        }
    }

    public int PendingLivesCount
    {
        get
        {
            return this.GetPendingRequestCount("life");
        }
    }


    public IEnumerable<FacebookAskForTournamentLifeRequestData> PendingAskForTournamentLives
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (data.RequestType == "askforTournamentlives")
                {
                    yield return (FacebookAskForTournamentLifeRequestData)data;
                }
            }
            yield break;
        }
    }

    public int PendingAskForTournamentLivesCount
    {
        get
        {
            return this.GetPendingRequestCount("askforTournamentlives");
        }
    }

    public TimeSpan TimeToNextAskForTournamentLife(string facebookId)
    {
        return this.TimeToNextRequest(facebookId, this.lastAskForTournamentLifeSent, "lastAskForTournamentLifeSent");
    }

    public TimeSpan TimeToNextSendLifeToUser(string facebookId)
    {
        return this.TimeToNextRequest(facebookId, this.lastSendLifeToUser, "lastSendLifeToUser");
    }



    private void MarkAskForTournamentLifeSent(string facebookId)
    {
        this.MarkRequestSent(facebookId, this.lastAskForTournamentLifeSent, "lastAskForTournamentLifeSent");
    }

    private void MarkSentLifeToUser(string facebookId)
    {
        this.MarkRequestSent(facebookId, this.lastSendLifeToUser, "lastSendLifeToUser");
    }

    public IEnumerable<FacebookTournamentLifeRequestData> PendingTournamentLives
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (data.RequestType == "tournamentLife")
                {
                    yield return (FacebookTournamentLifeRequestData)data;
                }
            }
            yield break;
        }
    }

    public int PendingTournamentLivesCount
    {
        get
        {
            return this.GetPendingRequestCount("tournamentLife");
        }
    }



    public IEnumerable<FacebookAskForKeyRequestData> PendingAskForKeys
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (data.RequestType == "askforkey")
                {
                    yield return (FacebookAskForKeyRequestData)data;
                }
            }
            yield break;
        }
    }

    public int PendingAskForKeyCount
    {
        get
        {
            return this.GetPendingRequestCount("askforkey");
        }
    }

    public TimeSpan TimeToNextAskForKey(string facebookId)
    {
        return this.TimeToNextRequest(facebookId, this.lastAskForKeySent, "askForKeySent");
    }



    private void MarkAskForKeySent(string facebookId)
    {
        this.MarkRequestSent(facebookId, this.lastAskForKeySent, "askForKeySent");
    }

    public IEnumerable<FacebookKeyRequestData> PendingKeys
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (data.RequestType == "key")
                {
                    yield return (FacebookKeyRequestData)data;
                }
            }
            yield break;
        }
    }

    public int PendingKeyCount
    {
        get
        {
            return this.GetPendingRequestCount("key");
        }
    }



    public TimeSpan TimeToNextInvite(string facebookId)
    {
        return this.TimeToNextRequest(facebookId, this.lastInviteSent, "lastInviteSent");
    }


    private void MarkInviteSent(string facebookId)
    {
        this.MarkRequestSent(facebookId, this.lastInviteSent, "lastInviteSent");
    }




    private int GetPendingRequestCount(string requestType)
    {
        int num = 0;
        foreach (FacebookRequestData facebookRequestData in this.pendingRequestData)
        {
            if (facebookRequestData.RequestType == requestType)
            {
                num++;
            }
        }
        return num;
    }

    private TimeSpan TimeToNextRequest(string facebookId, Dictionary<string, DateTime> state, string prefsKey)
    {
        if (state.ContainsKey(facebookId))
        {
            if (state[facebookId] > DateTime.UtcNow)
            {
                this.MarkRequestSent(facebookId, state, prefsKey);
            }
            TimeSpan t = DateTime.UtcNow - state[facebookId];
            return new TimeSpan(1, 0, 0) - t;
        }
        return new TimeSpan(0L);
    }

    private void MarkRequestSent(string facebookId, Dictionary<string, DateTime> state, string prefsKey)
    {
        state[facebookId] = DateTime.UtcNow;
        this.SetPersistedLastSent(prefsKey, state);
    }

    private Dictionary<string, DateTime> GetPersistedLastSent(string key)
    {
        string securedString = TactilePlayerPrefs.GetSecuredString(key, string.Empty);
        Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>();
        if (securedString.Length > 0)
        {
            Hashtable hashtable = securedString.hashtableFromJson();
            IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
                    dictionary[(string)dictionaryEntry.Key] = this.utcEpoch.AddSeconds((double)((long)((double)dictionaryEntry.Value)));
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = (enumerator as IDisposable)) != null)
                {
                    disposable.Dispose();
                }
            }
        }
        return dictionary;
    }

    private void SetPersistedLastSent(string key, Dictionary<string, DateTime> dict)
    {
        if (dict != null)
        {
            Hashtable hashtable = new Hashtable();
            foreach (KeyValuePair<string, DateTime> keyValuePair in dict)
            {
                TimeSpan timeSpan = keyValuePair.Value - this.utcEpoch;
                hashtable[keyValuePair.Key] = (long)timeSpan.TotalSeconds;
            }
            TactilePlayerPrefs.SetSecuredString(key, hashtable.toJson());
        }
        else
        {
            TactilePlayerPrefs.SetSecuredString(key, string.Empty);
        }
    }

    public void EmitPendingRequestDataUpdated()
    {
        if (this.pendingRequestDataUpdated != null)
        {
            this.pendingRequestDataUpdated();
        }
    }

    public List<FacebookRequestData> PendingRequestData
    {
        get
        {
            return this.pendingRequestData;
        }
    }

    public int PendingNonConsumedRequestDataCount
    {
        get
        {
            int num = 0;
            foreach (FacebookRequestData facebookRequestData in this.pendingRequestData)
            {
                if (!facebookRequestData.IsRequestConsumed)
                {
                    num++;
                }
            }
            return num;
        }
    }

    public bool HasPendingNonConsumedRequestData
    {
        get
        {
            foreach (FacebookRequestData facebookRequestData in this.pendingRequestData)
            {
                if (!facebookRequestData.IsRequestConsumed)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public IEnumerable<FacebookRequestData> PendingNonConsumedRequestData
    {
        get
        {
            foreach (FacebookRequestData data in this.pendingRequestData)
            {
                if (!data.IsRequestConsumed)
                {
                    yield return data;
                }
            }
            yield break;
        }
    }

    public void RefreshRequestFilter()
    {
        List<FacebookRequestData> list = new List<FacebookRequestData>();
        foreach (FacebookRequestData facebookRequestData in this.pendingRequestData)
        {
            if (!this.IsRequestAllowed(facebookRequestData))
            {
                list.Add(facebookRequestData);
            }
        }

    }

    public void MakeSureRequestIsMarkedConsumed(FacebookRequestData request)
    {
        request.IsRequestConsumed = true;
    }

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action pendingRequestDataUpdated;

    private Dictionary<string, DateTime> lastAskForLifeSent;

    private Dictionary<string, DateTime> lastSendLifeToUser;

    private Dictionary<string, DateTime> lastInviteSent;

    private Dictionary<string, DateTime> lastAskForTournamentLifeSent;

    private Dictionary<string, DateTime> lastAskForKeySent;

    private DateTime utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private Action<FacebookRequestData> successfulRequestHandler;

    private Func<FacebookRequestData, bool> IsRequestAllowed;

    private CloudClientBase cloudClient;

    private FacebookClient facebookClient;

    private FacebookRequestManager.IDataProvider dataProvider;

    private Func<bool> isPlayingLevel;

    private List<FacebookRequestData> pendingRequestData = new List<FacebookRequestData>();

    private bool syncInprogress;

    private const string PREFS_LAST_ASK_FOR_LIFE_SENT = "lastAskForLifeSent";

    private const string PREFS_LAST_ASK_FOR_TOURNAMENT_LIFE_SENT = "lastAskForTournamentLifeSent";

    private const string PREFS_LAST_SEND_LIFE_TO_USER = "lastSendLifeToUser";

    private const string PREFS_LAST_INVITE_SENT = "lastInviteSent";

    private const string PREFS_LAST_ASK_FOR_KEY_SENT = "askForKeySent";

    public interface IDataProvider
    {
        string GetLocalizedInviteFriendsTitleText();

        string GetLocalizedInviteFriendsMessageText();
    }
}
