using System;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public interface IButtonAreaControllerButton<T> where T : Component
	{
		T Component { get; }
	}
}
