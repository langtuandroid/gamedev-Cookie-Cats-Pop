using System;
using UnityEngine;

public class UICurveModifier : UIWidgetModifier
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

	public float Width
	{
		get
		{
			return this.width;
		}
		set
		{
			this.width = value;
			base.MarkAsChanged();
		}
	}

	protected override void Modify(MeshData mesh)
	{
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 0f);
		base.GetMinMax(mesh.verts, ref vector, ref vector2);
		if (this.width > 1E-05f)
		{
			vector.x = (vector.x + vector2.x) * 0.5f - this.width * 0.5f;
			vector2.x = vector.x + this.width;
		}
		float num = vector2.x - vector.x;
		for (int i = 0; i < mesh.verts.Count; i++)
		{
			Vector2 v = mesh.verts[i];
			float num2 = (v.x - vector.x) / num;
			v.y += Mathf.Sin(num2 * 3.14159274f) * this.height;
			mesh.verts[i] = v;
		}
	}

	[SerializeField]
	private float height;

	[SerializeField]
	private float width;
}
