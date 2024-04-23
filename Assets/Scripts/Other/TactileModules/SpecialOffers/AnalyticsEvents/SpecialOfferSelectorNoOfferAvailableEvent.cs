using System;
using System.Collections.Generic;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferSelectorNoOfferAvailable", true)]
	public class SpecialOfferSelectorNoOfferAvailableEvent : SpecialOffersBasicEvent
	{
		public SpecialOfferSelectorNoOfferAvailableEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, List<ISpecialOffer> allOffers, string noOfferAvailableReason, int serverTime) : base(specialOffersGlobalCoolDown)
		{
			this.NoOfferAvailableReason = noOfferAvailableReason;
			this.ServerTime = DateHelper.GetDateTimeFromUnixTimestamp((long)serverTime);
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			List<string> list5 = new List<string>();
			List<string> list6 = new List<string>();
			foreach (ISpecialOffer specialOffer in allOffers)
			{
				list.Add(specialOffer.GetAnalyticsId());
				list2.Add((!specialOffer.IsActivated()) ? "not-activated" : "activated");
				list3.Add(specialOffer.GetLevelRequirement().ToString());
				list4.Add(this.GetFormattedTimeStamp(specialOffer.GetLastShowingTimeStamp()));
				list5.Add((!specialOffer.IsValid()) ? specialOffer.GetValidationInfo() : string.Empty);
				list6.Add(specialOffer.FeatureInstanceId);
			}
			this.AllOffersNames = this.GetJoinedValues(list);
			this.AllOffersActivationState = this.GetJoinedValues(list2);
			this.AllOffersLevelRequirement = this.GetJoinedValues(list3);
			this.AllOffersLastShowingTimeStamp = this.GetJoinedValues(list4);
			this.AllOffersValidationInfo = this.GetJoinedValues(list5);
			this.AllOffersFeatureInstanceId = this.GetJoinedValues(list6);
		}

		private TactileAnalytics.RequiredParam<string> NoOfferAvailableReason { get; set; }

		private TactileAnalytics.RequiredParam<DateTime> ServerTime { get; set; }

		private TactileAnalytics.OptionalParam<string> AllOffersNames { get; set; }

		private TactileAnalytics.OptionalParam<string> AllOffersActivationState { get; set; }

		private TactileAnalytics.OptionalParam<string> AllOffersLevelRequirement { get; set; }

		private TactileAnalytics.OptionalParam<string> AllOffersLastShowingTimeStamp { get; set; }

		private TactileAnalytics.OptionalParam<string> AllOffersValidationInfo { get; set; }

		private TactileAnalytics.OptionalParam<string> AllOffersFeatureInstanceId { get; set; }

		private string GetJoinedValues(List<string> values)
		{
			if (values.Count > 0)
			{
				return string.Join(",", values.ToArray());
			}
			return null;
		}

		public string GetAllParametersCombinedIntoOneString()
		{
			string str = this.NoOfferAvailableReason.ToString();
			str += this.GetParameterValueAsString(this.AllOffersNames);
			str += this.GetParameterValueAsString(this.AllOffersValidationInfo);
			str += this.GetParameterValueAsString(this.AllOffersActivationState);
			str += this.GetParameterValueAsString(this.AllOffersLevelRequirement);
			str += this.GetParameterValueAsString(this.AllOffersFeatureInstanceId);
			return str + this.GetParameterValueAsString(this.AllOffersLastShowingTimeStamp);
		}

		private string GetParameterValueAsString(TactileAnalytics.OptionalParam<string> parameter)
		{
			if (parameter.GetValue() != null)
			{
				parameter.ToString();
			}
			return string.Empty;
		}

		private string GetFormattedTimeStamp(int timeStamp)
		{
			return DateHelper.GetDateTimeFromUnixTimestamp((long)timeStamp).ToString("yyyy-MM-dd HH:mm:ss");
		}

		private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
	}
}
