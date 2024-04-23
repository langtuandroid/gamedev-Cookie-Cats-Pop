using System;
using UnityEngine;

namespace TactileModules.SagaCore
{
	public class SagaMapView : UIView
	{
		public MapStreamer MapStreamer
		{
			get
			{
				if (this.cachedMapStreamer == null)
				{
					this.cachedMapStreamer = base.GetComponentInChildren<MapStreamer>();
				}
				return this.cachedMapStreamer;
			}
		}

		public void MarkOriginalSize()
		{
			base.OriginalSize = base.GetElementSize();
		}

		public override Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
		{
			float num = screenSize.Aspect();
			Vector2 originalSize = base.OriginalSize;
			if (num > 1f)
			{
				originalSize.x = 960f;
				originalSize.y = originalSize.x / num;
			}
			else
			{
				originalSize.x = 640f;
				originalSize.y = originalSize.x / num;
			}
			return originalSize;
		}

		private MapStreamer cachedMapStreamer;
	}
}
