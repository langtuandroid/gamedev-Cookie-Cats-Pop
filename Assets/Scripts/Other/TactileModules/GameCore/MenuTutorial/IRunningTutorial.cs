using System;

namespace TactileModules.GameCore.MenuTutorial
{
	public interface IRunningTutorial
	{
		IMenuTutorialDefinition Definition { get; }

		void Start();

		void Stop();

		bool Step();
	}
}
