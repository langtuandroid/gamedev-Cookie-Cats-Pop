using System;
using UnityEngine;

public class SkyBackground : MonoBehaviour
{
	public void Initialize(Transform parallaxTransform)
	{
		this.parallaxTransform = parallaxTransform;
		this.emptyAreaTransform = this.emptyArea.transform;
		Vector3 b = new Vector3(0f, -this.emptyArea.Size.y, 0f);
		foreach (SkyBackground.Layer layer in this.layers)
		{
			layer.source = layer.transform.localPosition;
			layer.dest = layer.source + b;
		}
		this.Update();
	}

	private void Animate(float t)
	{
		for (int i = 0; i < this.layers.Length; i++)
		{
			this.layers[i].Interpolate(t);
		}
	}

	private void Update()
	{
		if (this.parallaxTransform == null)
		{
			return;
		}
		float num = (this.emptyAreaTransform.InverseTransformPoint(this.parallaxTransform.position).y + this.emptyArea.Size.y * 0.5f) / this.emptyArea.Size.y;
		this.Animate(1f - num);
	}

	public UIElement emptyArea;

	public SkyBackground.Layer[] layers;

	private Transform parallaxTransform;

	private Transform emptyAreaTransform;

	[Serializable]
	public class Layer
	{
		public void Interpolate(float t)
		{
			this.transform.localPosition = FiberAnimation.LerpNoClamp(this.source, this.dest, t * this.moveFactor);
		}

		public Transform transform;

		public float moveFactor;

		[NonSerialized]
		public Vector3 source;

		[NonSerialized]
		public Vector3 dest;
	}
}
