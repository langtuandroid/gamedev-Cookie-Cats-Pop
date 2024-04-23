using System;
using UnityEngine;

namespace TactileModules.PuzzleGame.ThemeHunt
{
	public abstract class MapThemeHuntItem : MonoBehaviour
	{
		protected void OnClicked(UIEvent e)
		{
			if (ThemeHuntManagerBase.Instance.TryCollectThemeItem(this.id))
			{
				this.OnCollected();
			}
		}

		protected abstract void OnCollected();

		public int id;
	}
}
