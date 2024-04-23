using System;
using UnityEngine;

namespace NinjaUI
{
	public class ColorPulsator : MonoBehaviour
	{
		private void OnEnable()
		{
			this.widget = base.GetComponent<UIWidget>();
			this.startColor = this.widget.Color;
			this.colorA = this.startColor;
			this.elapsed = 0f;
		}

		private void OnDisable()
		{
			this.widget.Color = this.startColor;
		}

		private void Update()
		{
			float num = Mathf.Sin((this.elapsed * this.speed + this.phase) * 3.14159274f * 2f) * 0.5f + 0.5f;
			this.elapsed += Time.deltaTime;
			num = Mathf.Pow(num, this.bias);
			this.widget.Color = Color.Lerp(this.colorA, this.colorB, num);
		}

		public float speed = 1f;

		public float bias = 1f;

		public float phase;

		private float elapsed;

		private Vector3 originalScale;

		public Color colorA = Color.white;

		public Color colorB = new Color(1f, 1f, 1f, 0f);

		private UIWidget widget;

		private Color startColor;
	}
}
