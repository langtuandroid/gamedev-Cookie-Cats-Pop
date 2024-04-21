using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.Foundation.CloudSynchronization;
using TactileModules.PuzzleGames.Configuration;
using UnityEngine;

namespace Tactile
{
	public class ConfigurationManager : SingleInstance<ConfigurationManager>, ICloudSynchronizable, IConfigurationManager
	{
		public ConfigurationManager(CloudClientBase cloudClient)
		{
			this.cloudClient = cloudClient;
			this.configClasses = ConfigurationManager.CollectConfigClassesFromAssembly();
			if (ConfigurationManager.PersistedLastSeenBundleVersion != int.Parse(SystemInfoHelper.BundleVersion))
			{
				ConfigurationManager.PersistedConfiguration = null;
			}
			ConfigurationManager.PersistedLastSeenBundleVersion = int.Parse(SystemInfoHelper.BundleVersion);
			Hashtable persistedConfiguration = ConfigurationManager.PersistedConfiguration;
			Hashtable newConfig;
			if (persistedConfiguration != null)
			{
				newConfig = persistedConfiguration;
			}
			else
			{
				newConfig = this.LoadDefaultConfiguration();
			}
			this.ReflectConfigHashtableToObjects(newConfig);
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ConfigurationUpdated;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ConfigurationDownloaded;



		public Hashtable LoadDefaultConfiguration()
		{
			TextAsset textAsset = Resources.Load("Configuration/configuration") as TextAsset;
			return textAsset.text.hashtableFromJson();
		}

		public void ForceDownloadConfiguration()
		{
			this.basicConfig.Version = 0;
			FiberCtrl.Pool.Run(this.DownloadConfigurationCr(), false);
		}

		public IEnumerator DownloadConfigurationCr()
		{
			if (this.loadInProgress)
			{
				while (this.loadInProgress)
				{
					yield return null;
				}
			}
			else
			{
				this.loadInProgress = true;
				yield return this.DownloadConfigurationInternalCr();
				this.loadInProgress = false;
			}
			yield break;
		}

		private IEnumerator DownloadConfigurationInternalCr()
		{
			while (this.cloudClient.CachedDevice == null)
			{
				yield return null;
			}
			yield return this.cloudClient.GetConfiguration(this.basicConfig.Version, delegate(object error, Hashtable latestConfig)
			{
				if (latestConfig != null)
				{
					ConfigurationManager.PersistedConfiguration = latestConfig;
					this.ReflectConfigHashtableToObjects(latestConfig);
					this.ConfigurationDownloaded();
					this.ConfigurationUpdated();
				}
			});
			yield break;
		}

		public void MarkConfigUpdatedExternally()
		{
			this.ConfigurationUpdated();
		}

		public static void ClearPersistedConfiguration()
		{
			ConfigurationManager.PersistedConfiguration = null;
		}

		private void ReflectConfigHashtableToObjects(Hashtable newConfig)
		{
			ConfigurationManager.ConfigurationToObjects(newConfig, this.configClasses, out this.basicConfig, out this.configAsObjects);
		}

		private static Dictionary<string, Type> CollectConfigClassesFromAssembly()
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Dictionary<Type, ConfigProviderAttribute> dictionary2 = ConfigProviderAttribute.CollectClassesInAssembly();
			foreach (KeyValuePair<Type, ConfigProviderAttribute> keyValuePair in dictionary2)
			{
				dictionary.Add(keyValuePair.Value.Name, keyValuePair.Key);
			}
			return dictionary;
		}

		private static void ConfigurationToObjects(Hashtable config, Dictionary<string, Type> configClasses, out ConfigurationManager.BasicConfiguration basicObject, out Dictionary<string, object> subObjects)
		{
			basicObject = JsonSerializer.HashtableToObject<ConfigurationManager.BasicConfiguration>(config);
			subObjects = new Dictionary<string, object>();
			foreach (KeyValuePair<string, Type> keyValuePair in configClasses)
			{
				object obj = JsonSerializer.HashtableToObject(keyValuePair.Value, config[keyValuePair.Key] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
				if (obj == null)
				{
					obj = Activator.CreateInstance(keyValuePair.Value, true);
				}
				subObjects.Add(keyValuePair.Key, obj);
			}
		}

		public void CleanDownloadCache()
		{
			TactilePlayerPrefs.DeleteKey("ConfigurationManagerConfiguration");
			TactilePlayerPrefs.DeleteKey("ConfigurationManagerLastSeenBundleVersion");
		}

		private static Hashtable PersistedConfiguration
		{
			get
			{
				string securedString = TactilePlayerPrefs.GetSecuredString("ConfigurationManagerConfiguration", string.Empty);
				if (securedString.Length > 0)
				{
					return securedString.hashtableFromJson();
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					TactilePlayerPrefs.SetSecuredString("ConfigurationManagerConfiguration", value.toJson());
				}
				else
				{
					TactilePlayerPrefs.DeleteKey("ConfigurationManagerConfiguration");
				}
			}
		}

		private static int PersistedLastSeenBundleVersion
		{
			get
			{
				return TactilePlayerPrefs.GetInt("ConfigurationManagerLastSeenBundleVersion", 0);
			}
			set
			{
				TactilePlayerPrefs.SetInt("ConfigurationManagerLastSeenBundleVersion", value);
			}
		}

		public static T Get<T>()
		{
			return SingleInstance<ConfigurationManager>.instance.GetConfig<T>();
		}

		public T GetConfig<T>()
		{
			foreach (KeyValuePair<string, Type> keyValuePair in this.configClasses)
			{
				if (keyValuePair.Value == typeof(T))
				{
					return (T)((object)this.configAsObjects[keyValuePair.Key]);
				}
			}
			return default(T);
		}

		public int GetVersion()
		{
			if (this.basicConfig != null)
			{
				return this.basicConfig.Version;
			}
			return -1;
		}

		public IEnumerator Synchronize()
		{
			if (this.cloudClient.CachedDevice == null)
			{
				yield break;
			}
			yield return this.DownloadConfigurationCr();
			yield break;
		}

		private const string PREFS_CONFIGURATION = "ConfigurationManagerConfiguration";

		private const string PREFS_LAST_SEEN_BUNDLE_VERSION = "ConfigurationManagerLastSeenBundleVersion";

		private const string VERSION = "Version";

		private CloudClientBase cloudClient;

		private ConfigurationManager.BasicConfiguration basicConfig;

		private Dictionary<string, object> configAsObjects;

		private bool loadInProgress;

		private Dictionary<string, Type> configClasses = new Dictionary<string, Type>();

		public class BasicConfiguration
		{
			[JsonSerializable("Version", null)]
			public int Version { get; set; }
		}
	}
}
