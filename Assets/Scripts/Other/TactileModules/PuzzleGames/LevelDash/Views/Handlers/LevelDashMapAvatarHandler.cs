using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.LevelDash.Providers;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelDash.Views.Handlers
{
    public class LevelDashMapAvatarHandler : IDisposable
    {
        public LevelDashMapAvatarHandler(MapContentController mapContentController, ILevelDashMapAvatarModifierProvider levelDashMapAvatarModifierProvider)
        {
            this.mapContentController = mapContentController;
            this.levelDashManager = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<LevelDashManager>();
            this.levelDashMapAvatarModifierProvider = levelDashMapAvatarModifierProvider;
            this.mapContentController.Avatars.AvatarsChanged += this.HandleAvatarsChanged;
            this.levelDashManager.OnLevelDashStarted += this.OnLevelDashActivationStateChanged;
            this.levelDashManager.OnLevelDashEntriesRefresh += this.OnLevelDashActivationStateChanged;
            this.levelDashManager.OnLevelDashEnded += this.OnLevelDashActivationStateChanged;
        }

        public void Dispose()
        {
            this.mapContentController.Avatars.AvatarsChanged -= this.HandleAvatarsChanged;
            this.levelDashManager.OnLevelDashStarted -= this.OnLevelDashActivationStateChanged;
            this.levelDashManager.OnLevelDashEntriesRefresh -= this.OnLevelDashActivationStateChanged;
            this.levelDashManager.OnLevelDashEnded -= this.OnLevelDashActivationStateChanged;
        }

        private void HandleAvatarsChanged(SagaAvatarController avatarController)
        {
            foreach (KeyValuePair<string, MapAvatare> keyValuePair in avatarController.Avatars)
            {
                this.ModifyAvatarForLevelDash(keyValuePair.Value, keyValuePair.Key == "me");
            }
        }

        private void ModifyAvatarForLevelDash(MapAvatare mapAvatar, bool isMyAvatar)
        {
            if (mapAvatar.DeviceId == null || this.levelDashManager == null || this.levelDashMapAvatarModifierProvider == null)
            {
                return;
            }
            if (!this.levelDashManager.IsActive())
            {
                mapAvatar.RemoveAdditionalIcon(this.levelDashManager.FeatureType);
                return;
            }
            string userId = (mapAvatar.CloudUser != null) ? mapAvatar.CloudUser.CloudId : null;
            GameObject avatarLeaderIconPrefab = this.GetAvatarLeaderIconPrefab(mapAvatar.DeviceId, userId);
            if (avatarLeaderIconPrefab != null)
            {
                mapAvatar.RemoveAdditionalIcon(this.levelDashManager.FeatureType);
                mapAvatar.AddAdditionalIcon(this.levelDashManager.FeatureType, avatarLeaderIconPrefab);
            }
            string text = (mapAvatar.CloudUser != null) ? mapAvatar.CloudUser.FacebookId : null;
            if (!string.IsNullOrEmpty(mapAvatar.DeviceId) && !isMyAvatar)
            {
                if (string.IsNullOrEmpty(text))
                {
                    Texture2D randomPortraitTexture = this.GetRandomPortraitTexture(mapAvatar.DeviceId);
                    if (randomPortraitTexture != null)
                    {
                        
                    }
                }
                else
                {
                    
                }
            }
        }

        private void OnLevelDashActivationStateChanged(LevelDashManager dashManager)
        {
            this.HandleAvatarsChanged(this.mapContentController.Avatars);
        }

        private GameObject GetAvatarLeaderIconPrefab(string deviceId, string userId)
        {
            int rank = this.levelDashManager.GetRank(deviceId, userId);
            if (rank >= 0 && rank <= this.levelDashManager.GetRewardsCount())
            {
                return Resources.Load<GameObject>(this.levelDashMapAvatarModifierProvider.GetAvatarLeaderIconPrefabPath(rank));
            }
            return null;
        }

        private Texture2D GetRandomPortraitTexture(string deviceId)
        {
            return RandomHelper.RandomPortrait(deviceId.GetHashCode());
        }

        private MapContentController mapContentController;

        private LevelDashManager levelDashManager;

        private ILevelDashMapAvatarModifierProvider levelDashMapAvatarModifierProvider;
    }
}
