using System;
using UnityEngine;

namespace TactileModules.EditorTools
{
	public class AssetVersion : MonoBehaviour
	{
		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public string ExampleAssetGuid
		{
			get
			{
				return this.exampleAssetGuid;
			}
			set
			{
				this.exampleAssetGuid = value;
			}
		}

		[HideInInspector]
		[SerializeField]
		private int version;

		[HideInInspector]
		[SerializeField]
		private string exampleAssetGuid;
	}
}
