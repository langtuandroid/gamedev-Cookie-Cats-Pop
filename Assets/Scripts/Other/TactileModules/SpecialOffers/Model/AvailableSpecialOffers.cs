using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.SpecialOffers.Model
{
	public class AvailableSpecialOffers : IAvailableSpecialOffers
	{
		public AvailableSpecialOffers(IFeatureManager featureManager, IFeatureTypeHandler featureTypeHandler)
		{
			this.featureManager = featureManager;
			this.featureTypeHandler = featureTypeHandler;
		}

		public List<ISpecialOffer> GetOffers()
		{
			List<ISpecialOffer> list = new List<ISpecialOffer>();
			List<string> allFeatureIds = this.GetAllFeatureIds();
			foreach (string featureInstanceId in allFeatureIds)
			{
				SpecialOffer specialOffer = new SpecialOffer(this.featureManager, this.featureTypeHandler, featureInstanceId);
				if (specialOffer.IsValid())
				{
					list.Add(specialOffer);
				}
			}
			return list;
		}

		private List<string> GetAllFeatureIds()
		{
			List<string> list = new List<string>();
			List<ActivatedFeatureInstanceData> activatedFeatures = this.featureManager.GetActivatedFeatures(this.featureTypeHandler);
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in activatedFeatures)
			{
				list.Add(activatedFeatureInstanceData.Id);
			}
			List<FeatureData> availableFeatures = this.featureManager.GetAvailableFeatures(this.featureTypeHandler);
			foreach (FeatureData featureData in availableFeatures)
			{
				string id = featureData.Id;
				if (!list.Contains(id))
				{
					list.Add(id);
				}
			}
			return list;
		}

		private readonly IFeatureManager featureManager;

		private readonly IFeatureTypeHandler featureTypeHandler;
	}
}
