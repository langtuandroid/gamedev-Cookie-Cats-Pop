using System;
using UnityEngine;

public class MovementAnimation : MonoBehaviour
{
	private void OnEnable()
	{
		this.spline = new Spline2();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.spline.Nodes.Add(this.nodes[i].localPosition);
		}
	}

	private void Update()
	{
		Vector2 vector = this.spline.Evaluate(this.spline.FullLength * this.curve.Evaluate(this.progress + this.offset));
		Vector3 localPosition = new Vector3(vector.x, vector.y, base.transform.localPosition.z);
		localPosition.y += this.yMovementOffset;
		base.transform.localPosition = localPosition;
		this.progress += Time.deltaTime * this.speed;
	}

	public AnimationCurve curve;

	public float speed = 1f;

	public float offset;

	public Transform[] nodes;

	public float yMovementOffset;

	private Spline2 spline;

	private float progress;

	private float delayProgress;

	private float yMovementProgress;
}
