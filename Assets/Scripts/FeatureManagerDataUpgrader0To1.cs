using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Prime31;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGames.StarTournament;

public static class FeatureManagerDataUpgrader0To1
{
	public static void UpgradeFeatureManagerPersistableState0To1(Hashtable root)
	{
		Hashtable hashtable = new Hashtable();
		root.Add("fmps", hashtable);
		int num = 1;
		hashtable.Add("sv", num);
		Hashtable hashtable2 = new Hashtable();
		hashtable.Add("fid", hashtable2);
		FeatureManagerDataUpgrader0To1.AddFeatureTypeData(root, hashtable2, "lr", "level-rush");
		FeatureManagerDataUpgrader0To1.AddFeatureTypeData(root, hashtable2, "trh", "treasure-hunt");
		FeatureManagerDataUpgrader0To1.AddFeatureTypeData(root, hashtable2, "srt", "star-tournament");
		FeatureManagerDataUpgrader0To1.AddFeatureTypeData(root, hashtable2, "olc", "one-life-challenge");
		ArrayList value = new ArrayList();
		hashtable.Add("df", value);
		ArrayList value2 = new ArrayList();
		hashtable.Add("dfp", value2);
		hashtable.Add("st", 0);
	}

	private static void AddFeatureTypeData(Hashtable root, Hashtable featureTypeDatas, string old_featureKey, string featureType)
	{
		if (root.ContainsKey(old_featureKey))
		{
			Hashtable hashtable = new Hashtable();
			featureTypeDatas.Add(featureType, hashtable);
			Hashtable customTypeData = FeatureManagerDataUpgrader0To1.GetCustomTypeData((Hashtable)root[old_featureKey]);
			hashtable.Add("fhpsb", customTypeData);
			string assemblyQualifiedName = FeatureManagerDataUpgrader0To1.GetCustomTypeDataType(featureType).AssemblyQualifiedName;
			hashtable.Add("cdt", assemblyQualifiedName);
			ArrayList arrayList = new ArrayList();
			Hashtable activatedFeatureInstanceData = FeatureManagerDataUpgrader0To1.GetActivatedFeatureInstanceData(root, old_featureKey);
			if (activatedFeatureInstanceData != null)
			{
				arrayList.Add(activatedFeatureInstanceData);
			}
			hashtable.Add("afid", arrayList);
		}
	}

	private static Hashtable GetInstanceCustomData(Hashtable old_featureRoot)
	{
		string text = (string)((Hashtable)old_featureRoot["rf"])["n"];
		Hashtable hashtable = new Hashtable();
		if (text == "level-rush")
		{
			int num = (int)((double)old_featureRoot["sl"]);
			hashtable.Add("sl", num);
			int num2 = (int)((double)old_featureRoot["rl"]);
			hashtable.Add("rl", num2);
			int num3 = (int)((double)old_featureRoot["lrc"]);
			hashtable.Add("lrc", num3);
		}
		else if (text == "treasure-hunt")
		{
			int num4 = (int)((double)old_featureRoot["progress"]);
			hashtable.Add("progress", num4);
			bool flag = (bool)old_featureRoot["c"];
			hashtable.Add("c", flag);
		}
		else if (text == "star-tournament")
		{
			string value = (string)old_featureRoot["jid"];
			hashtable.Add("jid", value);
		}
		else
		{
			if (!(text == "one-life-challenge"))
			{
				throw new NotImplementedException("FeatureType = " + text);
			}
			int num5 = (int)((double)old_featureRoot["rv"]);
			hashtable.Add("rv", num5);
			int num6 = (int)((double)old_featureRoot["pr"]);
			hashtable.Add("pr", num6);
			bool flag2 = (bool)old_featureRoot["c"];
			hashtable.Add("c", flag2);
		}
		hashtable.Add("sv", 1);
		return hashtable;
	}

	private static Hashtable GetCustomTypeData(Hashtable old_featureRoot)
	{
		Hashtable hashtable = new Hashtable();
		if (!old_featureRoot.ContainsKey("rf"))
		{
			return hashtable;
		}
		string text = (string)((Hashtable)old_featureRoot["rf"])["n"];
		if (string.IsNullOrEmpty(text))
		{
			return hashtable;
		}
		if (!(text == "level-rush"))
		{
			if (!(text == "treasure-hunt"))
			{
				if (text == "star-tournament")
				{
					Hashtable value = new Hashtable((Hashtable)old_featureRoot["ost"]);
					hashtable.Add("ost", value);
				}
				else if (!(text == "one-life-challenge"))
				{
					throw new NotImplementedException("FeatureType = " + text);
				}
			}
		}
		hashtable.Add("sv", 1);
		return hashtable;
	}

