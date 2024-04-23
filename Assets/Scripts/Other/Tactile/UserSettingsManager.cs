using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace Tactile
{
	public class UserSettingsManager : IUserSettings
	{
		private UserSettingsManager(int dataVersion, ICloudUserSettingsProvider cloudClient, Action<UserSettingsManager> defaultSettingsHandler, Action<Hashtable, Hashtable> versionConverter)
		{
			this.currentDataVersion = dataVersion;
			if (defaultSettingsHandler != null)
			{
				this.defaultSettingsHandler = defaultSettingsHandler;
			}
			if (versionConverter != null)
			{
				this.versionConverter = versionConverter;
			}
			this.availableSettingsProviders = SettingsProviderAttribute.CollectClassesInAssembly();
			this.cloudClient = cloudClient;
			this.LoadCachedCloudUserSettings();
			this.LoadLocalSettings();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<UserSettingsManager> SettingsSynced;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action SettingsSaved;



		public static UserSettingsManager Instance
		{
			get
			{
				return UserSettingsManager.instance;
			}
		}

		private bool LocalSettingsModified
		{
			get
			{
				if (this.CloudSettings != null)
				{
					Hashtable localSettingsAsTable = this.GetLocalSettingsAsTable(this.localPublicUserSettings);
					bool flag = HashtableDifference.AnyDifference(this.CloudSettings.Public, localSettingsAsTable);
					if (!flag)
					{
						Hashtable localSettingsAsTable2 = this.GetLocalSettingsAsTable(this.localPrivateUserSettings);
						flag = HashtableDifference.AnyDifference(this.CloudSettings.Private, localSettingsAsTable2);
					}
					return flag;
				}
				return false;
			}
		}

		private CloudUserSettings CloudSettings
		{
			get
			{
				return this.cloudSettings;
			}
			set
			{
				this.cloudSettings = value;
				this.SaveCachedCloudUserSettings();
			}
		}

		public static UserSettingsManager CreateInstance(int dataVersion, ICloudUserSettingsProvider cloudClient, Action<UserSettingsManager> defaultSettingsHandler, Action<Hashtable, Hashtable> versionConverter)
		{
			UserSettingsManager.instance = new UserSettingsManager(dataVersion, cloudClient, defaultSettingsHandler, versionConverter);
			return UserSettingsManager.instance;
		}

		public static UserSettingsManager CreateInstance(ICloudUserSettingsProvider cloudClient, Action<UserSettingsManager> defaultSettingsHandler, Action<Hashtable, Hashtable> versionConverter)
		{
			UserSettingsManager.instance = new UserSettingsManager(0, cloudClient, defaultSettingsHandler, versionConverter);
			return UserSettingsManager.instance;
		}

		private void FlushAllSavedDataWebGL()
		{
			TactilePlayerPrefs.SetSecuredString("UserSettingsManagerCachedCloudUserSettings", string.Empty);
			TactilePlayerPrefs.SetSecuredString("UserSettingsManagerLocalPublicUserSettings", string.Empty);
			TactilePlayerPrefs.SetSecuredString("UserSettingsManagerLocalPrivateUserSettings", string.Empty);
			this.localPublicUserSettings = new Dictionary<string, IPersistableState>();
			this.localPrivateUserSettings = new Dictionary<string, IPersistableState>();
		}

		private void TryInjectTestSettingsIntoLocalStorage()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("TestSettings");
			if (textAsset != null)
			{
				Hashtable hashtable = textAsset.text.hashtableFromJson();
				UserSettingsManager.SaveLocalSettingsTable("UserSettingsManagerLocalPrivateUserSettings", hashtable["privateSettings"] as Hashtable);
				UserSettingsManager.SaveLocalSettingsTable("UserSettingsManagerLocalPublicUserSettings", hashtable["publicSettings"] as Hashtable);
			}
		}

		public static T Get<T>() where T : IPersistableState
		{
			return UserSettingsManager.instance.GetSettings<T>();
		}

		public static T GetFriend<T>(CloudUser user) where T : IPersistableState
		{
			return UserSettingsManager.instance.GetFriendSettings<T>(user);
		}

		public T GetSettings<T>() where T : IPersistableState
		{
			SettingsProviderAttribute settingsProviderAttribute;
			if (!this.availableSettingsProviders.TryGetValue(typeof(T), out settingsProviderAttribute))
			{
				return default(T);
			}
			if (settingsProviderAttribute.IsPublic)
			{
				return (T)((object)this.localPublicUserSettings[settingsProviderAttribute.Name]);
			}
			return (T)((object)this.localPrivateUserSettings[settingsProviderAttribute.Name]);
		}

		public T GetFriendSettings<T>(CloudUser user)
		{
			SettingsProviderAttribute settingsProviderAttribute;
			if (this.availableSettingsProviders.TryGetValue(typeof(T), out settingsProviderAttribute))
			{
				if (settingsProviderAttribute == null)
				{
					return default(T);
				}
				if (settingsProviderAttribute.IsPublic)
				{
					CloudUserSettings cloudSettingsForCloudUser = this.cloudClient.GetCloudSettingsForCloudUser(user);
					if (cloudSettingsForCloudUser == null)
					{
						return default(T);
					}
					Hashtable table = cloudSettingsForCloudUser.Public[settingsProviderAttribute.Name] as Hashtable;
					return JsonSerializer.HashtableToObject<T>(table);
				}
			}
			return default(T);
		}

		public void ResetSettings<T>() where T : IPersistableState
		{
			SettingsProviderAttribute settingsProviderAttribute;
			if (this.availableSettingsProviders.TryGetValue(typeof(T), out settingsProviderAttribute) && settingsProviderAttribute != null)
			{
				if (settingsProviderAttribute.IsPublic)
				{
					this.localPublicUserSettings[settingsProviderAttribute.Name] = (Activator.CreateInstance(typeof(T), true) as IPersistableState);
				}
				else
				{
					this.localPrivateUserSettings[settingsProviderAttribute.Name] = (Activator.CreateInstance(typeof(T), true) as IPersistableState);
				}
			}
		}

		public void ClearHasLoadedSettingsThisSession()
		{
			this.hasLoadedSettingsThisSession = false;
		}

		public void SyncUserSettings()
		{
			FiberCtrl.Pool.Run(this.SyncUserSettingsCr(), false);
		}

		public IEnumerator SyncUserSettingsCr()
		{
			if (this.syncInProgress)
			{
				while (this.syncInProgress)
				{
					yield return null;
				}
			}
			else
			{
				this.syncInProgress = true;
				IEnumerator e = this.SyncUserSettingsInternalCr();
				while (e.MoveNext())
				{
					object obj = e.Current;
					yield return obj;
				}
				this.syncInProgress = false;
			}
			yield break;
		}

		private IEnumerator SyncUserSettingsInternalCr()
		{
			object error = null;
			if (!this.cloudClient.HasValidUser)
			{
				yield break;
			}
			yield return null;
			if (this.CloudSettings == null || !this.LocalSettingsModified)
			{
				if (!this.hasLoadedSettingsThisSession || this.CloudSettings == null)
				{
					IEnumerator e = this.cloudClient.GetUserSettings(delegate(object err, CloudUserSettings settings)
					{
						error = err;
						if (settings != null)
						{
							this.MergeNewCloudSettings(settings);
						}
					});
					while (e.MoveNext())
					{
						object obj = e.Current;
						yield return obj;
					}
					if (UserSettingsManager.IsError(error))
					{
						yield break;
					}
					this.hasLoadedSettingsThisSession = true;
				}
				if (this.CloudSettings == null)
				{
					Hashtable privateTable = this.GetLocalSettingsAsTable(this.localPrivateUserSettings);
					Hashtable publicTable = this.GetLocalSettingsAsTable(this.localPublicUserSettings);
					IEnumerator e2 = this.cloudClient.CreateUserSettings(privateTable, publicTable, delegate(object err, CloudUserSettings settings)
					{
						error = err;
						this.CloudSettings = settings;
					});
					while (e2.MoveNext())
					{
						object obj2 = e2.Current;
						yield return obj2;
					}
					UserSettingsManager.IsError(error);
					yield break;
				}
			}
			if (this.LocalSettingsModified)
			{
				bool localSettingsSent = false;
				error = null;
				while (!localSettingsSent && error == null)
				{
					Hashtable valuesToSet;
					Hashtable valuesToUnSet;
					this.ConstructPatchPaths(out valuesToSet, out valuesToUnSet);
					IEnumerator e3 = this.cloudClient.PatchUserSettings(valuesToSet, valuesToUnSet, this.CloudSettings.Version, delegate(object err, CloudUserSettings settings, bool updated)
					{
						error = err;
						if (!updated && settings != null)
						{
							this.MergeNewCloudSettings(settings);
						}
						else if (error == null)
						{
							if (settings == null)
							{
							}
							this.CloudSettings = settings;
						}
						localSettingsSent = updated;
					});
					while (e3.MoveNext())
					{
						object obj3 = e3.Current;
						yield return obj3;
					}
				}
				if (UserSettingsManager.IsError(error))
				{
					yield break;
				}
			}
			this.SettingsSynced(this);
			yield break;
		}

		private void ConstructPatchPaths(out Hashtable valuesToSet, out Hashtable valuesToUnset)
		{
			Hashtable localSettingsAsTable = this.GetLocalSettingsAsTable(this.localPrivateUserSettings);
			Hashtable localSettingsAsTable2 = this.GetLocalSettingsAsTable(this.localPublicUserSettings);
			valuesToSet = new Hashtable();
			valuesToUnset = new Hashtable();
			HashtableDifference.CollectDiffPaths(this.CloudSettings.Private, localSettingsAsTable, "privateSettings.", valuesToSet, valuesToUnset);
			HashtableDifference.CollectDiffPaths(this.CloudSettings.Public, localSettingsAsTable2, "publicSettings.", valuesToSet, valuesToUnset);
		}

		public static int GetDataVersionFromSettingsTable(Hashtable table)
		{
			if (table != null && table.ContainsKey("_settingsVersion"))
			{
				return int.Parse(table["_settingsVersion"].ToString());
			}
			return 0;
		}

		private void SetDataVersionOnSettingsTable(Hashtable table)
		{
			UserSettingsManager.SetDataVersionOnSettingsTable(table, this.currentDataVersion);
		}

		public static void SetDataVersionOnSettingsTable(Hashtable table, int version)
		{
			if (table != null)
			{
				table["_settingsVersion"] = version;
			}
		}

		private void MergeNewCloudSettings(CloudUserSettings settingsFromCloud)
		{
			CloudUserSettings cloudUserSettings = UserSettingsManager.DeepCopyCloudUserSettings(settingsFromCloud);
			this.versionConverter(cloudUserSettings.Private, cloudUserSettings.Public);
			List<Type> list = new List<Type>();
			int num = 0;
			int num2;
			do
			{
				num++;
				num2 = 0;
				foreach (KeyValuePair<Type, SettingsProviderAttribute> keyValuePair in this.availableSettingsProviders)
				{
					if (!list.Contains(keyValuePair.Key))
					{
						bool flag = false;
						foreach (Type item in keyValuePair.Value.MergeDependencies)
						{
							if (!list.Contains(item))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Type key = keyValuePair.Key;
							string name = keyValuePair.Value.Name;
							IPersistableState persistableState = null;
							IPersistableState obj;
							IPersistableState persistableState2;
							if (keyValuePair.Value.IsPublic)
							{
								obj = this.localPublicUserSettings[keyValuePair.Value.Name];
								persistableState2 = (IPersistableState)JsonSerializer.HashtableToObject(key, cloudUserSettings.Public[name] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
								if (this.CloudSettings != null)
								{
									persistableState = (IPersistableState)JsonSerializer.HashtableToObject(key, this.CloudSettings.Public[name] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
								}
							}
							else
							{
								obj = this.localPrivateUserSettings[keyValuePair.Value.Name];
								persistableState2 = (IPersistableState)JsonSerializer.HashtableToObject(key, cloudUserSettings.Private[name] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
								if (this.CloudSettings != null)
								{
									persistableState = (IPersistableState)JsonSerializer.HashtableToObject(key, this.CloudSettings.Private[name] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
								}
							}
							if (persistableState2 != null)
							{
								MethodInfo method = key.GetMethod("MergeFromOther");
								try
								{
									method.Invoke(obj, new IPersistableState[]
									{
										persistableState2,
										persistableState
									});
								}
								catch (Exception ex)
								{
									throw ex;
								}
							}
							list.Add(keyValuePair.Key);
							num2++;
						}
					}
				}
				if (this.availableSettingsProviders.Count == list.Count)
				{
					break;
				}
			}
			while (num2 != 0);
			this.SetDataVersionOnSettingsTable(cloudUserSettings.Private);
			this.SetDataVersionOnSettingsTable(cloudUserSettings.Public);
			this.CloudSettings = settingsFromCloud;
			this.SaveLocalSettings();
		}

		private Hashtable GetLocalSettingsAsTable(Dictionary<string, IPersistableState> settings)
		{
			Hashtable hashtable = new Hashtable();
			this.SetDataVersionOnSettingsTable(hashtable);
			foreach (KeyValuePair<string, IPersistableState> keyValuePair in settings)
			{
				Hashtable value = JsonSerializer.ObjectToHashtable(keyValuePair.Value);
				hashtable[keyValuePair.Key] = value;
			}
			return hashtable;
		}

		private void LoadCachedCloudUserSettings()
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("UserSettingsManagerCachedCloudUserSettings", string.Empty);
			if (securedString.Length > 0)
			{
				Hashtable table = securedString.hashtableFromJson();
				this.cloudSettings = JsonSerializer.HashtableToObject<CloudUserSettings>(table);
			}
			else
			{
				this.cloudSettings = null;
			}
		}

		private void SaveCachedCloudUserSettings()
		{
			string value = (this.cloudSettings == null) ? string.Empty : JsonSerializer.ObjectToHashtable(this.cloudSettings).toJson();
			TactilePlayerPrefs.SetSecuredString("UserSettingsManagerCachedCloudUserSettings", value);
		}

		private Type FindSettingsTypeFromKey(string key)
		{
			foreach (KeyValuePair<Type, SettingsProviderAttribute> keyValuePair in this.availableSettingsProviders)
			{
				if (keyValuePair.Value.Name == key)
				{
					return keyValuePair.Key;
				}
			}
			return null;
		}

		private void LoadLocalSettings()
		{
			Hashtable hashtable = UserSettingsManager.LoadLocalHashtable("UserSettingsManagerLocalPrivateUserSettings");
			Hashtable hashtable2 = UserSettingsManager.LoadLocalHashtable("UserSettingsManagerLocalPublicUserSettings");
			if (hashtable == null && hashtable2 == null)
			{
				this.localPrivateUserSettings = this.ReflectHashtableToObjects(null, false);
				this.localPublicUserSettings = this.ReflectHashtableToObjects(null, true);
				this.defaultSettingsHandler(this);
				return;
			}
			this.versionConverter(hashtable, hashtable2);
			this.localPrivateUserSettings = this.ReflectHashtableToObjects(hashtable, false);
			this.localPublicUserSettings = this.ReflectHashtableToObjects(hashtable2, true);
		}

		public void UserSettingsToHashTable(out Hashtable privateSettings, out Hashtable publicSettings)
		{
			privateSettings = this.GetLocalSettingsAsTable(this.localPrivateUserSettings);
			publicSettings = this.GetLocalSettingsAsTable(this.localPublicUserSettings);
		}

		public bool Restore(string userSettingsJson)
		{
			Hashtable hashtable = userSettingsJson.hashtableFromJson();
			return this.Restore(hashtable["publicSettings"] as Hashtable, hashtable["privateSettings"] as Hashtable);
		}

		public bool Restore(Hashtable publicSettings, Hashtable privateSettings)
		{
			bool flag = false;
			flag |= !this.RestoreInternal(publicSettings, true);
			flag |= !this.RestoreInternal(privateSettings, false);
			if (!flag)
			{
				this.SaveLocalSettings();
			}
			return !flag;
		}

		private bool RestoreInternal(Hashtable settingsTable, bool isPublic)
		{
			Dictionary<string, IPersistableState> dictionary = new Dictionary<string, IPersistableState>();
			bool flag = true;
			try
			{
				foreach (KeyValuePair<Type, SettingsProviderAttribute> keyValuePair in this.availableSettingsProviders)
				{
					if (keyValuePair.Value.IsPublic == isPublic)
					{
						string name = keyValuePair.Value.Name;
						Type type = this.FindSettingsTypeFromKey(keyValuePair.Value.Name);
						IPersistableState value;
						if (settingsTable.ContainsKey(name))
						{
							value = (IPersistableState)JsonSerializer.HashtableToObject(type, settingsTable[name] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
						}
						else
						{
							value = (IPersistableState)Activator.CreateInstance(type, true);
						}
						dictionary.Add(name, value);
					}
				}
			}
			catch (Exception)
			{
				flag = false;
			}
			if (flag)
			{
				if (isPublic)
				{
					this.localPublicUserSettings = dictionary;
				}
				else
				{
					this.localPrivateUserSettings = dictionary;
				}
			}
			return flag;
		}

		private Dictionary<string, IPersistableState> ReflectHashtableToObjects(Hashtable table, bool publicSettings)
		{
			if (table == null)
			{
				table = new Hashtable();
			}
			Dictionary<string, IPersistableState> dictionary = new Dictionary<string, IPersistableState>();
			foreach (KeyValuePair<Type, SettingsProviderAttribute> keyValuePair in this.availableSettingsProviders)
			{
				if (keyValuePair.Value.IsPublic == publicSettings)
				{
					string name = keyValuePair.Value.Name;
					Type type = this.FindSettingsTypeFromKey(keyValuePair.Value.Name);
					IPersistableState value;
					if (table.ContainsKey(name))
					{
						value = (IPersistableState)JsonSerializer.HashtableToObject(type, table[name] as Hashtable, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
					}
					else
					{
						value = (IPersistableState)Activator.CreateInstance(type, true);
					}
					dictionary.Add(name, value);
				}
			}
			return dictionary;
		}

		public T GetSettingsFromRawData<T>(string userSettingsJson) where T : IPersistableState
		{
			Hashtable hashtable = userSettingsJson.hashtableFromJson();
			return this.GetSettingsFromRawData<T>(hashtable["publicSettings"] as Hashtable, hashtable["privateSettings"] as Hashtable);
		}

		public T GetSettingsFromRawData<T>(Hashtable publicSettings, Hashtable privateSettings) where T : IPersistableState
		{
			Dictionary<string, IPersistableState> dictionary = this.ReflectHashtableToObjects(publicSettings, true);
			Dictionary<string, IPersistableState> dictionary2 = this.ReflectHashtableToObjects(privateSettings, false);
			SettingsProviderAttribute settingsProviderAttribute = this.availableSettingsProviders[typeof(T)];
			Dictionary<string, IPersistableState> dictionary3 = (!settingsProviderAttribute.IsPublic) ? dictionary2 : dictionary;
			IPersistableState persistableState = dictionary3[settingsProviderAttribute.Name];
			return (T)((object)persistableState);
		}

		public void SaveLocalSettings()
		{
			Hashtable localSettingsAsTable = this.GetLocalSettingsAsTable(this.localPrivateUserSettings);
			UserSettingsManager.SaveLocalSettingsTable("UserSettingsManagerLocalPrivateUserSettings", localSettingsAsTable);
			Hashtable localSettingsAsTable2 = this.GetLocalSettingsAsTable(this.localPublicUserSettings);
			UserSettingsManager.SaveLocalSettingsTable("UserSettingsManagerLocalPublicUserSettings", localSettingsAsTable2);
			this.SettingsSaved();
		}

		private static Hashtable LoadLocalHashtable(string key)
		{
			string securedString = TactilePlayerPrefs.GetSecuredString(key, string.Empty);
			return (Hashtable)MiniJSON.jsonDecode(securedString);
		}

		private static void SaveLocalSettingsTable(string key, Hashtable value)
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString(key, value.toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString(key, string.Empty);
			}
		}

		private static bool IsError(object error)
		{
			return error != null;
		}

		private static CloudUserSettings DeepCopyCloudUserSettings(CloudUserSettings cloudUserSettings)
		{
			string json = JsonSerializer.ObjectToHashtable(cloudUserSettings).toJson();
			return JsonSerializer.HashtableToObject<CloudUserSettings>(json.hashtableFromJson());
		}

		private const string PREFS_CACHED_CLOUD_USER_SETTINGS = "UserSettingsManagerCachedCloudUserSettings";

		private const string PREFS_LOCAL_PUBLIC_USER_SETTINGS = "UserSettingsManagerLocalPublicUserSettings";

		private const string PREFS_LOCAL_PRIVATE_USER_SETTINGS = "UserSettingsManagerLocalPrivateUserSettings";

		private const string DATA_VERSION_KEY = "_settingsVersion";

		private static UserSettingsManager instance;

		private readonly int currentDataVersion;

		private readonly Dictionary<Type, SettingsProviderAttribute> availableSettingsProviders;

		private readonly ICloudUserSettingsProvider cloudClient;

		private readonly Action<UserSettingsManager> defaultSettingsHandler = delegate(UserSettingsManager A_0)
		{
		};

		private readonly Action<Hashtable, Hashtable> versionConverter = delegate(Hashtable A_0, Hashtable A_1)
		{
		};

		private Dictionary<string, IPersistableState> localPrivateUserSettings;

		private Dictionary<string, IPersistableState> localPublicUserSettings;

		private CloudUserSettings cloudSettings;

		private bool hasLoadedSettingsThisSession;

		private bool syncInProgress;
	}
}
