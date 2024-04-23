using System;
using UnityEngine;

namespace TactileModules.GameCore.TemplateAssets
{
	public class AssetVersion : MonoBehaviour
	{
		public int Version
		{
			get
			{
				return this.version;
			}
		}

		[SerializeField]
		private int version;

		[SerializeField]
		private AssetGuidReference originAsset;
	}
}
