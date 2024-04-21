using System;
using System.Collections;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.MapSystem
{
	public class MainMapUI
	{
		public MainMapUI(IUIController uiController, IAssetModel assets, IUserSettings userSettings, FlowStack flowStack)
		{
			this.uiController = uiController;
			this.assets = assets;
			this.userSettings = userSettings;
			this.flowStack = flowStack;
		}

		public MainMapButtonsView ButtonsView
		{
			get
			{
				return this.buttonsView;
			}
		}

		public void ScreenAcquired()
		{
			if (this.buttonsView == null)
			{
				this.buttonsView = this.uiController.ShowView<MainMapButtonsView>(this.assets.GetStoryMapButtonsView());
			}
		}

		public void ScreenLost()
		{
			if (this.buttonsView != null)
			{
				this.buttonsView.Close(0);
				this.buttonsView = null;
			}
		}

		public IEnumerator HideUI()
		{
			this.buttonsView.GetButtonAreaRoot().SetActive(false);
			yield break;
		}

		public IEnumerator ShowUI()
		{
			this.buttonsView.GetButtonAreaRoot().SetActive(true);
			yield break;
		}

		private void OnEscapeKeyDown()
		{
			this.flowStack.Push(new QuitGameFromMapController(this.userSettings, this.uiController, this.assets));
		}

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private readonly IUserSettings userSettings;

		private readonly FlowStack flowStack;

		private MainMapButtonsView buttonsView;
	}
}
