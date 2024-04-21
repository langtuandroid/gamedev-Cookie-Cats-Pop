using System;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	public class FeatureManagerBasicEventDecorator : IEventDecorator<BasicEvent>, IEventDecorator
	{
		public FeatureManagerBasicEventDecorator(FeatureManager featureManagerInstance)
		{
			this.featureManager = featureManagerInstance;
		}

		public void Decorate(BasicEvent basicEvent)
		{
			string text = this.featureManager.GetCommaSeperatedListOfActiveFeatureTypes();
			string text2 = this.featureManager.GetCommaSeperatedListOfActiveFeaturesIds();
			text = ((!(text == string.Empty)) ? text : null);
			text2 = ((!(text2 == string.Empty)) ? text2 : null);
			basicEvent.SetFeatureManagerParameters(text, text2);
		}

		private readonly FeatureManager featureManager;
	}
}
