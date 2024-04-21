using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UITaperedSprite : UISprite
{
	public float TaperAmount
	{
		get
		{
			return this.taperAmount;
		}
		set
		{
			if (this.taperAmount == value)
			{
				return;
			}
			this.taperAmount = value;
			this.MarkAsChanged();
		}
	}

	private void GetMinMax(List<Vector3> verts, ref Vector2 min, ref Vector2 max)
	{
		if (verts.Count == 0)
		{
			return;
		}
		min = new Vector2(verts[0].x, verts[0].y);
		max = min;
		for (int i = 1; i < verts.Count; i++)
		{
			if (verts[i].x < min.x)
			{
				min.x = verts[i].x;
			}
			if (verts[i].y < min.y)
			{
				min.y = verts[i].y;
			}
			if (verts[i].x > max.x)
			{
				max.x = verts[i].x;
			}
			if (verts[i].y > max.y)
			{
				max.y = verts[i].y;
			}
		}
	}

	protected override void OnFill(MeshData meshData)
	{
		base.OnFill(meshData);
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		this.GetMinMax(meshData.verts, ref vector, ref vector2);
		Vector2 vector3 = vector2 - vector;
		Vector2 vector4 = (vector + vector2) * 0.5f;
		float num = this.taperAmount * vector3.x * 0.5f / vector3.y;
		for (int i = 0; i < meshData.verts.Count; i++)
		{
			Vector3 value = meshData.verts[i];
			bool flag = value.x > vector4.x;
			float num2 = vector2.y - value.y;
			if (flag)
			{
				num2 = -num2;
			}
			value.x += num2 * num;
			if (!flag)
			{
				value.x = Mathf.Min(value.x, vector4.x);
			}
			else
			{
				value.x = Mathf.Max(value.x, vector4.x);
			}
			meshData.verts[i] = value;
		}
	}

	[SerializeField]
	private float taperAmount;
}
