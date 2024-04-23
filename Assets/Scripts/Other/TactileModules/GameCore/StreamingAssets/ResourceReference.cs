using System;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets
{
	[Serializable]
	public class ResourceReference
	{
		public UnityEngine.Object Load()
		{
			return Resources.Load(this.path);
		}

		public T Load<T>() where T : UnityEngine.Object
		{
			return (T)((object)this.Load());
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.path);
			}
		}

		[SerializeField]
		private string path;
	}
}
