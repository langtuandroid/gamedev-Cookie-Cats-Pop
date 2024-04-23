using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Tactile;

namespace TactileModules.PuzzleGame.PlayablePostcard.Views
{
	public class PlayablePostcardMapButtonView : ExtensibleView<IPostcardMapButtonView>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnExitButtonClicked;



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
			this.OnExitButtonClicked();
		}
	}
}
