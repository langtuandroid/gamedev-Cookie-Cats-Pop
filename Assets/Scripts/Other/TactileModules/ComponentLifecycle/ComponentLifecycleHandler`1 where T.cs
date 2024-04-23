using System;
using UnityEngine;

namespace TactileModules.ComponentLifecycle
{
	public abstract class ComponentLifecycleHandler<T> where T : MonoBehaviour
	{
		protected ComponentLifecycleHandler(ComponentLifecycleHandler<T>.InitializationTiming timing = ComponentLifecycleHandler<T>.InitializationTiming.Awake)
		{
			if (timing == ComponentLifecycleHandler<T>.InitializationTiming.Awake)
			{
				LifecycleBroadcasting.GameObjectAwakened += this.InitializeGameObject;
			}
			else
			{
				LifecycleBroadcasting.GameObjectStarted += this.InitializeGameObject;
			}
			LifecycleBroadcasting.GameObjectWillBeDestroyed += this.TeardownGameObject;
		}

		private void InitializeGameObject(GameObject gameObject)
		{
			T[] components = gameObject.GetComponents<T>();
			foreach (T component in components)
			{
				this.InitializeComponent(component);
			}
		}

		private void TeardownGameObject(GameObject gameObject)
		{
			T[] components = gameObject.GetComponents<T>();
			foreach (T component in components)
			{
				this.TeardownComponent(component);
			}
		}

		protected abstract void InitializeComponent(T component);

		protected virtual void TeardownComponent(T component)
		{
		}

		protected enum InitializationTiming
		{
			Awake,
			Start
		}
	}
}
