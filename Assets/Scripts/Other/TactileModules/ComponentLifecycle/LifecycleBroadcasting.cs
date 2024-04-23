using System;
using System.Diagnostics;
using UnityEngine;

namespace TactileModules.ComponentLifecycle
{
	public static class LifecycleBroadcasting
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GameObject> GameObjectAwakened;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GameObject> GameObjectStarted;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GameObject> GameObjectWillBeDestroyed;

		public static void NotifyGameObjectAwakened(GameObject gameObject)
		{
			LifecycleBroadcasting.GameObjectAwakened(gameObject);
		}

		public static void NotifyGameObjectStarted(GameObject gameObject)
		{
			LifecycleBroadcasting.GameObjectStarted(gameObject);
		}

		public static void NotifyGameObjectWillBeDestroyed(GameObject gameObject)
		{
			LifecycleBroadcasting.GameObjectWillBeDestroyed(gameObject);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static LifecycleBroadcasting()
		{
			LifecycleBroadcasting.GameObjectAwakened = delegate(GameObject A_0)
			{
			};
			LifecycleBroadcasting.GameObjectStarted = delegate(GameObject A_0)
			{
			};
			LifecycleBroadcasting.GameObjectWillBeDestroyed = delegate(GameObject A_0)
			{
			};
		}
	}
}
