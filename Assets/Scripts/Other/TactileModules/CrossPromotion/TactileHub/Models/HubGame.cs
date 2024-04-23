using System;
using TactileModules.CrossPromotion.General.PromotedGameUtility;
using TactileModules.CrossPromotion.TactileHub.Data;
using TactileModules.NinjaUi.SharedViewControllers;
using TactileModules.RuntimeTools;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.Models
{
	public class HubGame : IHubGame
	{
		public HubGame(HubGameData hubGameData, IPromotedGameLauncher promotedGameLauncher, ITextureLoader textureLoader, ISpinnerViewController spinnerViewController, string hubIconPath)
		{
			this.hubGameData = hubGameData;
			this.promotedGameLauncher = promotedGameLauncher;
			this.textureLoader = textureLoader;
			this.spinnerViewController = spinnerViewController;
			this.hubIconPath = hubIconPath;
		}

		public string GetGameTitle()
		{
			return this.hubGameData.Name;
		}

		public void SendToStoreOrLaunchGame()
		{
			IUIView view = this.spinnerViewController.ShowSpinnerView(L.Get("Please wait"));
			this.promotedGameLauncher.SendToStoreOrLaunchGame(this.hubGameData, "TactileHub", null, delegate
			{
				this.spinnerViewController.CloseSpinnerView(view);
			});
		}

		public Texture2D GetIconTexture()
		{
			return this.textureLoader.LoadTexture(this.hubIconPath, false);
		}

		private readonly HubGameData hubGameData;

		private readonly string hubIconPath;

		private readonly IPromotedGameLauncher promotedGameLauncher;

		private readonly ITextureLoader textureLoader;

		private readonly ISpinnerViewController spinnerViewController;
	}
}
