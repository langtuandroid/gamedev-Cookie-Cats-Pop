using System;
using System.Collections;

namespace TactileModules.TactileCloud.TargetingParameters
{
	public interface ITargetingParametersProvider
	{
		Hashtable GetAdditionalTargetingParameters();
	}
}
