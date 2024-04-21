using System;
using System.Collections.Generic;
using UnityEngine;

public class UIShadowModifier : UIWidgetModifier
{
	public Vector2 Shadow
	{
		get
		{
			return this.shadow;
		}
		set
		{
			this.shadow = value;
			base.MarkAsChanged();
		}
	}

	public float Stroke
	{
		get
		{
			return this.stroke;
		}
		set
		{
			this.stroke = value;
			base.MarkAsChanged();
		}
	}

	public Color StrokeColor
	{
		get
		{
			return this.strokeColor;
		}
		set
		{
			this.strokeColor = value;
			base.MarkAsChanged();
		}
	}

	public Color ShadowColor
	{
		get
		{
			return this.shadowColor;
		}
		set
		{
			this.shadowColor = value;
			base.MarkAsChanged();
		}
	}

	public bool ShadowEnabled
	{
		get
		{
			return this.shadow != Vector2.zero;
		}
	}

	public bool StrokeEnabled
	{
		get
		{
			return !Mathf.Approximately(this.stroke, 0f);
		}
	}

	protected override void Modify(MeshData meshData)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < meshData.numSubmeshes; i++)
		{
			list.Add(meshData.GetIndices(i).Count);
		}
		int count = meshData.verts.Count;
		Vector3 localScale = base.transform.localScale;
		if (localScale.x == 0f || localScale.y == 0f)
		{
			return;
		}
		if (this.StrokeEnabled)
		{
			Vector2 vector = Vector2.one * this.stroke;
			this.ApplyShadow(meshData, count, list, vector.x, -vector.y, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, -vector.x, -vector.y, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, vector.x, vector.y, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, -vector.x, vector.y, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, -vector.x, 0f, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, vector.x, 0f, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, 0f, -vector.y, this.StrokeColor);
			this.ApplyShadow(meshData, count, list, 0f, vector.y, this.StrokeColor);
		}
		if (this.ShadowEnabled)
		{
			Vector2 vector2 = this.shadow;
			this.ApplyShadow(meshData, count, list, vector2.x, -vector2.y, this.shadowColor);
		}
	}

	private void ApplyShadow(MeshData meshData, int orgVertexCount, List<int> orgIndexCounts, float x, float y, Color c)
	{
		for (int i = 0; i < orgVertexCount; i++)
		{
			Vector3 item = meshData.verts[i];
			item.x += x;
			item.y += y;
			item.z = item.z;
			meshData.verts.Add(item);
			Color color = meshData.colors[i];
			Color item2 = c;
			item2.a *= color.a * item2.a;
			meshData.colors.Add(item2);
			meshData.uvs.Add(meshData.uvs[i]);
		}
		for (int j = 0; j < meshData.numSubmeshes; j++)
		{
			List<int> indices = meshData.GetIndices(j);
			List<int> list = new List<int>();
			for (int k = 0; k < orgIndexCounts[j]; k++)
			{
				list.Add(indices[k] + orgVertexCount);
			}
			indices.InsertRange(0, list);
		}
	}

	[SerializeField]
	private Vector2 shadow = new Vector2(6f, 6f);

	[SerializeField]
	private float stroke = 6f;

	[SerializeField]
	private Color strokeColor = Color.black;

	[SerializeField]
	private Color shadowColor = new Color(0f, 0f, 0f, 0.5f);
}
