using System;
using TactileModules.TactileCloud.AssetBundles;

public class LocalizationManager : SingleInstance<LocalizationManager>, AssetBundleManager.IManagedAssetBundleHandler
{
	public LocalizationManager(AssetBundleManager assetBundleManager)
	{
		this._assetBundleManager = assetBundleManager;
		this._assetBundleManager.RegisterManagedAssetBundleHandler(LocalizationManager.LOCALIZATIONS_BUNDLE_ID, this);
	}

	public void OnStateChanged(ManagedAssetbundle managedAssetbundle, ManagedAssetbundle.State newState)
	{
		if (newState == ManagedAssetbundle.State.AWAITING_CONSUMPTION)
		{
			L.UpdateLocalizationsWithAssetBundle(managedAssetbundle.AssetBundle);
			managedAssetbundle.ConsumeChanges(true);
		}
	}

	public static string LOCALIZATIONS_BUNDLE_ID = "LocalizationsBundle";

	private AssetBundleManager _assetBundleManager;
}
