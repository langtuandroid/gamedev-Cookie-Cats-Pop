using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LoadBytesTextureForRenderer : MonoBehaviour
{
	private void Start()
	{
		Renderer component = base.GetComponent<Renderer>();
		if (component.sharedMaterial == null)
		{
			return;
		}
		TextAsset textAsset = (TextAsset)Resources.Load(this.textureResourcePath);
		if (textAsset == null)
		{
			return;
		}
		Texture2D texture2D = new Texture2D(1, 1);
		texture2D.wrapMode = this.textureWrapMode;
		texture2D.LoadImage(textAsset.bytes, true);
		component.material.mainTexture = texture2D;
	}

	public string textureResourcePath;

	public TextureWrapMode textureWrapMode;
}
