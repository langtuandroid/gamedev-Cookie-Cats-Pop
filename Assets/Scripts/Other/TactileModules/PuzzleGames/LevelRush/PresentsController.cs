using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class PresentsController : LevelRushPresent.ILabelDataProvider
	{
		public PresentsController(MapContentController mapContent, IMainProgression mainProgression, ILevelRushActivation levelRushActivation, ILevelRushProgression progression, IMainMapPlugin mainMapPlugin, IAssetModel assetModel)
		{
			this.mapContent = mapContent;
			this.mainProgression = mainProgression;
			this.levelRushActivation = levelRushActivation;
			this.progression = progression;
			this.mainMapPlugin = mainMapPlugin;
			this.assetModel = assetModel;
			this.RebuildPresents();
		}

		public void DestroyObjects()
		{
			this.DestroyPresents();
		}

		public IEnumerator DropPresents()
		{
			yield return new Fiber.OnExit(delegate()
			{
				UICamera.EnableInput();
			});
			this.RebuildPresents();
			UICamera.DisableInput();
			yield return this.DropPresentsOnMap(this.presents);
			yield break;
		}

		private void DestroyPresents()
		{
			foreach (LevelRushPresent levelRushPresent in this.presents)
			{
				if (levelRushPresent != null)
				{
					UnityEngine.Object.Destroy(levelRushPresent.gameObject);
				}
			}
			this.presents.Clear();
		}

		public void RebuildPresents()
		{
			this.DestroyPresents();
			this.presents = this.SpawnVisualPresentObjects();
			foreach (LevelRushPresent levelRushPresent in this.presents)
			{
				GameObject gameObject = levelRushPresent.gameObject;
				levelRushPresent.Clicked += this.PresentClicked;
				gameObject.transform.parent = this.mapContent.ContentRoot;
				gameObject.SetLayerRecursively(this.mapContent.ContentRoot.gameObject.layer);
				int levelIndexFromRewardIndex = this.progression.GetLevelIndexFromRewardIndex(levelRushPresent.RewardIndex);
				Vector3 a;
				if (!this.mapContent.TryGetDotPosition(levelIndexFromRewardIndex, out a))
				{
					throw new Exception("Failed to retrieve level dot position!");
				}
				gameObject.transform.localPosition = a + Vector3.back * 45f;
			}
		}

		private void PresentClicked(LevelRushPresent present)
		{
			if (!this.progression.IsRewardBeyondFarthestUnlockedLevel(present.RewardIndex))
			{
				FiberCtrl.Pool.Run(new ClaimRewardFlow(this.levelRushActivation, this.progression, this.mainMapPlugin, this.assetModel), false);
			}
			else
			{
				UIViewManager.IUIViewStateGeneric<LevelRushPresentInfoView> iuiviewStateGeneric = UIViewManager.Instance.ShowViewFromPrefab<LevelRushPresentInfoView>(this.assetModel.LevelRushPresentInfoView, new object[0]);
				iuiviewStateGeneric.View.Initialize(this.progression.GetReward(present.RewardIndex), present.RewardIndex);
			}
		}

		private AnimationCurve GetMapPanCurve()
		{
			return AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}

		private IEnumerator DropPresentsOnMap(List<LevelRushPresent> presents)
		{
			Vector3 dropOffset = Vector3.up * 500f;
			foreach (LevelRushPresent levelRushPresent in presents)
			{
				levelRushPresent.gameObject.SetActive(false);
			}
			foreach (LevelRushPresent present in presents)
			{
				int lastLevelIndex = this.progression.GetLevelIndexFromRewardIndex(present.RewardIndex);
				yield return this.mapContent.PanToDot(lastLevelIndex, 0.5f, this.GetMapPanCurve());
				yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
				present.gameObject.SetActive(true);
				yield return present.AnimateDrop(dropOffset);
				yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
			}
			yield return this.mapContent.PanToDot(this.mainProgression.GetFarthestUnlockedLevelIndex(), 1.5f, this.GetMapPanCurve());
			yield break;
		}

		private List<LevelRushPresent> SpawnVisualPresentObjects()
		{
			List<LevelRushPresent> list = new List<LevelRushPresent>();
			foreach (LevelRushConfig.Reward reward in this.progression.GetUnclaimedRewards())
			{
				LevelRushPresent levelRushPresent = this.SpawnPresent();
				levelRushPresent.Initialize(this, this.progression.GetRewardIndex(reward));
				list.Add(levelRushPresent);
			}
			return list;
		}

		private LevelRushPresent SpawnPresent()
		{
			return UnityEngine.Object.Instantiate<LevelRushPresent>(this.assetModel.LevelRushPresent);
		}

		int LevelRushPresent.ILabelDataProvider.GetSecondsLeft()
		{
			return this.levelRushActivation.GetSecondsLeft();
		}

		bool LevelRushPresent.ILabelDataProvider.IsRewardOnNextLevelToComplete(int rewardIndex)
		{
			return this.progression.IsRewardOnNextLevelToComplete(rewardIndex);
		}

		private const string PRESENT_PREFAB_PATH = "LevelRush/LevelRushPresent";

		private readonly MapContentController mapContent;

		private readonly IMainProgression mainProgression;

		private readonly ILevelRushActivation levelRushActivation;

		private readonly ILevelRushProgression progression;

		private readonly IMainMapPlugin mainMapPlugin;

		private readonly IAssetModel assetModel;

		private List<LevelRushPresent> presents = new List<LevelRushPresent>();
	}
}
