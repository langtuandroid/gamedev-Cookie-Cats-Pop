using System;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.UI;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public class RewardAreaController : IRewardAreaController
	{
		public RewardAreaController(IUIController uiController, IRewardAreaModel rewardAreaModel)
		{
			this.uiController = uiController;
			this.rewardAreaModel = rewardAreaModel;
			this.HookEvents();
		}

		private void HookEvents()
		{
			this.uiController.ViewCreated += this.OnViewCreated;
			this.uiController.ViewDestroyed += this.OnViewDestroyed;
		}

		private void OnViewCreated(IUIView view)
		{
			this.RegisterTargetsFromGameObject(view.gameObject);
		}

		private void OnViewDestroyed(IUIView view)
		{
			this.UnregisterTargetsFromGameObject(view.gameObject);
		}

		public void RegisterTargetsFromGameObject(GameObject go)
		{
			InventoryCollectTarget[] allInventoryCollectTargetFromGameObject = this.GetAllInventoryCollectTargetFromGameObject(go);
			for (int i = 0; i < allInventoryCollectTargetFromGameObject.Length; i++)
			{
				this.rewardAreaModel.RegisterTarget(allInventoryCollectTargetFromGameObject[i]);
			}
		}

		public void UnregisterTargetsFromGameObject(GameObject go)
		{
			InventoryCollectTarget[] allInventoryCollectTargetFromGameObject = this.GetAllInventoryCollectTargetFromGameObject(go);
			for (int i = 0; i < allInventoryCollectTargetFromGameObject.Length; i++)
			{
				this.rewardAreaModel.UnregisterTarget(allInventoryCollectTargetFromGameObject[i]);
			}
		}

		private InventoryCollectTarget[] GetAllInventoryCollectTargetFromGameObject(GameObject go)
		{
			return go.GetComponentsInChildren<InventoryCollectTarget>();
		}

		private readonly IUIController uiController;

		private readonly IRewardAreaModel rewardAreaModel;
	}
}
