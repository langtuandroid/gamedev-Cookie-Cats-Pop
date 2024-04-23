using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.ScheduledBooster.Data;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using TactileModules.PuzzleGame.ScheduledBooster.Views;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.UI
{
	[RequireComponent(typeof(UIElement))]
	public class ScheduledBoosterTogglePurchase : MonoBehaviour
	{
		[Instantiator.SerializeLocalizableProperty]
		public Instantiator ParentInstantiator
		{
			get
			{
				return this.parentInstantiator;
			}
			set
			{
				this.parentInstantiator = value;
				if (Application.isPlaying)
				{
					this.InstantiatedAwake();
				}
			}
		}

		private UIView GetParentView()
		{
			if (this.cachedParentView == null)
			{
				this.cachedParentView = this.FindParentView();
			}
			return this.cachedParentView;
		}

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
				return this.ScheduledBoosters.GetBoosterForLocation(ScheduledBoosterLocation.PreGame);
			}
		}

		private void InstantiatedAwake()
		{
			this.Booster.Deactivate();
			base.transform.parent = this.parentInstantiator.GetInstance().transform;
			this.button.receiver = base.gameObject;
			this.button.methodName = "ButtonClicked";
			this.boosterIcon.SpriteName = this.Booster.Definition.icon;
			this.boosterPrice = this.Booster.Price;
			this.UpdateUI();
			UISafeTimer uisafeTimer = new UISafeTimer(base.gameObject, new Action(this.UpdateTimerLabel), 1f);
			uisafeTimer.Run();
		}

		private UIView FindParentView()
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				UIView component = parent.GetComponent<UIView>();
				if (component != null)
				{
					return component;
				}
				parent = parent.parent;
			}
			return null;
		}

		private void Start()
		{
			UIView uiview = this.FindParentView();
			ILevelStartView levelStartView = uiview as ILevelStartView;
			if (levelStartView != null)
			{
				levelStartView.DismissButtonClicked += this.OnViewDismissed;
				levelStartView.PlayButtonClicked += this.OnViewClickedPlay;
			}
		}

		private void OnViewClickedPlay(List<SelectedBooster> selectedBoosters)
		{
			if (!this.Booster.IsFree() && this.Booster.IsActive)
			{
				string analyticsString = "LimitedAvailabilityBooster_" + this.Booster.Type;
				this.ScheduledBoosters.InventoryProvider.ConsumeCoins(this.boosterPrice, analyticsString);
			}
		}

		private void OnViewDismissed()
		{
			if (!this.Booster.IsFree() && this.Booster.IsActive)
			{
				FiberCtrl.Pool.Run(this.ViewProvider.AnimateCoinsBack(this.GetParentView(), this.button.gameObject.transform.position), false);
				this.ScheduledBoosters.InventoryProvider.UnreserveCoins(this.boosterPrice);
				this.Booster.Deactivate();
			}
		}

		private void UpdateTimerLabel()
		{
			this.timeLabel.text = this.ScheduledBoosters.GetBooster(this.Booster.Type).GetTimeRemainingAsFormattedString();
		}

		private void ButtonClicked(UIEvent e)
		{
			if (!this.Booster.IsActive && this.Booster.IsFree())
			{
				this.animFiber.Start(this.ShowInfoView());
			}
			else if (this.animFiber.IsTerminated)
			{
				this.animFiber.Start(this.ToggleActivatedBoosterIfPossible());
			}
		}

		private IEnumerator ShowInfoView()
		{
			UIViewManager.UIViewStateGeneric<ScheduledBoosterInfoView> vs = ManagerRepository.Get<UIViewManager>().ShowView<ScheduledBoosterInfoView>(new object[]
			{
				this.Booster.Type
			});
			yield return vs.WaitForClose();
			if ((int)vs.ClosingResult == 1)
			{
				this.Booster.Activate();
			}
			this.UpdateUI();
			yield break;
		}

		private IEnumerator ToggleActivatedBoosterIfPossible()
		{
			UICamera.DisableInput();
			yield return new Fiber.OnExit(delegate()
			{
				UICamera.EnableInput();
			});
			if (this.Booster.IsFree())
			{
				if (this.Booster.IsActive)
				{
					this.Booster.Deactivate();
				}
				else
				{
					this.Booster.Activate();
				}
				this.UpdateUI();
				yield break;
			}
			if (!this.Booster.IsActive)
			{
				if (this.ScheduledBoosters.CanBuyBooster(this.Booster.Type))
				{
					this.ScheduledBoosters.InventoryProvider.ReserveCoins(this.boosterPrice);
					this.Booster.Activate();
					yield return this.ViewProvider.AnimateCoins(this.GetParentView(), this.button.gameObject.transform.position);
					this.UpdateUI();
				}
				else
				{
					yield return this.ViewProvider.ShowShopView();
				}
			}
			else
			{
				this.ScheduledBoosters.InventoryProvider.UnreserveCoins(this.boosterPrice);
				this.Booster.Deactivate();
				this.UpdateUI();
				yield return this.ViewProvider.AnimateCoinsBack(this.GetParentView(), this.button.gameObject.transform.position);
			}
			yield break;
		}

		private void UpdateUI()
		{
			this.checkMark.SetActive(this.Booster.IsActive);
			if (this.Booster.IsFree())
			{
				this.pricePivot.SetActive(false);
				this.freeLabelPivot.SetActive(true);
			}
			else
			{
				this.priceLabel.text = this.Booster.Price.ToString() + " [C]";
				this.pricePivot.SetActive(!this.Booster.IsActive);
			}
		}

		private void OnDestroy()
		{
			this.animFiber.Terminate();
		}

		[SerializeField]
		private UIButton button;

		[SerializeField]
		private UISprite boosterIcon;

		[SerializeField]
		private GameObject checkMark;

		[SerializeField]
		private GameObject pricePivot;

		[SerializeField]
		private UILabel priceLabel;

		[SerializeField]
		private GameObject freeLabelPivot;

		[SerializeField]
		private UILabel timeLabel;

		private readonly Fiber animFiber = new Fiber();

		private Instantiator parentInstantiator;

		private UIView cachedParentView;

		private int boosterPrice;

		private string boosterType;
	}
}
