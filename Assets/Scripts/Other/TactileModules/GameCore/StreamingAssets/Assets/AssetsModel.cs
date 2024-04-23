using System;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets.Assets
{
	public class AssetsModel : IAssetsModel
	{
		public StreamingAssetsDependencies StreamingAssetsDependencies
		{
			get
			{
				return Resources.Load<StreamingAssetsDependencies>("StreamingAssets/StreamingAssetsDependencies");
			}
		}

		public LowresAssetGeneratorSettings LowresAssetGeneratorSettings
		{
			get
			{
				return Resources.Load<LowresAssetGeneratorSettings>("StreamingAssets/LowresAssetGeneratorSettings");
			}
		}
	}
}
