using System;
using System.Collections.Generic;
using TactileModules.ComponentLifecycle;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.TactileHub.Models;
using TactileModules.CrossPromotion.TactileHub.ViewComponents;
using TactileModules.CrossPromotion.TactileHub.ViewControllers;

namespace TactileModules.CrossPromotion.TactileHub.LifecycleHandlers
{
	public class MapButtonLifecycleHandler : ComponentLifecycleHandler<TactileHubMapButton>
	{
		public MapButtonLifecycleHandler(TactileHubViewControllerFactory viewControllerFactory, IGeneralDataRetriever generalDataRetriever) : base(ComponentLifecycleHandler<TactileHubMapButton>.InitializationTiming.Awake)
		{
			this.viewControllerFactory = viewControllerFactory;
			this.generalDataRetriever = generalDataRetriever;
		}

		protected override void InitializeComponent(TactileHubMapButton tactileHubMapButton)
		{
			List<IHubGame> allCachedHubGames = this.generalDataRetriever.GetAllCachedHubGames();
			tactileHubMapButton.Initialize(allCachedHubGames);
			tactileHubMapButton.OnButtonClicked += this.OpenTactileHubView;
		}

		private void OpenTactileHubView()
		{
			TactileHubViewController tactileHubViewController = this.viewControllerFactory.CreateTactileHubViewController();
			tactileHubViewController.ShowTactileHubView();
		}

		private readonly TactileHubViewControllerFactory viewControllerFactory;

		private readonly IGeneralDataRetriever generalDataRetriever;
	}
}
