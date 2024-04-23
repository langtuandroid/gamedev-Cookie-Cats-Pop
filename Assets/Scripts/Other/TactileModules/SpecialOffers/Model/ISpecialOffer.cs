using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.SpecialOffers.Model
{
	public interface ISpecialOffer
	{
		string FeatureInstanceId { get; }

		bool IsActivated();

		bool CanActivate(int farthestUnlockedLevelHumanNumber);

		int GetLastShowingTimeStamp();

		void SetLastShowingTimeStamp();

		void Activate();

		void Deactivate();

		List<ItemAmount> GetReward();

		string GetIAPIdentifier();

		Texture2D LoadTexture();

		Texture2D LoadSideMapButtonTexture();

		string GetTimeLeft();

		int GetSecondsLeft();

		string GetPriceNow();

		string GetPriceBefore();

		SpecialOfferTypeEnum GetType();

		int GetCoinPrice();

		string GetAnalyticsId();

		int GetLevelRequirement();

		bool IsValid();

		string GetValidationInfo();
	}
}
