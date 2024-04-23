using System;
using UnityEngine;

namespace TactileModules.GameCore.MenuTutorial.Assets
{
	public class AssetModel : IAssetModel
	{
		public MenuTutorialDatabase GetMenuTutorialDatabase()
		{
			return Resources.Load<MenuTutorialDatabase>("MenuTutorial/MenuTutorialDatabase");
		}

		public MenuTutorialMessage GetMessage()
		{
			return Resources.Load<MenuTutorialMessage>("MenuTutorial/Message");
		}

		public MenuTutorialHighlight GetHighlight()
		{
			return Resources.Load<MenuTutorialHighlight>("MenuTutorial/Highlight");
		}
	}
}
