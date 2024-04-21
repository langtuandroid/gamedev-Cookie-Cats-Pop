using System;
using UnityEngine;

namespace TactileModules.ComponentLifecycle
{
	public class LifecycleBroadcaster : MonoBehaviour
	{
		protected virtual void Awake()
		{
			LifecycleBroadcasting.NotifyGameObjectAwakened(base.gameObject);
		}

		protected virtual void Start()
		{
			LifecycleBroadcasting.NotifyGameObjectStarted(base.gameObject);
		}

		protected virtual void OnDestroy()
		{
			LifecycleBroadcasting.NotifyGameObjectWillBeDestroyed(base.gameObject);
		}
	}
}
