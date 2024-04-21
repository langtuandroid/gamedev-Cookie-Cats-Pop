using System;

public abstract class Singleton<T> where T : new()
{
	public static T Instance { get; private set; }

	public static T CreateInstance()
	{
		Singleton<T>.Instance = Activator.CreateInstance<T>();
		return Singleton<T>.Instance;
	}
}
