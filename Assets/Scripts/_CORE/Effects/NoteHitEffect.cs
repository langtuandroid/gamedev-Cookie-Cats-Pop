using System;
using System.Collections;
using UnityEngine;

public class NoteHitEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		yield return null;
		if (this.noteMeshWidth < 140f)
		{
			this.flashEffect.Play();
		}
		this.bubble.SetActive(false);
		if (this.noteMeshNodesWorldPositions == null || this.noteMeshNodesWorldPositions.Length == 0)
		{
			yield break;
		}
		this.notesMesh.width = this.noteMeshWidth;
		this.notesMesh.endU = 0.0848f * this.noteMeshWidth;
		this.animatingNotesMesh.numberOfNotes = this.numberOfNotes;
		this.animatingNotesMesh.numberOfNotesNextToEachOther = Mathf.Max(1, Mathf.FloorToInt(0.02f * this.noteMeshWidth));
		this.notesMesh.nodePositions = new Vector3[this.noteMeshNodesWorldPositions.Length];
		for (int i = 0; i < this.noteMeshNodesWorldPositions.Length; i++)
		{
			this.notesMesh.nodePositions[i] = base.transform.InverseTransformPoint(this.noteMeshNodesWorldPositions[i]);
		}
		this.notesMesh.position = 0f;
		this.notesMesh.length = 0f;
		this.animatingNotesMesh.position = this.notesMesh.position;
		this.animatingNotesMesh.length = this.notesMesh.length;
		this.animatingNotesMesh.UpdateMesh();
		this.notesMesh.UpdateMesh();
		this.animatingNotesMesh.UpdateMesh();
		yield return FiberAnimation.Animate(1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			this.notesMesh.position = 0f;
			this.notesMesh.length = t;
			this.animatingNotesMesh.position = this.notesMesh.position;
			this.animatingNotesMesh.length = this.notesMesh.length;
		}, false);
		yield return FiberAnimation.Animate(1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			this.notesMesh.position = t;
			this.notesMesh.length = 1f - t;
			this.animatingNotesMesh.position = this.notesMesh.position;
			this.animatingNotesMesh.length = this.notesMesh.length;
		}, false);
		yield break;
	}

	public NotesMesh notesMesh;

	public AnimatingNotesMesh animatingNotesMesh;

	public ParticleSystem flashEffect;

	public GameObject bubble;

	[NonSerialized]
	public Vector3[] noteMeshNodesWorldPositions;

	[NonSerialized]
	public float noteMeshWidth;

	[NonSerialized]
	public int numberOfNotes;
}
