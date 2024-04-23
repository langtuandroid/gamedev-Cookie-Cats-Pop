using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersMapButtonView : ExtensibleView<IMapButtonView>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ExitClicked;



		public SlidesAndLaddersWheelWidget SlidesAndLaddersWheelWidget
		{
			get
			{
				return this.wheelInstantiator.GetInstance().GetComponent<SlidesAndLaddersWheelWidget>();
			}
		}

		private SlidesAndLaddersHandler Handler
		{
			get
			{
				return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<SlidesAndLaddersHandler>();
			}
		}

		protected override void ViewWillAppear()
		{
			if (base.Extension != null)
			{
				base.Extension.ObtainOverlay(this);
			}
		}

		protected override void ViewDidDisappear()
		{
			if (base.Extension != null)
			{
				MaskOverlay.Instance.Alpha = 0.5f;
				base.Extension.ReleaseOverlay();
			}
		}

		protected override void ViewGotFocus()
		{
			if (base.Extension != null)
			{
				base.Extension.SetupOnLifeButtonClicked();
			}
		}

		protected override void ViewLostFocus()
		{
			if (base.Extension != null)
			{
				base.Extension.ReleaseOnLifeButtonClicked();
			}
		}

		[UsedImplicitly]
		private void ExitButtonClicked(UIEvent e)
		{
			if (!this.Handler.InstanceCustomData.HasShownTutorial)
			{
				return;
			}
			this.ExitClicked();
		}

		[SerializeField]
		private UIInstantiator wheelInstantiator;
	}
}
