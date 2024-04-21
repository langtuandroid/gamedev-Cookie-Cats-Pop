using System;
using UnityEngine;

[ExecuteInEditMode]
public class UITiledSprite : UISprite
{
	protected override void OnFill(MeshData meshData)
	{
		Texture mainTexture = base.MainTexture;
		if (mainTexture == null)
		{
			return;
		}
		Rect rect = this.mInner;
		if (base.Atlas.coordinates == UIAtlas.Coordinates.TexCoords)
		{
			rect = UIMath.ConvertToPixels(rect, mainTexture.width, mainTexture.height, true);
		}
		Vector2 size = base.Size;
		float num = base.Atlas.pixelSize * base.PixelSizeFactor;
		float num2 = Mathf.Abs(rect.width / size.x) * num;
		float num3 = Mathf.Abs(rect.height / size.y) * num;
		if (num2 < 0.01f || num3 < 0.01f)
		{
			num2 = 0.01f;
			num3 = 0.01f;
		}
		Vector2 vector = new Vector2(rect.xMin / (float)mainTexture.width, rect.yMin / (float)mainTexture.height);
		Vector2 vector2 = new Vector2(rect.xMax / (float)mainTexture.width, rect.yMax / (float)mainTexture.height);
		Vector2 vector3 = vector2;
		for (float num4 = 0f; num4 < 1f; num4 += num3)
		{
			float num5 = 0f;
			vector3.x = vector2.x;
			float num6 = num4 + num3;
			if (num6 > 1f)
			{
				vector3.y = vector.y + (vector2.y - vector.y) * (1f - num4) / (num6 - num4);
				num6 = 1f;
			}
			while (num5 < 1f)
			{
				float num7 = num5 + num2;
				if (num7 > 1f)
				{
					vector3.x = vector.x + (vector2.x - vector.x) * (1f - num5) / (num7 - num5);
					num7 = 1f;
				}
				meshData.verts.Add(new Vector3(num7 * base.Size.x, -num4 * base.Size.y, 0f));
				meshData.verts.Add(new Vector3(num7 * base.Size.x, -num6 * base.Size.y, 0f));
				meshData.verts.Add(new Vector3(num5 * base.Size.x, -num6 * base.Size.y, 0f));
				meshData.verts.Add(new Vector3(num5 * base.Size.x, -num4 * base.Size.y, 0f));
				meshData.uvs.Add(new Vector2(vector3.x, 1f - vector.y));
				meshData.uvs.Add(new Vector2(vector3.x, 1f - vector3.y));
				meshData.uvs.Add(new Vector2(vector.x, 1f - vector3.y));
				meshData.uvs.Add(new Vector2(vector.x, 1f - vector.y));
				meshData.colors.Add(base.Color);
				meshData.colors.Add(base.Color);
				meshData.colors.Add(base.Color);
				meshData.colors.Add(base.Color);
				num5 += num2;
			}
		}
	}
}
