using System;
using System.Collections;
using Fibers;
using NinjaUI;
using Shared.PiggyBank.Module.Interfaces;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank
{
	public class PiggyBankMapPlugin : IMapPlugin
	{
		public PiggyBankMapPlugin(IPiggyBankProgression piggyBankProgression, MainProgressionManager mainProgressionManager, IMapProvider mapProvider)
		{
			this.piggyBankProgression = piggyBankProgression;
			this.mainProgressionManager = mainProgressionManager;
			this.mapProvider = mapProvider;
		}

		public void ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow)
		{
			if (mapId != "Main")
			{
				return;
			}
			this.mapContent = mapContent;
			if (this.piggyBankProgression.IsFeatureEnabled() && this.piggyBankProgression.TutorialShown)
			{
				this.SpawnLevelDotIndicator(this.piggyBankProgression.GetNextFreeOpenLevelHumanNumber());
			}
		}

		public void ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent)
		{
			if (mapId != "Main")
			{
				return;
			}
			this.mapContent = null;
		}

		public IEnumerator PanCameraAndDropLevelDotIndicator(int levelHumanNumber)
		{
			yield return new Fiber.OnExit(delegate()
			{
				UICamera.EnableInput();
			});
			UICamera.DisableInput();
			if (this.mainProgressionManager.GetFarthestUnlockedLevelIndex() > this.mainProgressionManager.MaxAvailableLevel)
			{
				yield break;
			}
			int nextFreeOpenDotIndex = this.GetDotIndexFromHumanNumber(levelHumanNumber);
			if (nextFreeOpenDotIndex > this.mainProgressionManager.MaxAvailableLevel)
			{
				if (this.levelDotIndicator != null)
				{
					this.levelDotIndicator.SetActive(false);
				}
				yield break;
			}
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			yield return this.mapContent.PanToDot(nextFreeOpenDotIndex, 3f, this.mapProvider.CameraFocusCurve());
			GameObject spawnedObject = this.SpawnLevelDotIndicator(levelHumanNumber);
			if (spawnedObject == null)
			{
				yield break;
			}
			spawnedObject.SetActive(false);
			int currentLevelIndex = this.mainProgressionManager.GetFarthestUnlockedLevelIndex();
			yield return this.DropGameObject(spawnedObject);
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
			yield return this.mapContent.PanToDot(currentLevelIndex, 2f, this.mapProvider.CameraFocusCurve());
			yield break;
		}

		public GameObject SpawnLevelDotIndicator(int levelHumanNumber)
		{
			int dotIndexFromHumanNumber = this.GetDotIndexFromHumanNumber(levelHumanNumber);
			if (dotIndexFromHumanNumber > this.mainProgressionManager.MaxAvailableLevel)
			{
				return null;
			}
			if (this.levelDotIndicator == null)
			{
				this.levelDotIndicator = UnityEngine.Object.Instantiate<GameObject>(this.mapProvider.GetLevelDotIndicatorPrefab());
			}
			this.levelDotIndicator.transform.parent = this.mapContent.ContentRoot;
			this.levelDotIndicator.SetLayerRecursively(this.mapContent.ContentRoot.gameObject.layer);
			Vector3 a;
			if (!this.mapContent.TryGetDotPosition(dotIndexFromHumanNumber, out a))
			{
				throw new Exception("Failed to retrieve level dot position!");
			}
			this.levelDotIndicator.transform.localPosition = a + Vector3.back * 25f;
			return this.levelDotIndicator;
		}

		private IEnumerator DropGameObject(GameObject objectToDrop)
		{
			objectToDrop.transform.position += Vector3.up * 500f;
			objectToDrop.SetActive(true);
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveLocalTransform(objectToDrop.transform, objectToDrop.transform.localPosition, objectToDrop.transform.localPosition + Vector3.down * 500f, this.mapProvider.ItemDropCurve(), 0.3f),
				FiberAnimation.ScaleTransform(objectToDrop.transform, Vector3.one, Vector3.one, this.mapProvider.ItemDropScaleCurve(), 0.3f)
			});
			this.mapProvider.PlayDropGameObjectSound();
			yield return CameraShaker.ShakeDecreasing(0.3f, 10f, 30f, 0f, false);
			yield break;
		}

		private int GetDotIndexFromHumanNumber(int humanNumber)
		{
			MainLevelDatabase database = this.mainProgressionManager.GetDatabase();
			return database.NonGateIndexToDotIndex(database.GetLevel(humanNumber - 1), null);
		}

		private MapContentController mapContent;

		private GameObject levelDotIndicator;

		private readonly IPiggyBankProgression piggyBankProgression;

		private readonly MainProgressionManager mainProgressionManager;

		private readonly IMapProvider mapProvider;
	}
}
