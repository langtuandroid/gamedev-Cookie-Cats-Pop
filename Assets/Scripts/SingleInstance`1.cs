using System;

public class SingleInstance<T> : SingleInstance where T : SingleInstance<T>
{
	protected SingleInstance()
	{
		if (SingleInstance<T>.instance != null)
		{
			throw new Exception("SingleInstance already initialized: type=" + base.GetType().ToString());
		}
		SingleInstance<T>.instance = (this as T);
	}

	protected static T instance;
}
