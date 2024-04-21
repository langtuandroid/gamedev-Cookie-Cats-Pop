using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class UITextureCoordRectangle : MonoBehaviour
{
	private void Start()
	{
		this.widget = base.GetComponent<UIWidget>();
		this.widget.OnPostFill += this.OnPostFill;
		this.widget.MarkAsChanged();
	}

	public Vector2 StartCoord
	{
		get
		{
			return this.startCoord;
		}
		set
		{
			if (this.startCoord == value)
			{
				return;
			}
			this.startCoord = value;
			this.widget.MarkAsChanged();
		}
	}

	public Vector2 EndCoord
	{
		get
		{
			return this.endCoord;
		}
		set
		{
			if (this.endCoord == value)
			{
				return;
			}
			this.endCoord = value;
			this.widget.MarkAsChanged();
		}
	}

	private void GetMinMax(List<Vector3> verts, out Vector2 min, out Vector2 max)
	{
		if (verts.Count == 0)
		{
			min = new Vector2(0f, 0f);
			max = new Vector2(0f, 0f);
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

	private void GetMinMaxUV(List<Vector2> uvs, out Vector2 min, out Vector2 max)
	{
		if (uvs.Count == 0)
		{
			min = new Vector2(0f, 0f);
			max = new Vector2(0f, 0f);
			return;
		}
		min = new Vector2(uvs[0].x, uvs[0].y);
		max = min;
		for (int i = 1; i < uvs.Count; i++)
		{
			if (uvs[i].x < min.x)
			{
				min.x = uvs[i].x;
			}
			if (uvs[i].y < min.y)
			{
				min.y = uvs[i].y;
			}
			if (uvs[i].x > max.x)
			{
				max.x = uvs[i].x;
			}
			if (uvs[i].y > max.y)
			{
				max.y = uvs[i].y;
			}
		}
	}

	private void OnPostFill(MeshData mesh)
	{
		Vector2 vector;
		Vector2 vector2;
		this.GetMinMax(mesh.verts, out vector, out vector2);
		Vector2 vector3;
		Vector2 vector4;
		this.GetMinMaxUV(mesh.uvs, out vector3, out vector4);
		Vector2 vector5 = new Vector2(vector3.x + (vector4.x - vector3.x) * this.startCoord.x, vector3.y + (vector4.y - vector3.y) * this.startCoord.y);
		Vector2 vector6 = new Vector2(vector3.x + (vector4.x - vector3.x) * this.endCoord.x, vector3.y + (vector4.y - vector3.y) * this.endCoord.y);
		for (int i = 0; i < mesh.verts.Count; i++)
		{
			Vector3 vector7 = mesh.verts[i];
			float t = (vector7.x - vector.x) / (vector2.x - vector.x);
			float t2 = (vector7.y - vector.y) / (vector2.y - vector.y);
			mesh.uvs[i] = new Vector2(Mathf.Lerp(vector5.x, vector6.x, t), Mathf.Lerp(vector5.y, vector6.y, t2));
		}
	}

	private UIWidget widget;

	[SerializeField]
	private Vector2 startCoord = new Vector2(0f, 0f);

	[SerializeField]
	private Vector2 endCoord = new Vector2(1f, 1f);
}
