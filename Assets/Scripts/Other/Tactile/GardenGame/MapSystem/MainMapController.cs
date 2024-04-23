using System;
using System.Diagnostics;
using Tactile.GardenGame.MapSystem.Prop;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MainMapController
	{
		public MainMapController(IUIController uiController, PropsManager propsManager, IAssetModel assets, FlowStack flowStack, IUserSettings userSettings)
		{
			MainMapController _0024this = this;
			this.uiController = uiController;
			this.assets = assets;
			this.UI = new MainMapUI(uiController, assets, userSettings, flowStack);
			Camera initializeCamera = new GameObject().AddComponent<Camera>();
			initializeCamera.enabled = false;
			this.Map = new Map(propsManager, () => (!(_0024this.UI.ButtonsView != null)) ? initializeCamera : _0024this.UI.ButtonsView.UICamera, assets);
			UnityEngine.Object.Destroy(initializeCamera.gameObject);
			this.Map.Visible = false;
		}

		public Map Map { get; private set; }

		public MainMapUI UI { get; private set; }

		public ChoosePropController ChoosePropController { get; private set; }

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float> Stepped;

		public void Destroy()
		{
			this.Map.Destroy();
		}

		public void ScreenAcquired()
		{
			this.ChoosePropController = new ChoosePropController(this.uiController, this.Map.ClickableManager, this.UI, this.Map.Camera, this.Map.PropsManager, this.assets);
			this.UI.ScreenAcquired();
			this.Map.Visible = true;
		}

		public void ScreenLost()
		{
			this.ChoosePropController.Destroy();
			this.UI.ScreenLost();
			this.Map.Visible = false;
		}

		public void Step(float dt)
		{
			if (this.Stepped != null)
			{
				this.Stepped(dt);
			}
		}

		private readonly IUIController uiController;

		private readonly IAssetModel assets;
	}
}
