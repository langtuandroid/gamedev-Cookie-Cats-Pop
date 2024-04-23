using System;
using TactileModules.SagaCore;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.Views
{
	public class PlayablePostcardMapView : SagaMapView
	{
		public override Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
		{
			float num = screenSize.Aspect();
			Vector2 originalSize = base.OriginalSize;
			originalSize.y = 1680f;
			originalSize.x = originalSize.y * num;
			return originalSize;
		}
	}
}
