using System;
using System.Collections;
using Fibers;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class QuitGameFromMapController : IFlow, IFiberRunnable
	{
		public QuitGameFromMapController(IUserSettings userSettings, IUIController uiController, IAssetModel assets)
		{
			this.userSettings = userSettings;
			this.uiController = uiController;
			this.assets = assets;
		}

		public IEnumerator Run()
		{
			QuitGameFromMapView view = this.uiController.ShowView<QuitGameFromMapView>(this.assets.GetQuitGameFromMapView());
			view.OnQuitGame += this.QuitGame;
			while (!view.Dismissed)
			{
				yield return null;
			}
			view.OnQuitGame -= this.QuitGame;
			view.Close(0);
			yield break;
		}

		private void QuitGame()
		{
			this.userSettings.SaveLocalSettings();
			Application.Quit();
		}

		public void OnExit()
		{
		}

		private readonly IUserSettings userSettings;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;
	}
}
