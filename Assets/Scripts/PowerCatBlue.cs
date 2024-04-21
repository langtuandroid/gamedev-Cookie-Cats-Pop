using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class PowerCatBlue : PowerCat
{
	protected override void OnInitialize()
	{
		this.notesMesh.position = 0f;
		this.notesMesh.length = 0f;
		this.animatingNotes.position = this.notesMesh.position;
		this.animatingNotes.length = this.notesMesh.length;
		this.musicPivot.SetActive(false);
	}

	protected override IEnumerator ChargedState()
	{
		yield return this.spine.PlayUntilEvent("Eating", "Loop_Start");
		base.ChargedAnimIsLooping = true;
		yield return this.spine.PlayLoopBetweenEvents("Eating", "Loop_Start", "Loop_End", -1f);
		yield break;
	}

	public override IEnumerator AnimatePowerPiece(PieceId pieceId, Transform targetPivot, Action bubbleSpawned)
	{
		if (this.currentState != PowerCat.State.Charged)
		{
			float endTime = 2f;
			yield return this.spine.PlayTimeline("Eating", 1.66666663f, endTime, endTime - 1.66666663f, 1f, null);
		}
		GameObject bubble = UnityEngine.Object.Instantiate<GameObject>(this.bubblePivot);
		bubble.transform.parent = targetPivot;
		bubble.transform.localPosition = Vector3.zero;
		bubble.transform.localScale = Vector3.one;
		bubble.SetActive(false);
		this.notesMeshEndPivot.position = targetPivot.position;
		base.ScheduleIdle(5.6f);
		this.spine.PlayAnimation(0, "Power", false, false);
		yield return FiberHelper.Wait(1.2f, (FiberHelper.WaitFlag)0);
		this.musicPivot.SetActive(true);
		yield return new Fiber.OnExit(delegate()
		{
			this.notesMesh.position = 0f;
			this.notesMesh.length = 0f;
			this.animatingNotes.position = this.notesMesh.position;
			this.animatingNotes.length = this.notesMesh.length;
			this.animatingNotes.UpdateMesh();
			this.notesMesh.UpdateMesh();
			this.animatingNotes.UpdateMesh();
			this.musicPivot.SetActive(false);
		});
		this.notesMesh.position = 0f;
		this.notesMesh.length = 0f;
		this.animatingNotes.position = this.notesMesh.position;
		this.animatingNotes.length = this.notesMesh.length;
		this.animatingNotes.UpdateMesh();
		this.notesMesh.UpdateMesh();
		this.animatingNotes.UpdateMesh();
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberAnimation.Animate(1f, SingletonAsset<PowerVisualSettings>.Instance.notesExpandCurve, delegate(float t)
				{
					this.notesMesh.position = 0f;
					this.notesMesh.length = t;
					this.animatingNotes.position = this.notesMesh.position;
					this.animatingNotes.length = this.notesMesh.length;
				}, false),
				FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0),
				FiberAnimation.Animate(1f, SingletonAsset<PowerVisualSettings>.Instance.notesExpandCurve, delegate(float t)
				{
					this.notesMesh.position = t;
					this.notesMesh.length = 1f - t;
					this.animatingNotes.position = this.notesMesh.position;
					this.animatingNotes.length = this.notesMesh.length;
				}, false)
			}),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.RunDelayed(0.7f, delegate
				{
					bubble.SetActive(true);
					bubbleSpawned();
				}),
				FiberAnimation.ScaleTransform(bubble.transform, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.bluebubbleExpandCurve, 3f)
			})
		});
		this.musicPivot.SetActive(false);
		yield break;
	}

	public NotesMesh notesMesh;

	public AnimatingNotesMesh animatingNotes;

	public GameObject musicPivot;

	public GameObject bubblePivot;

	public Transform notesMeshEndPivot;
}
