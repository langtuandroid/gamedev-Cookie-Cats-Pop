using System;
using System.Collections.Generic;

namespace TactileModules.GameCore.MenuTutorial
{
	public interface IMenuTutorialModel
	{
		void AddRunningTutorial(IRunningTutorial tutorial);

		void RemoveRunningTutorial(IRunningTutorial tutorial);

		IEnumerable<IRunningTutorial> RunningTutorials { get; }

		IEnumerable<IMenuTutorialDefinition> TutorialDefinitions { get; }

		bool Empty { get; }
	}
}
