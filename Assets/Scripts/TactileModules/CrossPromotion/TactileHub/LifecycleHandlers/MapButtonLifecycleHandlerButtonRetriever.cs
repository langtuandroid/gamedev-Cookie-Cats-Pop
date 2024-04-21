using System;
using TactileModules.ComponentLifecycle;
using TactileModules.CrossPromotion.TactileHub.ViewComponents;

namespace TactileModules.CrossPromotion.TactileHub.LifecycleHandlers
{
	public class MapButtonLifecycleHandlerButtonRetriever : ComponentLifecycleHandler<TactileHubMapButton>, ITactileHubMapButtonRetriever
	{
		public MapButtonLifecycleHandlerButtonRetriever() : base(ComponentLifecycleHandler<TactileHubMapButton>.InitializationTiming.Awake)
		{
		}

		protected override void InitializeComponent(TactileHubMapButton tactileHubMapButton)
		{
			this.hubMapButton = tactileHubMapButton;
		}

		public UIElement GetHubButtonUiElement()
		{
			return this.hubMapButton.GetElement();
		}

		private TactileHubMapButton hubMapButton;
	}
}
