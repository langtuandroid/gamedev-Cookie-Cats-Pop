using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class UIGradient : MonoBehaviour
{
	private void Start()
	{
		this.widget = base.GetComponent<UIWidget>();
		this.widget.OnPostFill += this.OnPostFill;
	}

	public Color Color1
	{
		get
		{
			return this.color1;
		}
		set
		{
			if (this.color1 == value)
			{
				return;
			}
			this.color1 = value;
			this.widget.MarkAsChanged();
		}
	}

	public Color Color2
	{
		get
		{
			return this.color2;
		}
		set
		{
			if (this.color2 == value)
			{
				return;
			}
			this.color2 = value;
			this.widget.MarkAsChanged();
		}
	}

	public UIGradient.GradientDirection Direction
	{
		get
		{
			return this.direction;
		}
		set
		{
			if (this.direction == value)
			{
				return;
			}
			this.direction = value;
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

	private void OnPostFill(MeshData mesh)
	{
		Vector2 vector;
		Vector2 vector2;
		this.GetMinMax(mesh.verts, out vector, out vector2);
		if (this.direction == UIGradient.GradientDirection.Vertical)
		{
			for (int i = 0; i < mesh.verts.Count; i++)
			{
				float t = (mesh.verts[i].y - vector.y) / (vector2.y - vector.y);
				mesh.colors[i] = Color.Lerp(this.color2, this.color1, t);
			}
		}
		else
		{
			for (int j = 0; j < mesh.verts.Count; j++)
			{
				float t2 = (mesh.verts[j].x - vector.x) / (vector2.x - vector.x);
				mesh.colors[j] = Color.Lerp(this.color1, this.color2, t2);
			}
		}
	}

	private UIWidget widget;

	[SerializeField]
	private Color color1 = Color.white;

	[SerializeField]
	private Color color2 = Color.white;

	[SerializeField]
	private UIGradient.GradientDirection direction;

	public enum GradientDirection
	{
		Vertical,
		Horizontal
	}
}
