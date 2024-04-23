using System;
using System.Collections.Generic;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.General;
using TactileModules.CrossPromotion.TactileHub.Models;

namespace TactileModules.CrossPromotion.Cloud.DataRetrievers
{
	public interface IGeneralDataRetriever
	{
		CrossPromotionGeneralData GetGeneralData();

		List<IHubGame> GetAllCachedHubGames();

		string[] GetAllInstalledGames();

		int GetMaxAdsPerSession(AdType type);
	}
}
