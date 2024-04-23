using System;
using System.Collections.Generic;
using TactileModules.CrossPromotion.TactileHub.Models;

namespace TactileModules.CrossPromotion.TactileHub.Views
{
	public interface ITactileHubView : IUIView
	{
		void Initialize(List<IHubGame> hubGames, UIElement hubButtonUiElement, IUIViewManager uiViewManager);

		event Action<IHubGame> OnClick;
	}
}
