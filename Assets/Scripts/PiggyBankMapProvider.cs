using System;
using Shared.PiggyBank.Module.Interfaces;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class PiggyBankMapProvider : IMapProvider
{
	public MapViewBase FindMapView()
	{
		return UIViewManager.Instance.FindView<MapView>();
	}

	public AnimationCurve CameraFocusCurve()
	{
		return SingletonAsset<CommonCurves>.Instance.easeInOut;
	}

	public AnimationCurve ItemDropCurve()
	{
		return SingletonAsset<CommonCurves>.Instance.itemDropCurve;
	}

	public AnimationCurve ItemDropScaleCurve()
	{
		return SingletonAsset<CommonCurves>.Instance.itemDropScaleCurve;
	}

	public GameObject GetLevelDotIndicatorPrefab()
	{
		if (this.levelDotIndicatorPrefab == null)
		{
			this.levelDotIndicatorPrefab = Resources.Load<GameObject>("PiggyBank/PiggyBankLevelDotIndicator");
		}
		return this.levelDotIndicatorPrefab;
	}

	public int GetDotIndexFromHumanNumber(int humanNumber)
	{
		MainLevelDatabase levelDatabase = ManagerRepository.Get<LevelDatabaseCollection>().GetLevelDatabase<MainLevelDatabase>("Main");
		return levelDatabase.NonGateIndexToDotIndex(levelDatabase.GetLevel(humanNumber - 1), null);
	}

	public void PlayDropGameObjectSound()
	{
	}

	private const string MAP_INDICATOR_PREFAB_PATH = "PiggyBank/PiggyBankLevelDotIndicator";

	private GameObject levelDotIndicatorPrefab;
}
