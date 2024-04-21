using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimatingNotesMesh : MonoBehaviour
{
	private void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshBuilder = new MeshBuilder();
		this.mesh = new Mesh();
		this.notesMesh.MeshUpdated = delegate(Spline2 l, Spline2 r)
		{
			this.left = l;
			this.right = r;
		};
		this.length = 0f;
	}

	private void OnDestroy()
	{
		if (this.mesh != null)
		{
			UnityEngine.Object.Destroy(this.mesh);
		}
	}

	private void Update()
	{
		this.UpdateMesh();
		this.phase += this.phaseSpeed * Time.deltaTime;
	}

	public void UpdateMesh()
	{
		if (this.notesMesh == null)
		{
			return;
		}
		this.notesMesh.MeshUpdated = delegate(Spline2 l, Spline2 r)
		{
			this.left = l;
			this.right = r;
		};
		if (this.left != null)
		{
			if (this.notes == null || this.numberOfNotes * this.numberOfNotesNextToEachOther != this.notes.Count)
			{
				if (this.notes != null)
				{
					this.notes.Clear();
				}
				this.CreateNotes(this.numberOfNotes);
			}
			this.CreateMesh();
		}
	}

	public void CreateNotes(int count)
	{
		float num = 1f / (float)count;
		float num2 = 1f / (float)this.numberOfNotesNextToEachOther;
		this.notes = new List<AnimatingNotesMesh.Note>();
		for (int i = 0; i < count; i++)
		{
			for (int j = 0; j < this.numberOfNotesNextToEachOther; j++)
			{
				this.notes.Add(this.CreateRandom((float)i * num + UnityEngine.Random.Range(-num * 0.25f, num * 0.25f), (float)j * num2 + num2 * 0.5f));
			}
		}
	}

	private AnimatingNotesMesh.Note CreateRandom(float pos, float yPos)
	{
		return new AnimatingNotesMesh.Note
		{
			type = UnityEngine.Random.Range(0, 4),
			position = new Vector2(pos, yPos),
			velocity = new Vector2(1f, 0f),
			rotation = UnityEngine.Random.Range(-30f, 30f)
		};
	}

	private void CreateMesh()
	{
		if (this.notes == null)
		{
			this.CreateNotes(this.numberOfNotes);
		}
		if (this.meshBuilder == null)
		{
			this.meshBuilder = new MeshBuilder();
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
		}
		this.meshBuilder.Clear();
		float fullLength = this.left.FullLength;
		float fullLength2 = this.right.FullLength;
		if (this.length > 0.0001f)
		{
			for (int i = 0; i < this.notes.Count; i++)
			{
				AnimatingNotesMesh.Note note = this.notes[i];
				float num = note.position.x + this.phase * 0.1f;
				num = Mathf.Repeat(num, 1f);
				Vector2 v = this.left.Evaluate(num * fullLength);
				Vector2 v2 = this.right.Evaluate(num * fullLength2);
				Vector2 v3 = Vector3.Lerp(v, v2, note.position.y);
				this.world = Matrix4x4.TRS(v3, Quaternion.Euler(0f, 0f, note.rotation), Vector3.one * this.noteSize);
				float time = (num - this.position) / this.length;
				float a = this.alphaCurve.Evaluate(time);
				this.meshBuilder.AddQuad(ref this.world, Vector2.one, AnimatingNotesMesh.texCoords[note.type], new Color(1f, 1f, 1f, a), false);
			}
		}
		this.mesh.Clear();
		this.meshBuilder.ApplyToMesh(this.mesh);
		this.meshFilter.mesh = this.mesh;
	}

	public NotesMesh notesMesh;

	public float position;

	public float length;

	public float phase;

	public float phaseSpeed = 0.01f;

	public int numberOfNotes = 30;

	public int numberOfNotesNextToEachOther = 1;

	public float noteSize = 30f;

	public AnimationCurve alphaCurve;

	private MeshBuilder meshBuilder;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private Spline2 left;

	private Spline2 right;

	private Matrix4x4 world;

	private List<AnimatingNotesMesh.Note> notes;

	private static Rect[] texCoords = new Rect[]
	{
		new Rect(0f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0f, 0.5f, 0.5f),
		new Rect(0.5f, 0.5f, 0.5f, 0.5f),
		new Rect(0f, 0.5f, 0.5f, 0.5f)
	};

	private class Note
	{
		public int type;

		public Vector2 position;

		public Vector2 velocity;

		public float rotation;
	}
}
