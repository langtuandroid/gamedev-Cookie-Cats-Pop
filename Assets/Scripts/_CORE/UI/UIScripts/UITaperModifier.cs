using System;
using UnityEngine;

public class UITaperModifier : UIWidgetModifier
{
	public float Height
	{
		get
		{
			return this.height;
		}
		set
		{
			this.height = value;
			base.MarkAsChanged();
		}
	}

	protected override void Modify(MeshData meshData)
	{
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		base.GetMinMax(meshData.verts, ref vector, ref vector2);
		Vector2 vector3 = vector2 - vector;
		Vector2 vector4 = (vector + vector2) * 0.5f;
		float num = this.height * vector3.x * 0.5f / vector3.y;
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
	private float height;
}
