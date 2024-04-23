using System;
using System.Collections.Generic;

namespace TactileModules.GameCore.MenuTutorial
{
	public interface IMenuTutorialDatabase
	{
		IEnumerable<IMenuTutorialDefinition> Tutorials { get; }
	}
}
