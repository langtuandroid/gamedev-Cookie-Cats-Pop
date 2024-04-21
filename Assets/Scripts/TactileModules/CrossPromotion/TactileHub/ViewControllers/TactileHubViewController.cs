using System;
using System.Collections.Generic;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.TactileHub.LifecycleHandlers;
using TactileModules.CrossPromotion.TactileHub.Models;
using TactileModules.CrossPromotion.TactileHub.ViewFactories;
using TactileModules.CrossPromotion.TactileHub.Views;

namespace TactileModules.CrossPromotion.TactileHub.ViewControllers
{
	public class TactileHubViewController
	{
		public TactileHubViewController(IGeneralDataRetriever generalDataRetriever, ITactileHubViewFactory viewFactory, IViewPresenter viewPresenter, ITactileHubMapButtonRetriever tactileHubMapButtonRetriever, IUIViewManager uiViewManager)
		{
			this.generalDataRetriever = generalDataRetriever;
			this.viewFactory = viewFactory;
			this.viewPresenter = viewPresenter;
			this.tactileHubMapButtonRetriever = tactileHubMapButtonRetriever;
			this.uiViewManager = uiViewManager;
		}

		public void ShowTactileHubView()
		{
			List<IHubGame> allCachedHubGames = this.generalDataRetriever.GetAllCachedHubGames();
			ITactileHubView tactileHubView = this.viewFactory.CreateHubView();
			tactileHubView.Initialize(allCachedHubGames, this.tactileHubMapButtonRetriever.GetHubButtonUiElement(), this.uiViewManager);
			tactileHubView.OnClick += this.ClickedOnGame;
			this.viewPresenter.ShowViewInstance<ITactileHubView>(tactileHubView, new object[0]);
		}

		private void ClickedOnGame(IHubGame hubGame)
		{
			hubGame.SendToStoreOrLaunchGame();
		}

		private readonly IGeneralDataRetriever generalDataRetriever;

		private readonly IViewPresenter viewPresenter;

		private readonly ITactileHubMapButtonRetriever tactileHubMapButtonRetriever;

		private readonly IUIViewManager uiViewManager;

		private readonly ITactileHubViewFactory viewFactory;
	}
}
