using System;
using Tactile;
using TactileModules.AgeInfo;
using TactileModules.Foundation;

public class AgeInfoManagerProvider : AgeInfoManager.IDataProvider
{
	public AgeInfoConfig Config
	{
		get
		{
			return ConfigurationManager.Get<AgeInfoConfig>();
		}
	}

	public void ShowInvalidAgeView(int ageThreshold)
	{
		string text = string.Format(L.Get("You must be at least {0} to play Cookie Cats Pop."), ageThreshold);
		ManagerRepository.Get<UIViewManager>().ShowView<MessageBoxView>(new object[]
		{
			L.Get("Unable to continue"),
			text
		});
	}
}
