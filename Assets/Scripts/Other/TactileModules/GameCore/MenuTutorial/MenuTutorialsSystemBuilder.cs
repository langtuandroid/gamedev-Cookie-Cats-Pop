using System;
using TactileModules.GameCore.MainProgression;
using TactileModules.GameCore.MenuTutorial.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.MenuTutorial
{
	public static class MenuTutorialsSystemBuilder
	{
		public static void Build(IUIController uiController, IMainProgressionModel mainProgression, Func<bool> enabledFunction)
		{
			IAssetModel assetModel = new AssetModel();
			MenuTutorialModel model = new MenuTutorialModel(assetModel);
			RunningTutorialFactory runningTutorialFactory = new RunningTutorialFactory(uiController, assetModel.GetMessage());
			MenuTutorialEngine menuTutorialEngine = new MenuTutorialEngine(model, assetModel.GetMenuTutorialDatabase(), mainProgression, runningTutorialFactory, enabledFunction);
		}
	}
}