	private static Type GetCustomTypeDataType(string featureType)
	{
		if (featureType == "level-rush")
		{
			return typeof(FeatureTypeCustomData);
		}
		if (featureType == "treasure-hunt")
		{
			return typeof(FeatureTypeCustomData);
		}
		if (featureType == "star-tournament")
		{
			return typeof(StarTournamentTypeCustomData);
		}
		if (featureType == "one-life-challenge")
		{
			return typeof(FeatureTypeCustomData);
		}
		throw new NotImplementedException("FeatureType = " + featureType);
	}

	private static Type GetCustomInstanceDataType(Hashtable old_featureRoot)
	{
		string text = (string)((Hashtable)old_featureRoot["rf"])["n"];
		if (text == "star-tournament")
		{
			return typeof(StarTournamentInstanceCustomData);
		}
		if (text == "one-life-challenge")
		{
			return typeof(OneLifeChallengeInstanceCustomData);
		}
		throw new NotImplementedException("FeatureType = " + text);
	}

	private static Hashtable GetActivatedFeatureInstanceData(Hashtable root, string old_featureKey)
	{
		Hashtable hashtable = (Hashtable)root[old_featureKey];
		Hashtable hashtable2 = null;
		if (hashtable.ContainsKey("rf"))
		{
			hashtable2 = (Hashtable)hashtable["rf"];
		}
		if (hashtable2 != null && !string.IsNullOrEmpty((string)hashtable2["n"]))
		{
			Hashtable hashtable3 = new Hashtable();
			Hashtable instanceCustomData = FeatureManagerDataUpgrader0To1.GetInstanceCustomData(hashtable);
			hashtable3.Add("ficd", instanceCustomData);
			string assemblyQualifiedName = FeatureManagerDataUpgrader0To1.GetCustomInstanceDataType(hashtable).AssemblyQualifiedName;
			hashtable3.Add("cdt", assemblyQualifiedName);
			Hashtable activationDataV0To = FeatureManagerDataUpgrader0To1.GetActivationDataV0To1(hashtable);
			hashtable3.Add("fiad", activationDataV0To);
			return hashtable3;
		}
		return null;
	}

	private static Hashtable GetActivationDataV0To1(Hashtable old_featureCustomData)
	{
		Hashtable hashtable = new Hashtable();
		Hashtable featureDataV0To = FeatureManagerDataUpgrader0To1.GetFeatureDataV0To1((Hashtable)old_featureCustomData["rf"]);
		hashtable.Add("afd", featureDataV0To);
		int num = (int)FeatureManagerDataUpgrader0To1.DeserializeDateTime((string)old_featureCustomData["csd"]).toEpochTime();
		hashtable.Add("ast", num);
		return hashtable;
	}

	private static Hashtable GetFeatureDataV0To1(Hashtable old_featureScheduleInfo)
	{
		Hashtable hashtable = new Hashtable();
		string value = (string)old_featureScheduleInfo["id"];
		hashtable.Add("id", value);
		string value2 = (string)old_featureScheduleInfo["n"];
		hashtable.Add("type", value2);
		long num = Convert.ToInt64(FeatureManagerDataUpgrader0To1.DeserializeDateTime((string)old_featureScheduleInfo["sd"]).ToUtcEpoch());
		hashtable.Add("startAt", num);
		long num2 = Convert.ToInt64(FeatureManagerDataUpgrader0To1.DeserializeDateTime((string)old_featureScheduleInfo["ed"]).ToUtcEpoch());
		hashtable.Add("endAt", num2);
		int num3 = (int)((double)old_featureScheduleInfo["mptd"]);
		hashtable.Add("maxDuration", num3);
		long num4 = Convert.ToInt64(FeatureManagerDataUpgrader0To1.DeserializeDateTime((string)old_featureScheduleInfo["jed"]).ToUtcEpoch());
		int num5 = (int)(num2 - num4);
		hashtable.Add("minDuration", num5);
		Hashtable value3 = (Hashtable)old_featureScheduleInfo["m"];
		hashtable.Add("meta", value3);
		return hashtable;
	}

	private static DateTime DeserializeDateTime(string str)
	{
		DateTime result;
		if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
		{
			return result;
		}
		throw new InvalidEnumArgumentException("string not deserializable to string: " + str);
	}
}
