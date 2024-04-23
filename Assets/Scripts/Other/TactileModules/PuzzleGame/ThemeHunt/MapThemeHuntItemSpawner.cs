using System;
using UnityEngine;

namespace TactileModules.PuzzleGame.ThemeHunt
{
	public class MapThemeHuntItemSpawner : MonoBehaviour
	{
		[Instantiator.SerializeProperty]
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
				if (Application.isPlaying)
				{
					this.InitSpawning();
				}
			}
		}

		private void InitSpawning()
		{
			this.mapThemeItem.SetActive(false);
			ThemeHuntManagerBase.Instance.OnItemSpawn += this.SpawnMapThemeHuntItem;
			ThemeHuntManagerBase.Instance.OnHuntEnded += this.RemoveMapThemeHuntItem;
			this.SpawnMapThemeHuntItem();
		}

		private void OnDisable()
		{
			ThemeHuntManagerBase.Instance.OnItemSpawn -= this.SpawnMapThemeHuntItem;
			ThemeHuntManagerBase.Instance.OnHuntEnded -= this.RemoveMapThemeHuntItem;
			this.RemoveMapThemeHuntItem();
		}

		private void SpawnMapThemeHuntItem()
		{
			if (!ThemeHuntManagerBase.Instance.IsHuntActiveOnClient())
			{
				return;
			}
			if (ThemeHuntManagerBase.Instance.GetActiveThemeItems.Contains(this.Id))
			{
				this.mapThemeItem.SetActive(true);
				this.mapThemeItem.GetComponent<MapThemeHuntItem>().id = this.Id;
			}
		}

		private void RemoveMapThemeHuntItem()
		{
			if (this.mapThemeItem != null)
			{
				this.mapThemeItem.SetActive(false);
			}
		}

		[SerializeField]
		private GameObject mapThemeItem;

		private int id;
	}
}
