using System;

namespace TactileModules.GameCore.StreamingAssets.Assets
{
	public interface IAssetsModel
	{
		StreamingAssetsDependencies StreamingAssetsDependencies { get; }

		LowresAssetGeneratorSettings LowresAssetGeneratorSettings { get; }
	}
}
