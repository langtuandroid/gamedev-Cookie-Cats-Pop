using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class PostcardImageCapture
	{
		public Texture2D CapturePostcard(GameObject postcard, Shader captureShader)
		{
			Vector3 position = postcard.transform.position;
			postcard.transform.position = postcard.transform.position + new Vector3(10000f, 10000f, 0f);
			Shader shader = postcard.GetComponentInChildren<UITextureQuad>(true).Material.shader;
			this.SetShader(postcard, captureShader);
			this.CalculatePostcardSize(postcard);
			RenderTexture renderTexture = this.CreateRenderTexture();
			Camera camera = this.CreateCaptureCamera(postcard, renderTexture);
			Texture2D result = this.RenderToTexture(renderTexture, camera);
			UnityEngine.Object.Destroy(renderTexture);
			UnityEngine.Object.Destroy(camera.gameObject);
			postcard.transform.position = position;
			this.SetShader(postcard, shader);
			return result;
		}

		private void SetShader(GameObject postcard, Shader shader)
		{
			foreach (UITextureQuad uitextureQuad in postcard.GetComponentsInChildren<UITextureQuad>(true))
			{
				uitextureQuad.Material.shader = shader;
			}
		}

		private RenderTexture CreateRenderTexture()
		{
			return new RenderTexture((int)this.textureSize.x, (int)this.textureSize.y, 24)
			{
				format = RenderTextureFormat.ARGB32
			};
		}

		private Camera CreateCaptureCamera(GameObject postcard, RenderTexture renderTexture)
		{
			Camera camera = new GameObject("TemporaryCapturePostcardCamera").AddComponent<Camera>();
			camera.transform.position = postcard.transform.position + Vector3.back * 100f;
			camera.transform.rotation = postcard.transform.rotation;
			camera.orthographic = true;
			camera.orthographicSize = this.worldSize.y / 2f;
			camera.cullingMask = 1 << postcard.gameObject.layer;
			camera.clearFlags = CameraClearFlags.Color;
			camera.targetTexture = renderTexture;
			camera.backgroundColor = Color.clear;
			return camera;
		}

		private Texture2D RenderToTexture(RenderTexture renderTexture, Camera captureCamera)
		{
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D((int)this.textureSize.x, (int)this.textureSize.y, TextureFormat.ARGB32, false);
			captureCamera.Render();
			texture2D.ReadPixels(new Rect(0f, 0f, this.textureSize.x, this.textureSize.y), 0, 0);
			texture2D.Apply();
			RenderTexture.active = null;
			return texture2D;
		}

		private void CalculatePostcardSize(GameObject postcard)
		{
			this.worldSize = Vector2.zero;
			IEnumerator enumerator = postcard.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					UIElement component = transform.GetComponent<UIElement>();
					if (component != null)
					{
						if (component.Size.x > this.worldSize.x)
						{
							this.worldSize.x = component.Size.x;
						}
						if (component.Size.y > this.worldSize.y)
						{
							this.worldSize.y = component.Size.y;
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.worldSize.Scale(postcard.transform.localScale);
			this.textureSize = this.worldSize * 1.6f;
		}

		private const float SCALE_FACTOR = 1.6f;

		private Vector2 worldSize;

		private Vector2 textureSize;
	}
}
