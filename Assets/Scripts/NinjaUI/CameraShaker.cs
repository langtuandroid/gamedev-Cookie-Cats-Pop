using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

namespace NinjaUI
{
	public static class CameraShaker
	{
		public static IEnumerator ShakeDecreasing(float duration = 0.3f, float amount = 10f, float tremor = 30f, float phase = 0f, bool reverse = false)
		{
			AnimationCurve curve = (!reverse) ? AnimationCurve.Linear(0f, 1f, 1f, 0f) : AnimationCurve.Linear(0f, 0f, 1f, 1f);
			for (int i = 0; i < UICamera.AllCameras.Count; i++)
			{
				if (!CameraShaker.positions.ContainsKey(UICamera.AllCameras[i]))
				{
					CameraShaker.positions.Add(UICamera.AllCameras[i], UICamera.AllCameras[i].transform.localPosition);
				}
			}
			yield return new Fiber.OnExit(delegate()
			{
				CameraShaker.isShaking = false;
				for (int j = 0; j < UICamera.AllCameras.Count; j++)
				{
					if (CameraShaker.positions.ContainsKey(UICamera.AllCameras[j]))
					{
						Vector2 vector = CameraShaker.positions[UICamera.AllCameras[j]];
						UICamera.AllCameras[j].transform.localPosition = new Vector3(vector.x, vector.y, UICamera.AllCameras[j].transform.localPosition.z);
					}
				}
			});
			CameraShaker.isShaking = true;
			Vector3 p = Vector3.zero;
			yield return FiberAnimation.Animate(duration, curve, delegate(float t)
			{
				p.x = Mathf.Sin(t * tremor + phase) * t * amount;
				p.y = Mathf.Cos(t * tremor * 0.6f + phase) * t * amount;
				for (int j = 0; j < UICamera.AllCameras.Count; j++)
				{
					if (!CameraShaker.positions.ContainsKey(UICamera.AllCameras[j]))
					{
						CameraShaker.positions.Add(UICamera.AllCameras[j], UICamera.AllCameras[j].transform.localPosition);
					}
					if (CameraShaker.positions.ContainsKey(UICamera.AllCameras[j]))
					{
						UICamera uicamera = UICamera.AllCameras[j];
						float num = uicamera.cachedCamera.orthographicSize * 2f / UIViewManager.Instance.CurrentScreenWidth;
						Vector2 vector = p;
						vector.x = (float)Mathf.RoundToInt(p.x / num) * num;
						vector.y = (float)Mathf.RoundToInt(p.y / num) * num;
						Vector2 vector2 = CameraShaker.positions[UICamera.AllCameras[j]];
						UICamera.AllCameras[j].transform.localPosition = new Vector3(vector2.x + vector.x, vector2.y + vector.y, UICamera.AllCameras[j].transform.localPosition.z);
					}
				}
			}, false);
			yield break;
		}

		private static Dictionary<UICamera, Vector2> positions = new Dictionary<UICamera, Vector2>();

		public static bool isShaking;
	}
}
