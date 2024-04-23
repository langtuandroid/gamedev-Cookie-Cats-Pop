using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapLighting
	{
		public static void UpdateLighting(Camera renderCamera, RenderTexture lightingTexture, int lightingLayer)
		{
			Matrix4x4 projectionMatrix = renderCamera.projectionMatrix;
			int cullingMask = renderCamera.cullingMask;
			renderCamera.cullingMask = 1 << lightingLayer;
			renderCamera.targetTexture = lightingTexture;
			renderCamera.clearFlags = CameraClearFlags.Color;
			renderCamera.backgroundColor = Color.black;
			renderCamera.projectionMatrix = projectionMatrix;
			renderCamera.Render();
			renderCamera.targetTexture = null;
			renderCamera.cullingMask = (cullingMask & ~renderCamera.cullingMask);
			renderCamera.clearFlags = CameraClearFlags.Depth;
			renderCamera.ResetProjectionMatrix();
		}

		public static void Initialize()
		{
			Shader.SetGlobalTexture("_LightTexture", GardenGameSetup.Get.lightingRenderTexture);
			Shader.SetGlobalColor("_NightColor", GardenGameSetup.Get.nightColor);
			MapLighting.NightColorR = GardenGameSetup.Get.nightColorR;
			MapLighting.NightColorG = GardenGameSetup.Get.nightColorG;
			MapLighting.NightColorB = GardenGameSetup.Get.nightColorB;
			Shader.EnableKeyword("ALPHA_SPLITTING_ON");
			Shader.DisableKeyword("ALPHA_SPLITTING_OFF");
			MapLighting.Darkness = 0f;
		}

		public static float Darkness
		{
			get
			{
				return MapLighting.darkness;
			}
			set
			{
				MapLighting.darkness = value;
				Shader.SetGlobalFloat("_NightBlend", MapLighting.darkness);
			}
		}

		public static Color NightColorR
		{
			get
			{
				return MapLighting.nightColorR;
			}
			set
			{
				MapLighting.nightColorR = value;
				Shader.SetGlobalColor("_NightColorR", MapLighting.nightColorR);
			}
		}

		public static Color NightColorG
		{
			get
			{
				return MapLighting.nightColorG;
			}
			set
			{
				MapLighting.nightColorG = value;
				Shader.SetGlobalColor("_NightColorG", MapLighting.nightColorG);
			}
		}

		public static Color NightColorB
		{
			get
			{
				return MapLighting.nightColorB;
			}
			set
			{
				MapLighting.nightColorB = value;
				Shader.SetGlobalColor("_NightColorB", MapLighting.nightColorB);
			}
		}

		private static float darkness;

		private static Color nightColorR;

		private static Color nightColorG;

		private static Color nightColorB;
	}
}
