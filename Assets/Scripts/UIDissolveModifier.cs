using System;
using UnityEngine;

public class UIDissolveModifier : UIWidgetModifier
{
	public float Progress
	{
		get
		{
			return this.progress;
		}
		set
		{
			this.progress = value;
			base.MarkAsChanged();
		}
	}

	public float DissolveWidth
	{
		get
		{
			return this.dissolveWidth;
		}
		set
		{
			this.dissolveWidth = value;
			base.MarkAsChanged();
		}
	}

	protected override void Modify(MeshData mesh)
	{
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 0f);
		base.GetMinMax(mesh.verts, ref vector, ref vector2);
		float num = vector2.x - vector.x;
		float num2 = this.progress * num;
		for (int i = 0; i < mesh.verts.Count; i++)
		{
			float num3 = Mathf.Clamp01((mesh.verts[i].x - num2) / this.dissolveWidth);
			Color value = mesh.colors[i];
			value.a *= num3;
			mesh.colors[i] = value;
		}
	}

	[SerializeField]
	private float progress;

	[SerializeField]
	private float dissolveWidth = 0.1f;
}
