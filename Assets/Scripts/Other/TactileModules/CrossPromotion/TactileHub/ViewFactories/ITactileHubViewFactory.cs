using System;
using TactileModules.CrossPromotion.TactileHub.Views;

namespace TactileModules.CrossPromotion.TactileHub.ViewFactories
{
	public interface ITactileHubViewFactory
	{
		ITactileHubView CreateHubView();
	}
}
