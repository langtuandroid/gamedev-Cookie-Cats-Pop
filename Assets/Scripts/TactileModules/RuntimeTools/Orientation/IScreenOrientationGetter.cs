using System;
using UnityEngine;

namespace TactileModules.RuntimeTools.Orientation
{
	public interface IScreenOrientationGetter
	{
		ScreenOrientation GetOrientation();

		ScreenOrientation GetOppositeOrientation();
	}
}
