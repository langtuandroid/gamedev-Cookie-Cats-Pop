using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
	public MeshBuilder()
	{
		this.vertices = new List<Vector3>();
		this.uvs = new List<Vector2>();
		this.colors = new List<Color>();
		this.triangles = new List<int>();
	}

	public void Clear()
	{
		this.vertices.Clear();
		this.uvs.Clear();
		this.colors.Clear();
		this.triangles.Clear();
	}

	public void Combine(MeshBuilder source)
	{
		int count = this.vertices.Count;
		this.vertices.AddRange(source.vertices);
		this.uvs.AddRange(source.uvs);
		this.colors.AddRange(source.colors);
		for (int i = 0; i < source.triangles.Count; i++)
		{
			this.triangles.Add(count + source.triangles[i]);
		}
	}

	public void Combine(params MeshBuilder[] sources)
	{
		for (int i = 0; i < sources.Length; i++)
		{
			this.Combine(sources[i]);
		}
	}

	public void ApplyToMesh(Mesh mesh)
	{
		mesh.vertices = this.vertices.ToArray();
		mesh.uv = this.uvs.ToArray();
		mesh.colors = this.colors.ToArray();
		mesh.triangles = this.triangles.ToArray();
	}

	public void AddQuad(ref Matrix4x4 world, Vector2 size, Rect textureCoords, Color color, bool flip = false)
	{
		int count = this.vertices.Count;
		for (int i = 0; i < MeshBuilder.quadCorners.Length; i++)
		{
			this.vertices.Add(world.MultiplyPoint(new Vector3(MeshBuilder.quadCorners[i].x * size.x, MeshBuilder.quadCorners[i].y * size.y, MeshBuilder.quadCorners[i].z)));
			this.colors.Add(color);
		}
		this.uvs.Add(textureCoords.min);
		this.uvs.Add(new Vector2(textureCoords.xMax, textureCoords.yMin));
		this.uvs.Add(textureCoords.max);
		this.uvs.Add(new Vector2(textureCoords.xMin, textureCoords.yMax));
		int[] array = MeshBuilder.quadTriangles[(!flip) ? 0 : 1];
		for (int j = 0; j < array.Length; j++)
		{
			this.triangles.Add(count + array[j]);
		}
	}

	public void TransformVertices(ref Matrix4x4 world)
	{
		for (int i = 0; i < this.vertices.Count; i++)
		{
			this.vertices[i] = world.MultiplyPoint(this.vertices[i]);
		}
	}

	public void AddInsetQuad(ref Matrix4x4 world, Vector2 size, Rect textureCoords, Color color, Color edgeColor, float egdeWidth, bool flip = false)
	{
		int count = this.vertices.Count;
		for (int i = 0; i < MeshBuilder.quadCorners.Length; i++)
		{
			this.vertices.Add(world.MultiplyPoint(new Vector3(MeshBuilder.quadCorners[i].x * size.x, MeshBuilder.quadCorners[i].y * size.y, MeshBuilder.quadCorners[i].z)));
			this.colors.Add(edgeColor);
		}
		for (int j = 0; j < MeshBuilder.quadCorners.Length; j++)
		{
			this.vertices.Add(world.MultiplyPoint(new Vector3(MeshBuilder.quadCorners[j].x * size.x * egdeWidth, MeshBuilder.quadCorners[j].y * size.y * egdeWidth, MeshBuilder.quadCorners[j].z)));
			this.colors.Add(color);
		}
		for (int k = 0; k < 2; k++)
		{
			this.uvs.Add(textureCoords.min);
			this.uvs.Add(new Vector2(textureCoords.xMax, textureCoords.yMin));
			this.uvs.Add(textureCoords.max);
			this.uvs.Add(new Vector2(textureCoords.xMin, textureCoords.yMax));
		}
		int[] array = new int[]
		{
			0,
			4,
			5,
			0,
			5,
			1,
			1,
			5,
			6,
			1,
			6,
			2,
			2,
			6,
			7,
			2,
			7,
			3,
			3,
			7,
			4,
			3,
			4,
			0,
			4,
			7,
			6,
			4,
			6,
			5
		};
		int[] array2 = new int[]
		{
			0,
			5,
			4,
			0,
			1,
			5,
			1,
			6,
			5,
			1,
			2,
			6,
			2,
			7,
			6,
			2,
			3,
			7,
			3,
			4,
			7,
			3,
			0,
			4,
			4,
			6,
			7,
			4,
			5,
			6
		};
		for (int l = 0; l < array.Length; l++)
		{
			this.triangles.Add(count + (flip ? array2[l] : array[l]));
		}
	}

	public List<Vector3> vertices;

	public List<Vector2> uvs;

	public List<Color> colors;

	public List<int> triangles;

	private static Vector3[] quadCorners = new Vector3[]
	{
		new Vector3(-1f, -1f, 0f),
		new Vector3(1f, -1f, 0f),
		new Vector3(1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f)
	};

	private static int[][] quadTriangles = new int[][]
	{
		new int[]
		{
			0,
			2,
			1,
			0,
			3,
			2
		},
		new int[]
		{
			0,
			1,
			2,
			0,
			2,
			3
		}
	};
}
