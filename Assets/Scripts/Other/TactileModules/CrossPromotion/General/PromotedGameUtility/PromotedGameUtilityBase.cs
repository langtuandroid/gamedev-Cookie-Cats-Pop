using System;
using System.Collections;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;
using TactileModules.TactileLogger;
using UnityEngine;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
    public abstract class PromotedGameUtilityBase : IPromotedGameUtility
    {
        protected PromotedGameUtilityBase(string currentGameId)
        {
            this.currentGameId = currentGameId;
        }

        protected abstract string AdvertisementPlatform { get; }

        protected abstract string AdvertisementId { get; }

        public abstract bool IsGameInstalled(IPromotedGameUtilityPrimaryData utilityData);

        public abstract void LaunchGame(IPromotedGameUtilityPrimaryData utilityData);

        public abstract void SendToStore(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData, Action onComplete);

        public abstract void LogAdjustImpression(IPromotedGameUtilityData utilityData, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData);

        protected void LogAdjustImpressionInternal(string tracker, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData)
        {
            string url = this.ConstructAdjustLoggingUrl("https://view.adjust.com/impression/{0}?campaign={1}&adgroup={2}&creative={3}&{4}={5}&request_id={6}&campaign_id={7}&package_name={8}&device_id={9}", tracker, adGroupContext, additionalData);
            FiberCtrl.Pool.Run(PromotedGameUtilityBase.DoAdjustRequest(url, null), false);
        }

        protected void LogAdjustStoreInternal(string tracker, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData, Action onComplete)
        {
            string url = this.ConstructAdjustLoggingUrl("https://app.adjust.com/{0}?campaign={1}&adgroup={2}&creative={3}&{4}={5}&request_id={6}&campaign_id={7}&package_name={8}&device_id={9}", tracker, adGroupContext, additionalData);
            FiberCtrl.Pool.Run(PromotedGameUtilityBase.DoAdjustRequest(url, onComplete), false);
        }

        private string ConstructAdjustLoggingUrl(string adjustUrl, string trackerName, AdGroupContext adGroupContext, IPromotedGameUtilityAdditionalData additionalData)
        {
            string deviceID = SystemInfoHelper.DeviceID;
            string text = null;
            string text2 = null;
            string text3 = null;
            if (additionalData != null)
            {
                text = additionalData.TrackerCreativeName;
                text2 = additionalData.RequestId;
                text3 = additionalData.CampaignId;
            }
            return string.Format(adjustUrl, new object[]
            {
                trackerName,
                this.currentGameId,
                adGroupContext,
                text,
                this.AdvertisementPlatform,
                this.AdvertisementId,
                text2,
                text3,
                SystemInfoHelper.BundleIdentifier,
                deviceID
            });
        }

        private static IEnumerator DoAdjustRequest(string url, Action onComplete)
        {

            using (WWW www = new WWW(url))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    Log.Info(Category.CrossPromotion, () => "PromotedGameUtilityBase::DoAdjustRequest - logged: " + url, null);
                }
                else
                {
                    Log.Info(Category.CrossPromotion, () => "PromotedGameUtilityBase::DoAdjustRequest - not logged. Error: " + www.error, null);
                }
                if (onComplete != null)
                {
                    onComplete();
                }
            }
            yield break;
        }

        private readonly string currentGameId;

        private const string STORE_URL = "https://app.adjust.com/{0}?campaign={1}&adgroup={2}&creative={3}&{4}={5}&request_id={6}&campaign_id={7}&package_name={8}&device_id={9}";

        private const string IMPRESSION_URL = "https://view.adjust.com/impression/{0}?campaign={1}&adgroup={2}&creative={3}&{4}={5}&request_id={6}&campaign_id={7}&package_name={8}&device_id={9}";
    }
}
