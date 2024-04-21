using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGames.EndlessChallenge.Data;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
    public class EndlessChallengeLocalPersistedState
    {
        private EndlessChallengeHandler Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
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
                string securedString = TactilePlayerPrefs.GetSecuredString("EndlessChallengeNetworkState", string.Empty);
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
                    TactilePlayerPrefs.SetSecuredString("EndlessChallengeNetworkState", value2);
                }
                else
                {
                    TactilePlayerPrefs.SetSecuredString("EndlessChallengeNetworkState", string.Empty);
                }
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

        public void SaveLocalState()
        {
            this.LocalState = this.LocalState;
        }

        public void ResetLocalState()
        {
            this.LocalState = new LocalPersistableState();
        }

        private const string PREFS_NETWORK_STATE = "EndlessChallengeNetworkState";

        private LocalPersistableState cachedLocalState;
    }
}
