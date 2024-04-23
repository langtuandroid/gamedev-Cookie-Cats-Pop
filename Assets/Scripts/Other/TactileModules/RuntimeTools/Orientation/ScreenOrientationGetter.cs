using System;
using UnityEngine;

namespace TactileModules.RuntimeTools.Orientation
{
	public class ScreenOrientationGetter : IScreenOrientationGetter
	{
		public ScreenOrientation GetOrientation()
		{
			return Screen.orientation;
		}

		public ScreenOrientation GetOppositeOrientation()
		{
			return (Screen.orientation != ScreenOrientation.LandscapeLeft && Screen.orientation != ScreenOrientation.LandscapeRight) ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
		}
	}
}
