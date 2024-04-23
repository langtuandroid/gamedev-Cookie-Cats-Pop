using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using TactileModules.PuzzleGame.ScheduledBooster.Views;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.UI
{
	[RequireComponent(typeof(UIElement))]
	public class ScheduledBoosterButtonInstantPurchase : MonoBehaviour
	{
		[Instantiator.SerializeLocalizableProperty]
		public UIView ParentView { get; set; }

		private ScheduledBoosters ScheduledBoosters
		{
			get
			{
				return ManagerRepository.Get<ScheduledBoosterSystem>().ScheduledBoosters;
			}
		}

		private IScheduledBoosterViewProvider ViewProvider
		{
			get
			{
				return ManagerRepository.Get<ScheduledBoosterSystem>().ViewProvider;
			}
		}

		private IScheduledBooster Booster
		{
			get
			{
				return this.ScheduledBoosters.GetBoosterForLocation(this.location);
			}
		}

		private void Awake()
		{
			if (this.Booster == null)
			{
				return;
			}
			this.UpdateUI();
			if (this.Booster.Definition.boosterLargerIconPrefab != null)
			{
				GameObject prefab = this.CreateBoosterInfoPrefab();
				this.AssignPrefabToPivot(prefab);
			}
			UISafeTimer uisafeTimer = new UISafeTimer(base.gameObject, new Action(this.UpdateTimerLabel), 1f);
			uisafeTimer.Run();
		}

		private GameObject CreateBoosterInfoPrefab()
		{
			GameObject boosterLargerIconPrefab = this.Booster.Definition.boosterLargerIconPrefab;
			return UnityEngine.Object.Instantiate<GameObject>(boosterLargerIconPrefab);
		}

		private void AssignPrefabToPivot(GameObject prefab)
		{
			prefab.transform.parent = this.spawnIconPivot;
			prefab.transform.localPosition = Vector3.zero;
			prefab.SetLayerRecursively(base.gameObject.layer);
		}

		[UsedImplicitly]
		private void ButtonClicked(UIEvent e)
		{
			if (!this.Booster.IsActive && this.Booster.IsFree())
			{
				this.animFiber.Start(this.ShowInfoView());
			}
			else if (this.animFiber.IsTerminated)
			{
				this.animFiber.Start(this.BuyBooster());
			}
		}

		private IEnumerator ShowInfoView()
		{
			UIViewManager.UIViewStateGeneric<ScheduledBoosterInfoView> vs = UIViewManager.Instance.ShowView<ScheduledBoosterInfoView>(new object[]
			{
				this.Booster.Type
			});
			yield return vs.WaitForClose();
			if ((int)vs.ClosingResult == 1)
			{
				this.SetBoosterActive();
			}
			yield break;
		}

		private IEnumerator BuyBooster()
		{
			if (this.ScheduledBoosters.CanBuyBooster(this.Booster.Type))
			{
				string analyticsString = "LimitedAvailabilityBooster_" + this.Booster.Type;
				this.ScheduledBoosters.InventoryProvider.ConsumeCoins(this.Booster.Price, analyticsString);
				yield return this.ViewProvider.AnimateCoins(this.ParentView, this.button.gameObject.transform.position);
				this.SetBoosterActive();
			}
			else
			{
				yield return this.ViewProvider.ShowShopView();
			}
			yield break;
		}

		private void SetBoosterActive()
		{
			this.Booster.Activate();
			this.ParentView.Close(0);
		}

		private void UpdateUI()
		{
			if (this.Booster.IsFree())
			{
				this.pricePivot.SetActive(false);
				this.freeLabelPivot.SetActive(true);
			}
			else
			{
				this.priceLabel.text = this.Booster.Price.ToString() + " [C]";
				this.pricePivot.SetActive(!this.Booster.IsActive);
				this.freeLabelPivot.SetActive(false);
			}
		}

		private void UpdateTimerLabel()
		{
			this.timeLabel.text = this.ScheduledBoosters.GetBooster(this.Booster.Type).GetTimeRemainingAsFormattedString();
		}

		private void OnDestroy()
		{
			this.animFiber.Terminate();
		}

		[SerializeField]
		private Transform spawnIconPivot;

		[SerializeField]
		private UIButton button;

		[SerializeField]
		private UILabel timeLabel;

		[SerializeField]
		private GameObject pricePivot;

		[SerializeField]
		private UILabel priceLabel;

		[SerializeField]
		private GameObject freeLabelPivot;

		[SerializeField]
		private ScheduledBoosterLocation location;

		private readonly Fiber animFiber = new Fiber();

		private UIView parentView;
	}
}
