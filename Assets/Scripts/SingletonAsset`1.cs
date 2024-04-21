using System;
using UnityEngine;

public abstract class SingletonAsset<T> : ScriptableObject where T : ScriptableObject
{
	internal static T Instance
	{
		get
		{
			if (!Application.isPlaying)
			{
				throw new Exception("Using SingletonAsset<>.Instance in editor stop-mode is not allowed. Use 'Editor_Asset' instead.");
			}
			if (object.ReferenceEquals(null, SingletonAsset<T>.instance))
			{
				string path = SingletonAsset<T>.AssetPathToResourcePath(SingletonAsset<T>.GetPath<T>());
				SingletonAsset<T>.instance = Resources.Load<T>(path);
			}
			return SingletonAsset<T>.instance;
		}
	}

	private static string GetPath<Q>()
	{
		Type typeFromHandle = typeof(Q);
		SingletonAssetPath[] array = (SingletonAssetPath[])typeFromHandle.GetCustomAttributes(typeof(SingletonAssetPath), false);
		if (array.Length == 0)
		{
			return null;
		}
		return array[0].path;
	}

	public static string AssetPathToResourcePath(string assetPath)
	{
		int num = assetPath.IndexOf("Resources/") + 10;
		int num2 = assetPath.LastIndexOf(".");
		return assetPath.Substring(num, num2 - num);
	}

	protected static T instance;
}
