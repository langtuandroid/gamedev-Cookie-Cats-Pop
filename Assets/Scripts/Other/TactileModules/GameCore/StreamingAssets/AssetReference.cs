using System;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets
{
	[Serializable]
	public class AssetReference
	{
		public string Guid
		{
			get
			{
				return this.guid;
			}
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.guid);
			}
		}

		[SerializeField]
		private string guid;
	}
}
