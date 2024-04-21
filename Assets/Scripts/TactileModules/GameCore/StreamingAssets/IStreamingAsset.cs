using System;

namespace TactileModules.GameCore.StreamingAssets
{
	public interface IStreamingAsset
	{
		AssetReference Source { get; }

		void InvokeAssetChanged();
	}
}
