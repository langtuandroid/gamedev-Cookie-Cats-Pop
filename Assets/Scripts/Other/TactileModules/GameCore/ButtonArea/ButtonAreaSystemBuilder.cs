using System;
using TactileModules.GameCore.ButtonArea.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.ButtonArea
{
	public static class ButtonAreaSystemBuilder
	{
		public static IButtonAreaSystem Build(IUIController iuiController)
		{
			IButtonAreaModel model = new ButtonAreaModel();
			IAssetModel assets = new AssetModel();
			IButtonAreaController controller = new ButtonAreaController(model, iuiController, assets);
			return new ButtonAreaSystem(model, controller);
		}
	}
}
