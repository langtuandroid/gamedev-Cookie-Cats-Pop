using System;
using TactileModules.CrossPromotion.TactileHub.Data;

namespace TactileModules.CrossPromotion.TactileHub.Models
{
	public interface IHubGameFactory
	{
		IHubGame Create(HubGameData hubGameData, string iconPath);
	}
}
