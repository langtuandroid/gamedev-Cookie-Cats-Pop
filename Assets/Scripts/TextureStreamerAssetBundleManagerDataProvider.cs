using System;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.TactileCloud.AssetBundles;

public class TextureStreamerAssetBundleManagerDataProvider : TextureStreamerAssetBundleManager.IDataProvider
{
	public AssetBundleManager GetAssetBundleManager()
	{
		return ManagerRepository.Get<AssetBundleSystem>().AssetBundleManager;
	}

	public int FarthestCompletedLevelHumanNumber
	{
		get
		{
			return MainProgressionManager.Instance.GetFarthestCompletedLevelHumanNumber();
		}
	}

	public bool PendingDoUpdateAssetBundles { get; set; }
}
