using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.UserSupport.DataProviders
{
    public class MessageMetaData : IMessageMetaData
    {
        public MessageMetaData( FacebookClient facebookClient, CloudClientBase cloudClient, IClientMessageMetaDataProvider clientMessageMetaDataProvider)
        {
            this.facebookClient = facebookClient;
            this.cloudClient = cloudClient;
            this.clientMessageMetaDataProvider = clientMessageMetaDataProvider;
        }

        public Hashtable CreateMetaData()
        {
            Dictionary<string, string> dictionary = this.CreatePlatformParameters();
            this.AddUserParameters(dictionary);
            this.AddFacebookProperties(dictionary);
            this.AddCloudProperties(dictionary);
            Hashtable hashtable = new Hashtable(dictionary);
            this.AddUserSettings(hashtable);
            this.AddCostumProperties(hashtable);
            return hashtable;
        }

        private Dictionary<string, string> CreatePlatformParameters()
        {
            return new Dictionary<string, string>
            {
                {
                    "Version",
                    SystemInfoHelper.BundleShortVersion + "-" + SystemInfoHelper.BundleVersion
                },
                {
                    "DeviceID",
                    SystemInfoHelper.DeviceID
                },
                {
                    "DeviceType",
                    SystemInfoHelper.DeviceType
                },
                {
                    "DeviceModel",
                    SystemInfo.deviceModel
                },
                {
                    "DeviceManufacturer",
                    SystemInfoHelper.Manufacturer
                },
                {
                    "DeviceOperatingSystem",
                    SystemInfo.operatingSystem
                },
                {
                    "DeviceLanguage",
                    Application.systemLanguage.ToString()
                }
            };
        }

        private void AddUserParameters(Dictionary<string, string> parameters)
        {
            parameters.Add("UserIsPaying", PuzzleGameData.PlayerState.IsPayingUser.ToString());
        }

        private void AddFacebookProperties(Dictionary<string, string> parameters)
        {
            if (this.facebookClient.IsSessionValid && this.facebookClient.CachedMe != null)
            {
                parameters.Add("DisplayName", this.facebookClient.CachedMe.Name);
                parameters.Add("FacebookID", this.facebookClient.CachedMe.Id);
                parameters.Add("FacebookAccessToken", this.facebookClient.AccessToken);
                if (!string.IsNullOrEmpty(this.facebookClient.CachedMe.Email))
                {
                    parameters.Add("FacebookEmail", this.facebookClient.CachedMe.Email);
                }
            }
        }

        private void AddCloudProperties(Dictionary<string, string> parameters)
        {
            if (this.cloudClient.HasValidDevice)
            {
                parameters.Add("TactileDeviceID", this.cloudClient.CachedDevice.CloudId);
            }
            if (this.cloudClient.HasValidUser)
            {
                parameters.Add("TactileUserID", this.cloudClient.CachedMe.CloudId);
            }
        }

        private void AddUserSettings(Hashtable metaData)
        {
            Hashtable privateUserSettings;
            Hashtable publicUserSettings;
            this.clientMessageMetaDataProvider.AddUserSettings(out privateUserSettings, out publicUserSettings);
            this.AddUserSettingsToMetaDataTable(metaData, privateUserSettings, publicUserSettings);
        }

        private void AddUserSettingsToMetaDataTable(Hashtable metaData, Hashtable privateUserSettings, Hashtable publicUserSettings)
        {
            Hashtable json = new Hashtable
            {
                {
                    "privateSettings",
                    privateUserSettings
                },
                {
                    "publicSettings",
                    publicUserSettings
                }
            };
            metaData["userSettings"] = MiniJSON.jsonEncode(json, false, 0);
        }

        private void AddCostumProperties(Hashtable metaData)
        {
            Hashtable json;
            this.clientMessageMetaDataProvider.AddCustomData(out json);
            metaData["customData"] = MiniJSON.jsonEncode(json, false, 0);
        }

        private readonly FacebookClient facebookClient;

        private readonly CloudClientBase cloudClient;

        private readonly IClientMessageMetaDataProvider clientMessageMetaDataProvider;
    }
}
