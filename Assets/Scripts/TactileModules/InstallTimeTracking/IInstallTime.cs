using System;

namespace TactileModules.InstallTimeTracking
{
	public interface IInstallTime
	{
		int GetSecondsSinceFirstInstall();
	}
}
