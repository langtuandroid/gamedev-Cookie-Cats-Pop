using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story.Views
{
	public class BarsView : UIView
	{
		protected override void ViewLoad(object[] parameters)
		{
			this.topBar = new BlackBar(this.topBarWidget, true);
			this.bottomBar = new BlackBar(this.bottomBarWidget, false);
			this.barsVisible = false;
		}

		public IEnumerator Show()
		{
			if (this.barsVisible)
			{
				yield break;
			}
			this.barsVisible = true;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.topBar.SlideIn(),
				this.bottomBar.SlideIn()
			});
			yield break;
		}

		public IEnumerator Hide()
		{
			if (!this.barsVisible)
			{
				yield break;
			}
			this.barsVisible = false;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.topBar.SlideOut(),
				this.bottomBar.SlideOut()
			});
			yield break;
		}

		[SerializeField]
		private UIWidget topBarWidget;

		[SerializeField]
		private UIWidget bottomBarWidget;

		private BlackBar topBar;

		private BlackBar bottomBar;

		private bool barsVisible;
	}
}
