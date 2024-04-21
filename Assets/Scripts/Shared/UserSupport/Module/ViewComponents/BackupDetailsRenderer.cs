using System;
using TactileModules.UserSupport.Model;
using UnityEngine;

namespace Shared.UserSupport.Module.ViewComponents
{
	public class BackupDetailsRenderer : MonoBehaviour
	{
		public void Render(string title, BackupSummary summary)
		{
			if (summary == null)
			{
				return;
			}
			this.title.text = title;
			this.levelsTitle.text = L.Get("Progress");
			this.levelsValue.text = summary.MaxReachedLevel.ToString();
			this.coinsTitle.text = L.Get("Coins");
			this.coinsValue.text = summary.Coins.ToString();
		}

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UILabel levelsTitle;

		[SerializeField]
		private UILabel levelsValue;

		[SerializeField]
		private UILabel coinsTitle;

		[SerializeField]
		private UILabel coinsValue;
	}
}
