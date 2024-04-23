using System;
using System.Reflection;

public static class ILevelCollectionExtensions
{
	public static void DeserializePolymorphicMetaData(this ILevelCollection collection)
	{
		Assembly assembly = typeof(LevelStub).Assembly;
		foreach (ILevelStubPrivateAccess levelStubPrivateAccess in collection.LevelStubs)
		{
			if (!string.IsNullOrEmpty(levelStubPrivateAccess.MetaClassType))
			{
				Type type = assembly.GetType(levelStubPrivateAccess.MetaClassType);
				if (type != null)
				{
					ILevelStubPrivateAccess levelStubPrivateAccess2 = levelStubPrivateAccess;
					levelStubPrivateAccess2.MetaData = (JsonSerializer.Decode(type, levelStubPrivateAccess.MetaDataAsJson) as LevelMetaData);
				}
			}
		}
	}

	public static LevelMetaData DeserializeSinglePolymorphicMetaData(this ILevelCollection collection, int index)
	{
		Assembly assembly = typeof(LevelStub).Assembly;
		ILevelStubPrivateAccess levelStubPrivateAccess = collection.LevelStubs[index];
		if (string.IsNullOrEmpty(levelStubPrivateAccess.MetaClassType))
		{
			return null;
		}
		Type type = assembly.GetType(levelStubPrivateAccess.MetaClassType);
		if (type != null)
		{
			return JsonSerializer.Decode(type, levelStubPrivateAccess.MetaDataAsJson) as LevelMetaData;
		}
		return null;
	}

	public static LevelMetaData GetLevelMetaData(this ILevelCollection collection, int index)
	{
		LevelStub levelStub = collection.LevelStubs[index];
		ILevelStubPrivateAccess levelStubPrivateAccess = levelStub;
		LevelMetaData result;
		if ((result = levelStubPrivateAccess.MetaData) == null)
		{
			LevelMetaData levelMetaData = collection.DeserializeSinglePolymorphicMetaData(index);
			levelStubPrivateAccess.MetaData = levelMetaData;
			result = levelMetaData;
		}
		return result;
	}
}
