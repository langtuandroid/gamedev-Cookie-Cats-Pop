using System;
using TactileModules.CrossPromotion.General.PromotedGameUtility;
using TactileModules.CrossPromotion.TactileHub.Data;
using TactileModules.NinjaUi.SharedViewControllers;
using TactileModules.RuntimeTools;

namespace TactileModules.CrossPromotion.TactileHub.Models
{
	public class HubGameFactory : IHubGameFactory
	{
		public HubGameFactory(IPromotedGameLauncher promotedGameLauncher, ITextureLoader textureLoader, ISpinnerViewController spinnerViewController)
		{
			this.promotedGameLauncher = promotedGameLauncher;
			this.textureLoader = textureLoader;
			this.spinnerViewController = spinnerViewController;
		}

		public IHubGame Create(HubGameData hubGameData, string iconPath)
		{
			return new HubGame(hubGameData, this.promotedGameLauncher, this.textureLoader, this.spinnerViewController, iconPath);
		}

		private readonly IPromotedGameLauncher promotedGameLauncher;

		private readonly ITextureLoader textureLoader;

		private readonly ISpinnerViewController spinnerViewController;
	}
}
