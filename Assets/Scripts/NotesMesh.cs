using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class NotesMesh : MonoBehaviour
{
	private void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshBuilder = new MeshBuilder();
		this.mesh = new Mesh();
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.mesh);
	}

	private void CalculateSplines(Vector3[] nodes, float width, out Spline2 left, out Spline2 right)
	{
		left = new Spline2();
		right = new Spline2();
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		for (int i = 0; i < nodes.Length; i++)
		{
			Vector3 a = default(Vector3);
			Vector3 vector = default(Vector3);
			if (i == 0)
			{
				vector = nodes[i + 1] - nodes[i];
			}
			else if (i == nodes.Length - 1)
			{
				vector = nodes[i] - nodes[i - 1];
			}
			else
			{
				Vector3 vector2 = nodes[i] - nodes[i - 1];
				Vector3 vector3 = nodes[i + 1] - nodes[i];
				vector = (vector2.normalized + vector3.normalized).normalized;
			}
			a = new Vector3(vector.y, -vector.x, 0f);
			a.Normalize();
			list.Add(nodes[i]);
			list2.Add(nodes[i] + a * width);
			list3.Add(nodes[i] - a * width);
			left.Nodes.Add(nodes[i] + a * width);
			right.Nodes.Add(nodes[i] - a * width);
		}
		left.CalculateSpline();
		right.CalculateSpline();
	}

	private void Update()
	{
		if (this.update)
		{
			this.UpdateMesh();
		}
	}

	public void UpdateMesh()
	{
		if (this.nodePositions != null && this.nodePositions.Length > 1)
		{
			this.CreateSplineMesh(this.nodePositions);
		}
		else if (this.sourcePositions != null && this.sourcePositions.Length > 1)
		{
			Vector3[] array = new Vector3[this.sourcePositions.Length];
			for (int i = 0; i < this.sourcePositions.Length; i++)
			{
				array[i] = base.transform.InverseTransformPoint(this.sourcePositions[i].transform.position);
			}
			this.CreateSplineMesh(array);
		}
	}

	public void CreateSplineMesh(Vector3[] controlPoints)
	{
		Spline2 spline;
		Spline2 spline2;
		this.CalculateSplines(controlPoints, this.width, out spline, out spline2);
		this.CreateMesh(this.length, spline, spline2);
		if (this.MeshUpdated != null)
		{
			this.MeshUpdated(spline, spline2);
		}
	}

	private void CreateMesh(float normalizedLength, Spline2 left, Spline2 right)
	{
		if (this.meshBuilder == null)
		{
			this.meshBuilder = new MeshBuilder();
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
		}
		this.meshBuilder.Clear();
		float fullLength = left.FullLength;
		float num = left.FullLength * normalizedLength;
		float fullLength2 = right.FullLength;
		float b = right.FullLength * normalizedLength;
		int num2 = Mathf.CeilToInt(Mathf.Max(num, b) / this.segmentEvery);
		if (num2 > 0)
		{
			float num3 = 0f;
			float num4 = num / (float)num2;
			for (int i = 0; i <= num2; i++)
			{
				float num5 = fullLength * this.position + num4 * (float)i;
				float num6 = num5 * (fullLength2 / fullLength);
				Vector2 v = left.Evaluate(num5);
				Vector2 v2 = right.Evaluate(num6);
				int count = this.meshBuilder.vertices.Count;
				this.meshBuilder.vertices.Add(v);
				this.meshBuilder.vertices.Add(v);
				this.meshBuilder.vertices.Add(v2);
				this.meshBuilder.vertices.Add(v2);
				float a = this.alphaCurve.Evaluate((float)i / (float)num2);
				this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, 0f));
				this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, a));
				this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, a));
				this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, 0f));
				this.meshBuilder.uvs.Add(new Vector2(this.startU, num3));
				this.meshBuilder.uvs.Add(new Vector2(this.startU, num3));
				this.meshBuilder.uvs.Add(new Vector2(this.endU, num3));
				this.meshBuilder.uvs.Add(new Vector2(this.endU, num3));
				if (i < num2)
				{
					for (int j = 0; j < NotesMesh.triangles.Length; j++)
					{
						this.meshBuilder.triangles.Add(count + NotesMesh.triangles[j]);
					}
				}
				num3 += 1f;
			}
		}
		this.mesh.Clear();
		this.meshBuilder.ApplyToMesh(this.mesh);
		this.meshFilter.mesh = this.mesh;
	}

	private void CreateMeshFromPoints(Mesh mesh, List<Vector3> positions, List<Vector2> normals, float width, float edgeWidth, float startV, float vMultiplier = 1f)
	{
		if (this.meshBuilder == null)
		{
			this.meshBuilder = new MeshBuilder();
		}
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		this.meshBuilder.Clear();
		float d = width + edgeWidth;
		float num = startV;
		for (int i = 0; i < positions.Count; i++)
		{
			Vector3 vector;
			if (i == positions.Count - 1)
			{
				vector = positions[i] - positions[i - 1];
			}
			else
			{
				vector = positions[i + 1] - positions[i];
			}
			float magnitude = vector.magnitude;
			vector.x /= magnitude;
			vector.y /= magnitude;
			vector.z /= magnitude;
			Vector3 a = new Vector3(-vector.y, vector.x, 0f);
			int count = this.meshBuilder.vertices.Count;
			this.meshBuilder.vertices.Add(positions[i] - a * d);
			this.meshBuilder.vertices.Add(positions[i] - a * width);
			this.meshBuilder.vertices.Add(positions[i] + a * width);
			this.meshBuilder.vertices.Add(positions[i] + a * d);
			float a2 = this.alphaCurve.Evaluate((float)i / (float)(positions.Count - 1));
			this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, 0f));
			this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, a2));
			this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, a2));
			this.meshBuilder.colors.Add(new Color(1f, 1f, 1f, 0f));
			this.meshBuilder.uvs.Add(new Vector2(this.startU, num));
			this.meshBuilder.uvs.Add(new Vector2(this.startU, num));
			this.meshBuilder.uvs.Add(new Vector2(this.endU, num));
			this.meshBuilder.uvs.Add(new Vector2(this.endU, num));
			if (i < positions.Count - 1)
			{
				for (int j = 0; j < NotesMesh.triangles.Length; j++)
				{
					this.meshBuilder.triangles.Add(count + NotesMesh.triangles[j]);
				}
			}
			num += this.length * vMultiplier;
		}
		mesh.Clear();
		this.meshBuilder.ApplyToMesh(mesh);
		this.meshFilter.mesh = mesh;
	}

	private void CreateLine(MeshBuilder meshBuilder, Vector3 start, Vector3 end, float width, float edgeWidth, Vector3? prevVector, Vector3? nextVector, bool negateNormals)
	{
		Vector3 a = end - start;
		a.Normalize();
		Vector3 vector = new Vector3(-a.y, a.x, 0f);
		float d = width + edgeWidth;
		int count = meshBuilder.vertices.Count;
		Vector3 a2 = negateNormals ? vector : (-vector);
		if (prevVector != null)
		{
			a2 = Vector3.Normalize(-a + prevVector.Value);
		}
		Vector3 a3 = negateNormals ? vector : (-vector);
		if (nextVector != null)
		{
			a3 = -Vector3.Normalize(a + nextVector.Value);
		}
		meshBuilder.vertices.Add(start - a2 * width);
		meshBuilder.vertices.Add(start + a2 * width);
		meshBuilder.vertices.Add(end + a3 * width);
		meshBuilder.vertices.Add(end - a3 * width);
		meshBuilder.vertices.Add(start + a2 * width);
		meshBuilder.vertices.Add(start + a2 * d);
		meshBuilder.vertices.Add(end + a3 * d);
		meshBuilder.vertices.Add(end + a3 * width);
		meshBuilder.vertices.Add(start - a2 * d);
		meshBuilder.vertices.Add(start - a2 * width);
		meshBuilder.vertices.Add(end - a3 * width);
		meshBuilder.vertices.Add(end - a3 * d);
		for (int i = 0; i < NotesMesh.triangles.Length; i++)
		{
			meshBuilder.triangles.Add(count + NotesMesh.triangles[i]);
		}
		Vector2[] array = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		for (int j = 0; j < 3; j++)
		{
			for (int k = 0; k < array.Length; k++)
			{
				meshBuilder.uvs.Add(array[k]);
			}
		}
		float[] array2 = new float[]
		{
			1f,
			1f,
			1f,
			1f,
			1f,
			0f,
			0f,
			1f,
			0f,
			1f,
			1f,
			0f
		};
		for (int l = 0; l < array2.Length; l++)
		{
			meshBuilder.colors.Add(new Color(1f, 1f, 1f, array2[l]));
		}
	}

	public float width = 4f;

	public float edgeWidth = 4f;

	public float startV;

	public float vMultiplier;

	public float startU;

	public float endU = 3.005f;

	public float segmentEvery = 50f;

	public AnimationCurve alphaCurve;

	public float position;

	public float length;

	public bool update;

	public Transform[] sourcePositions;

	public Vector3[] nodePositions;

	public Action<Spline2, Spline2> MeshUpdated;

	private MeshBuilder meshBuilder;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private static int[] triangles = new int[]
	{
		0,
		4,
		1,
		4,
		5,
		1,
		1,
		5,
		2,
		5,
		6,
		2,
		2,
		6,
		3,
		6,
		7,
		3
	};
}
