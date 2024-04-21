using System;
using System.Collections.Generic;

namespace TactileModules.GameCore.MenuTutorial
{
	public interface IMenuTutorialDefinition
	{
		int LevelRequired { get; }

		List<MenuTutorialStep> Steps { get; }
	}
}
