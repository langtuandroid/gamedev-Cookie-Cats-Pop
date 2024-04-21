using System;

namespace TactileModules.GameCore.MenuTutorial
{
	public interface IRunningTutorialFactory
	{
		IRunningTutorial CreateTutorial(IMenuTutorialDefinition definition);
	}
}
