using System;
using UnityEngine;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class AssetBundleLinkageHelper
	{
		public static void RestoreReferences(GameObject go)
		{
			UIProjectSettings uiProjectSettings = UIProjectSettings.Get();
			AssetBundleLinkageHelper.RestoreAtlas(go, uiProjectSettings);
			AssetBundleLinkageHelper.RestoreFont(go, uiProjectSettings);
			AssetBundleLinkageHelper.UpdateShaderForTextureQuads(go);
			AssetBundleLinkageHelper.UpdateShaderForSkeletonAnimations(go);
		}

		private static void RestoreAtlas(GameObject go, UIProjectSettings uiProjectSettings)
		{
			UIAtlas defaultAtlas = uiProjectSettings.defaultAtlas;
			Texture mainTexture = defaultAtlas.Materials[0].mainTexture;
			foreach (UISprite uisprite in go.GetComponentsInChildren<UISprite>(true))
			{
				Shader shader = uisprite.renderer.materials[0].shader;
				if (shader == null)
				{
					uisprite.renderer.materials[0].shader = defaultAtlas.Materials[0].shader;
				}
				else
				{
					uisprite.renderer.materials[0].shader = Shader.Find(shader.name);
				}
				uisprite.Atlas = defaultAtlas;
				uisprite.renderer.materials[0].mainTexture = mainTexture;
			}
		}

		private static void RestoreFont(GameObject go, UIProjectSettings uiProjectSettings)
		{
			foreach (UILabel uilabel in go.GetComponentsInChildren<UILabel>(true))
			{
				bool flag = uilabel.font == null || uilabel.font.Material == null || uilabel.font.Material.mainTexture == null;
				if (flag)
				{
					string b = (!(uilabel.font != null)) ? string.Empty : uilabel.font.name;
					uilabel.font = null;
					uilabel.font = ((!(uiProjectSettings.defaultFont.name == b)) ? uiProjectSettings.secondaryFont : uiProjectSettings.defaultFont);
				}
			}
		}

		private static void UpdateShaderForTextureQuads(GameObject go)
		{
			foreach (UITextureQuad uitextureQuad in go.GetComponentsInChildren<UITextureQuad>(true))
			{
				uitextureQuad.Material.shader = Shader.Find(uitextureQuad.Material.shader.name);
			}
		}

		private static void UpdateShaderForSkeletonAnimations(GameObject go)
		{
			foreach (SkeletonAnimation skeletonAnimation in go.GetComponentsInChildren<SkeletonAnimation>(true))
			{
				string name = skeletonAnimation.GetComponent<Renderer>().materials[0].shader.name;
				skeletonAnimation.GetComponent<Renderer>().materials[0].shader = Shader.Find(name);
			}
		}
	}
}
