using System;
using Tactile;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Controllers
{
	public class SlidesAndLaddersWheelWidgetController
	{
		public SlidesAndLaddersWheelWidgetController(IDataProvider<SlidesAndLaddersInstanceCustomData> data, SlidesAndLaddersWheelWidget wheelWidget)
		{
			this.wheelModel = new SlidesAndLaddersWheelModel(SingletonAsset<SlidesAndLaddersSetup>.Instance.wheelSlots, data);
			this.wheelWidget = wheelWidget;
			this.data = data;
			wheelWidget.SetWheelState(this.wheelModel.CanSpinWheel());
			wheelWidget.OnAnimatingIsDone = (Action)Delegate.Combine(wheelWidget.OnAnimatingIsDone, new Action(this.HandleAnimatingIsDone));
			wheelWidget.OnWheelClicked = (Action)Delegate.Combine(wheelWidget.OnWheelClicked, new Action(this.HandleWheelClicked));
		}

		public Action<WheelSlot> OnAnimatingIsDone { get; set; }

		private void HandleWheelClicked()
		{
			if (!this.wheelModel.CanSpinWheel())
			{
				return;
			}
			UICamera.DisableInput();
			this.wheelSlot = this.wheelModel.Roll();
			Vector3 angleForWheelSlot = this.wheelModel.GetAngleForWheelSlot(this.wheelSlot);
			this.wheelWidget.AnimateWheelToAngle(angleForWheelSlot);
			this.data.Get().SpinCount++;
		}

		private void HandleAnimatingIsDone()
		{
			this.wheelWidget.SetWheelState(this.wheelModel.CanSpinWheel());
			UICamera.EnableInput();
			if (this.OnAnimatingIsDone != null)
			{
				this.OnAnimatingIsDone(this.wheelSlot);
			}
		}

		public void RefreshState()
		{
			this.wheelModel.Reset();
			this.wheelWidget.SetWheelState(this.wheelModel.CanSpinWheel());
		}

		public void Reset()
		{
			this.wheelModel.Reset();
			this.wheelWidget.SetWheelState(true);
		}

		public void PlayWheelAvailableEffect()
		{
			this.wheelWidget.WheelAvailableEffect();
		}

		private readonly SlidesAndLaddersWheelModel wheelModel;

		private readonly SlidesAndLaddersWheelWidget wheelWidget;

		private readonly IDataProvider<SlidesAndLaddersInstanceCustomData> data;

		private SlidesAndLaddersTutorialView tutorialView;

		private MaskOverlay maskOverlay;

		private WheelSlot wheelSlot;
	}
}
