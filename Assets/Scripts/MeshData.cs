using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public List<int> indices
	{
		get
		{
			return this.GetIndices(0);
		}
	}

	public bool IsEmpty()
	{
		return this.verts.Count == 0;
	}

	public void Clear()
	{
		this.numSubmeshes = 1;
		for (int i = 0; i < this.indicesList.Count; i++)
		{
			this.indicesList[i].Clear();
		}
		this.verts.Clear();
		this.uvs.Clear();
		this.colors.Clear();
	}

	public List<int> GetIndices(int subMesh)
	{
		while (subMesh >= this.indicesList.Count)
		{
			this.indicesList.Add(new List<int>(1024));
		}
		return this.indicesList[subMesh];
	}

	public void AddTrianglesForQuad(int subMesh)
	{
		List<int> indices = this.GetIndices(subMesh);
		int num = this.verts.Count - 4;
		indices.Add(num);
		indices.Add(num + 1);
		indices.Add(num + 2);
		indices.Add(num + 2);
		indices.Add(num + 3);
		indices.Add(num);
	}

	public void AddTrianglesForQuads(int amount, int subMesh)
	{
		List<int> indices = this.GetIndices(subMesh);
		for (int i = this.verts.Count - 4 * amount; i < this.verts.Count; i += 4)
		{
			indices.Add(i);
			indices.Add(i + 1);
			indices.Add(i + 2);
			indices.Add(i + 2);
			indices.Add(i + 3);
			indices.Add(i);
		}
	}

	public void AddTrisAutomatically()
	{
		if (this.indices.Count == 0)
		{
			for (int i = 0; i < this.verts.Count; i += 4)
			{
				this.indices.Add(i);
				this.indices.Add(i + 1);
				this.indices.Add(i + 2);
				this.indices.Add(i + 2);
				this.indices.Add(i + 3);
				this.indices.Add(i);
			}
		}
	}

	public int AddQuad(Vector3 pos, Vector2 size, Rect texCoords, Color color)
	{
		int count = this.verts.Count;
		for (int i = 0; i < MeshData.quadCorners.Length; i++)
		{
			this.verts.Add(pos + new Vector3(MeshData.quadCorners[i].x * size.x, MeshData.quadCorners[i].y * size.y, MeshData.quadCorners[i].z));
			this.colors.Add(color);
		}
		this.uvs.Add(texCoords.min);
		this.uvs.Add(new Vector2(texCoords.xMax, texCoords.yMin));
		this.uvs.Add(texCoords.max);
		this.uvs.Add(new Vector2(texCoords.xMin, texCoords.yMax));
		int[] array = MeshData.quadTriangles[0];
		for (int j = 0; j < array.Length; j++)
		{
			this.indices.Add(count + array[j]);
		}
		return count;
	}

	public void ApplyToMesh(Mesh m, Vector3 offset)
	{
		if (this.verts.Count == 0)
		{
			m.Clear();
			return;
		}
		if (offset != Vector3.zero)
		{
			for (int i = 0; i < this.verts.Count; i++)
			{
				List<Vector3> list;
				int index;
				(list = this.verts)[index = i] = list[index] + offset;
			}
		}
		this.AddTrisAutomatically();
		m.Clear();
		m.SetVertices(this.verts);
		m.SetColors(this.colors);
		m.SetUVs(0, this.uvs);
		m.subMeshCount = this.numSubmeshes;
		for (int j = 0; j < this.numSubmeshes; j++)
		{
			m.SetTriangles(this.GetIndices(j), j);
		}
	}

	private const int initialCapacity = 1024;

	public int numSubmeshes = 1;

	public List<Vector3> verts = new List<Vector3>(1024);

	public List<Vector2> uvs = new List<Vector2>(1024);

	public List<Color> colors = new List<Color>(1024);

	private List<List<int>> indicesList = new List<List<int>>(1);

	private static Vector3[] quadCorners = new Vector3[]
	{
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0f)
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
