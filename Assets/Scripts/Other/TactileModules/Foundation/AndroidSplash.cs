using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.Foundation
{
	public class AndroidSplash : MonoBehaviour
	{
		public static AndroidSplash Create()
		{
			GameObject gameObject = new GameObject("AndroidSplash");
			return gameObject.AddComponent<AndroidSplash>();
		}

		public IEnumerator Show(Action splashReady)
		{
			this.DisableAutoRotation();
			this.CreateSplash();
			this.FitImageToScreen();
			yield return null;
			yield return null;
			yield return null;
			splashReady();
			yield return null;
			this.RestoreAutoRotation();
			AndroidRotationLockHelper.Initialize();
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}

		private void DisableAutoRotation()
		{
			Screen.orientation = AndroidRotationLockHelper.GetScreenOrientation(Screen.orientation, this.IsAccelerometerEnabled());
		}

		private bool IsAccelerometerEnabled()
		{
			return DeviceUtilsAndroid.getAccelerometerRotation() == 1;
		}

		private void RestoreAutoRotation()
		{
			if (this.IsAccelerometerEnabled())
			{
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
		}

		private void CreateSplash()
		{
			GameObject gameObject = new GameObject("SplashScreen");
			gameObject.transform.SetParent(base.transform);
			this.camera = gameObject.AddComponent<Camera>();
			this.camera.orthographic = true;
			GameObject gameObject2 = new GameObject("Texture");
			gameObject2.transform.SetParent(gameObject.transform);
			string url = string.Concat(new string[]
			{
				"jar:file://",
				Application.dataPath,
				"!/res/drawable-",
				this.GetDensityName(),
				"-v4/unity_static_splash.png"
			});
			WWW www = new WWW(url);
			while (!www.isDone)
			{
			}
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
			if (string.IsNullOrEmpty(www.error))
			{
				www.LoadImageIntoTexture(texture2D);
			}
			this.spriteRenderer = gameObject2.AddComponent<SpriteRenderer>();
			this.spriteRenderer.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f);
			this.spriteRenderer.transform.position = new Vector3(0f, 0f, 10f);
		}

		private string GetDensityName()
		{
			float density = DeviceUtilsAndroid.getDensity();
			if ((double)density >= 4.0)
			{
				return "xxxhdpi";
			}
			if ((double)density >= 3.0)
			{
				return "xxhdpi";
			}
			if ((double)density >= 2.0)
			{
				return "xhdpi";
			}
			if ((double)density >= 1.5)
			{
				return "hdpi";
			}
			if ((double)density >= 1.0)
			{
				return "mdpi";
			}
			return "ldpi";
		}

		private void FitImageToScreen()
		{
			Vector2 correctedScreenDimensions = AndroidSplash.GetCorrectedScreenDimensions();
			float num = this.camera.orthographicSize * 2f;
			float num2 = num / correctedScreenDimensions.y * correctedScreenDimensions.x;
			float num3 = num2 / this.spriteRenderer.sprite.bounds.size.x;
			float num4 = num / this.spriteRenderer.sprite.bounds.size.y;
			float num5 = (num3 <= num4) ? num4 : num3;
			this.spriteRenderer.transform.localScale = new Vector3(num5, num5, 1f);
		}

		private static Vector2 GetCorrectedScreenDimensions()
		{
			switch (Screen.orientation)
			{
			case ScreenOrientation.Portrait:
			case ScreenOrientation.PortraitUpsideDown:
				if (Screen.height < Screen.width)
				{
					return new Vector2((float)Screen.height, (float)Screen.width);
				}
				break;
			case ScreenOrientation.LandscapeLeft:
			case ScreenOrientation.LandscapeRight:
				if (Screen.height > Screen.width)
				{
					return new Vector2((float)Screen.height, (float)Screen.width);
				}
				break;
			}
			return new Vector2((float)Screen.width, (float)Screen.height);
		}

		private Camera camera;

		private SpriteRenderer spriteRenderer;
	}
}
