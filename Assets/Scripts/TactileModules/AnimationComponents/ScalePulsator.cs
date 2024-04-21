using System;
using UnityEngine;

namespace TactileModules.AnimationComponents
{
	public class ScalePulsator : MonoBehaviour
	{
		private void OnEnable()
		{
			this.originalScale = base.transform.localScale;
			if (this.randomPhase)
			{
				this.randomPhaseInSeconds = UnityEngine.Random.value * 6.28318548f;
			}
		}

		private void OnDisable()
		{
			base.transform.localScale = this.originalScale;
		}

		private void Update()
		{
			float num;
			if (this.timeDomain == ScalePulsator.TimeDomain.TimeSinceLevelLoaded)
			{
				num = Time.timeSinceLevelLoad;
			}
			else
			{
				num = this.elapsed;
				this.elapsed += Time.deltaTime;
			}
			num += this.randomPhaseInSeconds;
			float num2 = Mathf.Sin(num * this.speed * 3.14159274f * 2f) * 0.5f + 0.5f;
			num2 = Mathf.Pow(num2, this.power);
			Vector3 b = Vector3.one + this.scaleInfluence * num2;
			base.transform.localScale = Vector3.Scale(this.originalScale, b);
		}

		[SerializeField]
		private ScalePulsator.TimeDomain timeDomain;

		[SerializeField]
		private float power = 1f;

		[SerializeField]
		private bool randomPhase;

		public float speed = 1f;

		public Vector3 scaleInfluence = new Vector3(0.2f, 0.2f, 0f);

		private float elapsed;

		private float randomPhaseInSeconds;

		private Vector3 originalScale;

		private enum TimeDomain
		{
			TimeSinceEnabled,
			TimeSinceLevelLoaded
		}
	}
}
