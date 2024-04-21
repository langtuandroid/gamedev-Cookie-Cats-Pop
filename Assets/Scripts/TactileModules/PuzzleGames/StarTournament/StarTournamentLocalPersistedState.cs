using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.StarTournament
{
    public sealed class StarTournamentLocalPersistedState
    {
        private StarTournamentManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<StarTournamentManager>();
            }
        }

        public int ReceivedResult
        {
            get
            {
                return TactilePlayerPrefs.GetInt("StarTournamentResultReceivedState", -100);
            }
            set
            {
                TactilePlayerPrefs.SetInt("StarTournamentResultReceivedState", value);
            }
        }

        public LocalPersistableState LocalState
        {
            get
            {
                if (this.cachedLocalState != null)
                {
                    return this.cachedLocalState;
                }
                string securedString = TactilePlayerPrefs.GetSecuredString("StarTournamentNetworkState", string.Empty);
                this.cachedLocalState = new LocalPersistableState();
                if (securedString.Length > 0)
                {
                    this.cachedLocalState = JsonSerializer.HashtableToObject<LocalPersistableState>(securedString.hashtableFromJson());
                }
                return this.cachedLocalState;
            }
            set
            {
                if (value != null)
                {
                    this.cachedLocalState = value;
                    string value2 = JsonSerializer.ObjectToHashtable(this.cachedLocalState).toJson();
                    TactilePlayerPrefs.SetSecuredString("StarTournamentNetworkState", value2);
                }
                else
                {
                    TactilePlayerPrefs.SetSecuredString("StarTournamentNetworkState", string.Empty);
                }
            }
        }

        public int PersistedUnsubmittedStars
        {
            get
            {
                if (this.unsubmittedStars == -1)
                {
                    this.unsubmittedStars = TactilePlayerPrefs.GetInt("StarTournamentUnsubmittedStars", 0);
                }
                return this.unsubmittedStars;
            }
            set
            {
                this.unsubmittedStars = value;
                TactilePlayerPrefs.SetInt("StarTournamentUnsubmittedStars", value);
            }
        }

        public void UpdateState(List<Entry> entries, List<CloudUser> users, Entry entry)
        {
            if (entries != null)
            {
                this.LocalState.Entries = entries;
                if (entry == null)
                {
                    string deviceId = (!this.Manager.CloudClient.HasValidDevice) ? string.Empty : this.Manager.CloudClient.CachedDevice.CloudId;
                    string userId = (!this.Manager.CloudClient.HasValidUser) ? string.Empty : this.Manager.CloudClient.CachedMe.CloudId;
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i].IsOwnedByDeviceOrUser(deviceId, userId))
                        {
                            this.LocalState.Entry = entries[i];
                        }
                    }
                }
            }
            if (users != null)
            {
                this.LocalState.Users = users;
            }
            if (entry != null)
            {
                this.LocalState.Entry = entry;
            }
            this.SaveLocalState();
        }

        public bool HasReceivedFinalStatus
        {
            get
            {
                return TactilePlayerPrefs.GetBool("StarTournamentReceivedFinalStatus");
            }
            set
            {
                TactilePlayerPrefs.SetBool("StarTournamentReceivedFinalStatus", value);
            }
        }

        public void SaveLocalState()
        {
            this.LocalState = this.LocalState;
        }

        public void ResetLocalState()
        {
            this.HasReceivedFinalStatus = false;
            this.LocalState = new LocalPersistableState();
        }

        private const string PREFS_NETWORK_STATE = "StarTournamentNetworkState";

        private const string UNSUBMITTED_STARS_KEY = "StarTournamentUnsubmittedStars";

        private const string PREFS_RESULT_RECEIVED_STATE = "StarTournamentResultReceivedState";

        private const string RECEIVED_FINAL_STATUS_KEY = "StarTournamentReceivedFinalStatus";

        private LocalPersistableState cachedLocalState;

        private int unsubmittedStars = -1;
    }
}
