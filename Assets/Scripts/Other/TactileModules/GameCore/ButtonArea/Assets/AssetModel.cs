using System;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea.Assets
{
	public class AssetModel : IAssetModel
	{
		public ButtonAreaSetup Setup
		{
			get
			{
				return Resources.Load<ButtonAreaSetup>("ButtonArea/Setup");
			}
		}
	}
}
