using System;
using System.Collections;

public static class FeatureManagerDataUpgrader
{
	public static FeatureManagerDataUpgrader.UpgradeResult UpgradeFeatureManager(Hashtable privateSettings, Func<Hashtable, bool> upgrader = null)
	{
		Hashtable from = new Hashtable(privateSettings);
		int i = FeatureManagerDataUpgrader.GetFeatureManagerPersistableStateSerializedVersion(privateSettings);
		int featureManagerPersistableStateSerializingVersion = FeatureManagerDataUpgrader.GetFeatureManagerPersistableStateSerializingVersion();
		while (i < featureManagerPersistableStateSerializingVersion)
		{
			int num = i;
			int toVersion = i + 1;
			if (!((upgrader != null) ? upgrader(privateSettings) : FeatureManagerDataUpgrader.UpgradeFeatureManagerPersistableState(privateSettings, i, toVersion)))
			{
				FeatureManagerDataUpgrader.CopyHashtable(from, privateSettings);
				return FeatureManagerDataUpgrader.UpgradeResult.UpgradeFailure;
			}
			i = FeatureManagerDataUpgrader.GetFeatureManagerPersistableStateSerializedVersion(privateSettings);
			if (num == i || i > featureManagerPersistableStateSerializingVersion || i < num)
			{
				FeatureManagerDataUpgrader.CopyHashtable(from, privateSettings);
				return FeatureManagerDataUpgrader.UpgradeResult.VersionFailure;
			}
		}
		return FeatureManagerDataUpgrader.UpgradeResult.Success;
	}

	private static bool UpgradeFeatureManagerPersistableState(Hashtable root, int fromVersion, int toVersion)
	{
		try
		{
			if (fromVersion != 0 || toVersion != 1)
			{
				throw new NotImplementedException();
			}
			FeatureManagerDataUpgrader0To1.UpgradeFeatureManagerPersistableState0To1(root);
		}
		catch (Exception e)
		{
			return false;
		}
		return true;
	}

	private static void CopyHashtable(Hashtable from, Hashtable to)
	{
		to.Clear();
		IDictionaryEnumerator enumerator = from.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				to[dictionaryEntry.Key] = dictionaryEntry.Value;
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

	public static int GetFeatureManagerPersistableStateSerializedVersion(Hashtable privateSettings)
	{
		if (privateSettings.ContainsKey("fmps"))
		{
			Hashtable hashtable = (Hashtable)privateSettings["fmps"];
			if (hashtable.ContainsKey("sv"))
			{
				object obj = hashtable["sv"];
				if (obj is double)
				{
					return (int)((double)obj);
				}
				if (obj is int)
				{
					return (int)obj;
				}
				throw new InvalidCastException("sv is not of type int, it is " + obj.GetType());
			}
		}
		return 0;
	}

	public static int GetFeatureManagerPersistableStateSerializingVersion()
	{
		return 1;
	}

	public enum UpgradeResult
	{
		None,
		Success,
		UpgradeFailure,
		VersionFailure
	}
}
