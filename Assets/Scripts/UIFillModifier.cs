using System;
using UnityEngine;

public class UIFillModifier : UIWidgetModifier
{
	public float FillAmount
	{
		get
		{
			return this.fillAmount;
		}
		set
		{
			this.fillAmount = value;
			base.MarkAsChanged();
		}
	}

	public float FillAmountVertical
	{
		get
		{
			return this.fillAmountVertical;
		}
		set
		{
			this.fillAmountVertical = value;
			base.MarkAsChanged();
		}
	}

	protected override void Modify(MeshData mesh)
	{
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 0f);
		base.GetMinMax(mesh.verts, ref vector, ref vector2);
		float sliceX = vector.x + (vector2.x - vector.x) * this.fillAmount;
		float sliceY = vector.y + (vector2.y - vector.y) * this.fillAmountVertical;
		for (int i = 0; i < mesh.verts.Count; i += 4)
		{
			this.DoQuad(sliceX, mesh, i);
			this.DoQuadVertical(sliceY, mesh, i);
		}
	}

	private void DoQuad(float sliceX, MeshData mesh, int index)
	{
		if (mesh.verts[index + 3].x > sliceX)
		{
			for (int i = 0; i < 4; i++)
			{
				Color value = mesh.colors[index + i];
				value.a = 0f;
				mesh.colors[index + i] = value;
			}
			for (int j = 0; j < 3; j++)
			{
				mesh.verts[index + j] = mesh.verts[index + 3];
			}
		}
		else
		{
			if (mesh.verts[index].x < sliceX)
			{
				return;
			}
			float num = sliceX - mesh.verts[index + 3].x;
			float t = num / (mesh.verts[index].x - mesh.verts[index + 3].x);
			mesh.verts[index] = Vector3.Lerp(mesh.verts[index + 3], mesh.verts[index], t);
			mesh.verts[index + 1] = Vector3.Lerp(mesh.verts[index + 2], mesh.verts[index + 1], t);
			mesh.uvs[index] = Vector2.Lerp(mesh.uvs[index + 3], mesh.uvs[index], t);
			mesh.uvs[index + 1] = Vector2.Lerp(mesh.uvs[index + 2], mesh.uvs[index + 1], t);
		}
	}

	private void DoQuadVertical(float sliceY, MeshData mesh, int index)
	{
		if (mesh.verts[index + 1].y > sliceY)
		{
			for (int i = 0; i < 4; i++)
			{
				Color value = mesh.colors[index + i];
				value.a = 0f;
				mesh.colors[index + i] = value;
			}
			for (int j = 0; j < 3; j++)
			{
				mesh.verts[index + j] = mesh.verts[index + 3];
			}
		}
		else
		{
			if (mesh.verts[index].y < sliceY)
			{
				return;
			}
			float num = sliceY - mesh.verts[index + 1].y;
			float t = num / (mesh.verts[index].y - mesh.verts[index + 1].y);
			mesh.verts[index] = Vector3.Lerp(mesh.verts[index + 1], mesh.verts[index], t);
			mesh.verts[index + 3] = Vector3.Lerp(mesh.verts[index + 2], mesh.verts[index + 3], t);
			mesh.uvs[index] = Vector2.Lerp(mesh.uvs[index + 1], mesh.uvs[index], t);
			mesh.uvs[index + 3] = Vector2.Lerp(mesh.uvs[index + 2], mesh.uvs[index + 3], t);
		}
	}

	[SerializeField]
	private float fillAmount = 1f;

	[SerializeField]
	private float fillAmountVertical = 1f;
}
