using System;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.MenuTutorial
{
	public class RunningTutorialFactory : IRunningTutorialFactory
	{
		public RunningTutorialFactory(IUIController uiController, MenuTutorialMessage messagePrefab)
		{
			this.uiController = uiController;
			this.messagePrefab = messagePrefab;
		}

		public IRunningTutorial CreateTutorial(IMenuTutorialDefinition definition)
		{
			return new RunningTutorial(definition, this.uiController, this.messagePrefab);
		}

		private readonly IUIController uiController;

		private readonly MenuTutorialMessage messagePrefab;
	}
}
