using System;

namespace TactileModules.GameCore.MenuTutorial.Assets
{
	public interface IAssetModel
	{
		MenuTutorialDatabase GetMenuTutorialDatabase();

		MenuTutorialMessage GetMessage();

		MenuTutorialHighlight GetHighlight();
	}
}
