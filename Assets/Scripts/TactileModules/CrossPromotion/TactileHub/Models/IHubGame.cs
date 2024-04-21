using System;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.Models
{
	public interface IHubGame
	{
		string GetGameTitle();

		void SendToStoreOrLaunchGame();

		Texture2D GetIconTexture();
	}
}
