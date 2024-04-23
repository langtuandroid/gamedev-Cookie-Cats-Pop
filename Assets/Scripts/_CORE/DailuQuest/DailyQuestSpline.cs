using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DailyQuestSpline : MonoBehaviour
{
	public void CalculateSpline()
	{
		if (this.spline == null)
		{
			this.spline = new DailyQuestSplineInternal();
		}
		this.spline.Nodes.Clear();
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				DailyQuestSplineNode component = transform.gameObject.GetComponent<DailyQuestSplineNode>();
				if (component != null && (!this.ignoreInactiveNodes || transform.gameObject.activeInHierarchy))
				{
					this.spline.Nodes.Add(base.transform.InverseTransformPoint(transform.position));
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		this.spline.MakeDirty();
		this.spline.CalculateSpline();
	}

	public void UpdateMesh()
	{
		if (this.linear)
		{
			this.CreateLinearMesh();
		}
		else
		{
			this.CreateSplineMesh();
		}
	}

	private void CreateLinearMesh()
	{
		if (this.spline == null)
		{
			this.CalculateSpline();
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
			this.vertices = new List<Vector3>();
			this.triangles = new List<int>();
			this.colors = new List<Color>();
			this.uvs = new List<Vector2>();
		}
		this.vertices.Clear();
		this.triangles.Clear();
		this.colors.Clear();
		this.uvs.Clear();
		for (int i = 0; i < this.spline.Nodes.Count - 1; i++)
		{
			Vector2 vector = this.spline.Nodes[i];
			Vector2 a = this.spline.Nodes[i + 1];
			Vector2 vector2 = a - vector;
			vector2.Normalize();
			Vector2 a2 = new Vector2(-vector2.y, vector2.x);
			Vector2 vector3 = vector + a2 * this.width;
			Vector2 vector4 = vector - a2 * this.width;
			Vector2 vector5 = a - a2 * this.width;
			Vector2 vector6 = a + a2 * this.width;
			int count = this.vertices.Count;
			this.vertices.Add(new Vector3(vector3.x, vector3.y, 0.1f));
			this.vertices.Add(new Vector3(vector4.x, vector4.y, 0.1f));
			this.vertices.Add(new Vector3(vector5.x, vector5.y, 0.1f));
			this.vertices.Add(new Vector3(vector6.x, vector6.y, 0.1f));
			this.colors.Add(this.color);
			this.colors.Add(this.color);
			this.colors.Add(this.color);
			this.colors.Add(this.color);
			float num = 0.05f;
			if (this.flipUVOnOppositeX && a.x > vector.x)
			{
				num = 1f - num;
			}
			this.uvs.Add(new Vector2(num, 0f));
			this.uvs.Add(new Vector2(1f - num, 0f));
			this.uvs.Add(new Vector2(1f - num, 1f));
			this.uvs.Add(new Vector2(num, 11f));
			this.triangles.Add(count);
			this.triangles.Add(count + 3);
			this.triangles.Add(count + 2);
			this.triangles.Add(count);
			this.triangles.Add(count + 2);
			this.triangles.Add(count + 1);
		}
		this.mesh.Clear();
		this.mesh.vertices = this.vertices.ToArray();
		this.mesh.colors = this.colors.ToArray();
		this.mesh.uv = this.uvs.ToArray();
		this.mesh.triangles = this.triangles.ToArray();
		this.mesh.RecalculateBounds();
		this.meshFilter.mesh = this.mesh;
	}

	private void CreateSplineMesh()
	{
		if (this.segments < 1)
		{
			return;
		}
		if (this.spline == null)
		{
			this.CalculateSpline();
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
			this.vertices = new List<Vector3>();
			this.triangles = new List<int>();
			this.colors = new List<Color>();
			this.uvs = new List<Vector2>();
		}
		this.vertices.Clear();
		this.triangles.Clear();
		this.colors.Clear();
		this.uvs.Clear();
		float startMargin = this.StartMargin;
		float num = this.spline.FullLength - this.EndMargin;
		float num2 = (num - startMargin) / (float)this.segments;
		for (int i = 0; i <= this.segments; i++)
		{
			float num3 = startMargin + (float)i * num2;
			Vector2 position = this.GetPosition(num3);
			Vector2 position2 = this.GetPosition(num3 + 0.01f);
			Vector2 vector = position2 - position;
			vector.Normalize();
			Vector2 a = new Vector2(-vector.y, vector.x);
			Vector2 vector2 = position + a * this.width;
			Vector2 vector3 = position - a * this.width;
			int count = this.vertices.Count;
			this.vertices.Add(new Vector3(vector2.x, vector2.y, 0.1f));
			this.vertices.Add(new Vector3(vector3.x, vector3.y, 0.1f));
			this.colors.Add(this.color);
			this.colors.Add(this.color);
			float y = (float)i / (float)this.segments;
			this.uvs.Add(new Vector2(0f, y));
			this.uvs.Add(new Vector2(1f, y));
			if (i < this.segments)
			{
				this.triangles.Add(count);
				this.triangles.Add(count + 3);
				this.triangles.Add(count + 1);
				this.triangles.Add(count);
				this.triangles.Add(count + 2);
				this.triangles.Add(count + 3);
			}
		}
		this.mesh.Clear();
		this.mesh.vertices = this.vertices.ToArray();
		this.mesh.colors = this.colors.ToArray();
		this.mesh.uv = this.uvs.ToArray();
		this.mesh.triangles = this.triangles.ToArray();
		this.mesh.RecalculateBounds();
		this.meshFilter.mesh = this.mesh;
	}

	public Vector2 GetPosition(float progress)
	{
		return this.spline.Evaluate(progress);
	}

	public float GetLength()
	{
		return this.spline.FullLength;
	}

	public float GetRealLength()
	{
		return this.spline.RealLength;
	}

	public void CreateNodes(GameObject prefab, int amount)
	{
		if (this.nodes == null)
		{
			this.nodes = new List<DailyQuestSplineNode>();
		}
		for (int i = 0; i < this.nodes.Count; i++)
		{
			if (this.nodes[i] != null)
			{
				UnityEngine.Object.Destroy(this.nodes[i]);
			}
		}
		this.nodes.Clear();
		for (int j = 0; j < amount; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			gameObject.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable);
			gameObject.transform.parent = base.transform;
			DailyQuestSplineNode component = gameObject.GetComponent<DailyQuestSplineNode>();
			this.nodes.Add(component);
			gameObject.SetActive(j > 0);
		}
	}

	public MeshFilter meshFilter;

	public int segments = 64;

	public float width = 0.08f;

	public float StartMargin = 0.25f;

	public float EndMargin = 0.25f;

	public Color color = new Color(0.5803922f, 0.7647059f, 0.8392157f, 1f);

	public bool linear;

	public bool flipUVOnOppositeX;

	public bool ignoreInactiveNodes;

	private DailyQuestSplineInternal spline;

	private Mesh mesh;

	private List<Vector3> vertices;

	private List<int> triangles;

	private List<Color> colors;

	private List<Vector2> uvs;

	private Dictionary<int, Vector3> oldPositions;

	private int oldSegments;

	private float oldWidth;

	private int oldNodesCount;

	private float oldStartMargin;

	private float oldEndMargin;

	public List<DailyQuestSplineNode> nodes;
}
