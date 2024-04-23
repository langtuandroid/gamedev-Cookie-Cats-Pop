using System;

namespace TactileModules.GameCore.ButtonArea
{
	public interface IButtonAreaSystem
	{
		IButtonAreaModel Model { get; }

		IButtonAreaController Controller { get; }
	}
}
