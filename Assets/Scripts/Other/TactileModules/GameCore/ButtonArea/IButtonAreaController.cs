using System;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public interface IButtonAreaController
	{
		IButtonAreaControllerButton<T> GetButton<T>(Predicate<T> predicate) where T : Component;
	}
}
