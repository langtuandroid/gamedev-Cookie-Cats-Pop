using System;
using TactileModules.SagaCore;
using UnityEngine;

public class OneLifeChallengeMapSagaView : SagaMapView
{
	public override Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
	{
		float num = screenSize.Aspect();
		Vector2 originalSize = base.OriginalSize;
		originalSize.y = 1280f;
		originalSize.x = originalSize.y * num;
		return originalSize;
	}
}
