using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story.Views
{
	public class BlackBarWidget : MonoBehaviour
	{
		private void Awake()
		{
			this.blackBar = new BlackBar(this.barWidget, this.isTop);
			if (this.defaultStateIn)
			{
				this.blackBar.SnapIn();
			}
			else
			{
				this.blackBar.SnapOut();
			}
		}

		public IEnumerator SlideIn()
		{
			yield return this.blackBar.SlideIn();
			yield break;
		}

		public IEnumerator SlideOut()
		{
			yield return this.blackBar.SlideOut();
			yield break;
		}

		public UIWidget barWidget;

		public bool isTop;

		public bool defaultStateIn;

		private BlackBar blackBar;
	}
}
