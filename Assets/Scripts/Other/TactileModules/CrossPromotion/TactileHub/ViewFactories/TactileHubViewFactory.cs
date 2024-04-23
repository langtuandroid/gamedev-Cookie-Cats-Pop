using System;
using TactileModules.CrossPromotion.TactileHub.Views;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.ViewFactories
{
	public class TactileHubViewFactory : ITactileHubViewFactory
	{
		public ITactileHubView CreateHubView()
		{
			string path = "CrossPromotion/TactileHub/Prefabs/TactileHubView";
			TactileHubView original = Resources.Load<TactileHubView>(path);
			return UnityEngine.Object.Instantiate<TactileHubView>(original);
		}
	}
}
