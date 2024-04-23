using System;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.TactileHub.LifecycleHandlers;
using TactileModules.CrossPromotion.TactileHub.ViewControllers;
using TactileModules.CrossPromotion.TactileHub.ViewFactories;

namespace TactileModules.CrossPromotion.TactileHub
{
	public static class TactileHubSystemBuilder
	{
		public static void Build(IGeneralDataRetriever generalDataRetriever, IUIViewManager uiViewManager)
		{
			TactileHubViewFactory viewFactory = new TactileHubViewFactory();
			MapButtonLifecycleHandlerButtonRetriever tactileHubMapButtonRetriever = new MapButtonLifecycleHandlerButtonRetriever();
			TactileHubViewControllerFactory viewControllerFactory = new TactileHubViewControllerFactory(generalDataRetriever, viewFactory, uiViewManager, uiViewManager, tactileHubMapButtonRetriever);
			new MapButtonLifecycleHandler(viewControllerFactory, generalDataRetriever);
		}
	}
}
