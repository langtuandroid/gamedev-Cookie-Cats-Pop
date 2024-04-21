using System;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.TactileHub.LifecycleHandlers;
using TactileModules.CrossPromotion.TactileHub.ViewFactories;

namespace TactileModules.CrossPromotion.TactileHub.ViewControllers
{
	public class TactileHubViewControllerFactory
	{
		public TactileHubViewControllerFactory(IGeneralDataRetriever dataRetriever, ITactileHubViewFactory viewFactory, IViewPresenter uiViewPresenter, IUIViewManager uiViewManager, ITactileHubMapButtonRetriever tactileHubMapButtonRetriever)
		{
			this.dataRetriever = dataRetriever;
			this.viewFactory = viewFactory;
			this.uiViewPresenter = uiViewPresenter;
			this.uiViewManager = uiViewManager;
			this.tactileHubMapButtonRetriever = tactileHubMapButtonRetriever;
		}

		public TactileHubViewController CreateTactileHubViewController()
		{
			return new TactileHubViewController(this.dataRetriever, this.viewFactory, this.uiViewPresenter, this.tactileHubMapButtonRetriever, this.uiViewManager);
		}

		private readonly IGeneralDataRetriever dataRetriever;

		private readonly ITactileHubViewFactory viewFactory;

		private readonly IViewPresenter uiViewPresenter;

		private readonly IUIViewManager uiViewManager;

		private readonly ITactileHubMapButtonRetriever tactileHubMapButtonRetriever;
	}
}
