using System;
using System.Collections.Generic;
using UnityEngine;

public class LaneAnimation : MonoBehaviour
{
	[Instantiator.SerializeProperty]
	public bool Flip { get; set; }

	private Vector3 startPosition
	{
		get
		{
			return (!this.Flip) ? this.startTransform.localPosition : this.endTransform.localPosition;
		}
	}

	private Vector3 endPosition
	{
		get
		{
			return (!this.Flip) ? this.endTransform.localPosition : this.startTransform.localPosition;
		}
	}

	private static bool IsFlipped(Vector3 s, Vector3 e)
	{
		return s.x < e.x;
	}

	private void Initialize()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.isInitialized = true;
		this.particleTypesLottery = new Lottery<LaneAnimation.ParticleType>();
		foreach (LaneAnimation.ParticleType particleType in this.particleTypes)
		{
			particleType.Initialize();
			this.particleTypesLottery.Add(particleType.probability, particleType);
			particleType.pivot.SetActive(false);
		}
		this.activeParticles.Clear();
	}

	private void Start()
	{
		this.Initialize();
	}

	private void Update()
	{
		if (this.particleTypes.Length == 0)
		{
			return;
		}
		float magnitude = (this.endPosition - this.startPosition).magnitude;
		if (!this.lastEmitFullyVisible)
		{
			this.lastEmitFullyVisible = true;
			for (int i = 0; i < this.activeParticles.Count; i++)
			{
				if (!this.activeParticles[i].IsFullyVisible(magnitude))
				{
					this.lastEmitFullyVisible = false;
					break;
				}
			}
		}
		else
		{
			if (this.emitImmediately || this.emitProgress > this.nextEmitTime)
			{
				this.emitImmediately = false;
				this.emitProgress = 0f;
				this.nextEmitTime = this.minFrequency + (this.maxFrequency - this.minFrequency) * UnityEngine.Random.value;
				int numberOfParticles = UnityEngine.Random.Range(this.minEmission, this.maxEmission + 1);
				this.lastEmitFullyVisible = false;
				this.Emit(numberOfParticles, magnitude);
			}
			if (this.activeParticles.Count < this.maxParticles)
			{
				this.emitProgress += Time.deltaTime / this.speedScaler;
			}
		}
		this.UpdateParticles(Time.deltaTime * this.speedScaler, magnitude);
	}

	public void Emit(int numberOfParticles, float travelLength)
	{
		float startPosition = 0f;
		float num = float.MaxValue;
		this.typeSpawnedThisEmit.Clear();
		for (int i = 0; i < numberOfParticles; i++)
		{
			LaneAnimation.ParticleType particleType = null;
			for (int j = 1000; j > 0; j--)
			{
				particleType = this.particleTypesLottery.PickRandomItem(false);
				if (!this.typeSpawnedThisEmit.Contains(particleType))
				{
					break;
				}
			}
			if (particleType != null)
			{
				this.typeSpawnedThisEmit.Add(particleType);
				LaneAnimation.Particle particle = particleType.CreateParticle();
				particle.pivot.transform.parent = base.transform;
				particle.pivot.transform.localScale = new Vector3((float)((!LaneAnimation.IsFlipped(this.startPosition, this.endPosition)) ? 1 : -1), 1f, 1f);
				particle.pivot.SetLayerRecursively(base.gameObject.layer);
				startPosition = particle.Initialize(startPosition, travelLength);
				num = Mathf.Min(num, particle.speed);
				particle.speed = num;
				particle.inFront = ((this.activeParticles.Count <= 0) ? null : this.activeParticles[this.activeParticles.Count - 1]);
				this.activeParticles.Add(particle);
			}
		}
	}

	public void UpdateParticles(float dt, float travelLength)
	{
		for (int i = 0; i < this.activeParticles.Count; i++)
		{
			if (!this.activeParticles[i].Update(dt, this.startPosition, this.endPosition, travelLength))
			{
				this.activeParticles[i].Destroy();
				this.activeParticles.RemoveAt(i--);
				if (this.activeParticles.Count > 0)
				{
					this.activeParticles[0].inFront = null;
				}
			}
		}
	}

	private void OnDisable()
	{
		foreach (LaneAnimation.Particle particle in this.activeParticles)
		{
			particle.Destroy();
		}
		this.activeParticles.Clear();
	}

	public LaneAnimation.ParticleType[] particleTypes;

	public bool emitImmediately;

	public float speedScaler = 1f;

	public Transform startTransform;

	public Transform endTransform;

	public int maxEmission = 1;

	public int minEmission = 1;

	public float minFrequency;

	public float maxFrequency;

	public int maxParticles = 3;

	private Lottery<LaneAnimation.ParticleType> particleTypesLottery;

	private List<LaneAnimation.Particle> activeParticles = new List<LaneAnimation.Particle>();

	private List<LaneAnimation.ParticleType> typeSpawnedThisEmit = new List<LaneAnimation.ParticleType>();

	private float emitProgress;

	private float nextEmitTime;

	private bool lastEmitFullyVisible;

	private bool isInitialized;

	[Serializable]
	public class ParticleType
	{
		public void Initialize()
		{
			Vector3 position = this.bounds.transform.TransformPoint(new Vector3(-this.bounds.Size.x * 0.5f, 0f, 0f));
			Vector3 position2 = this.bounds.transform.TransformPoint(new Vector3(this.bounds.Size.x * 0.5f, 0f, 0f));
			this.min = this.pivot.transform.parent.InverseTransformPoint(position).x;
			this.max = this.pivot.transform.parent.InverseTransformPoint(position2).x;
		}

		public LaneAnimation.Particle CreateParticle()
		{
			LaneAnimation.Particle particle = new LaneAnimation.Particle();
			particle.type = this;
			particle.pivot = UnityEngine.Object.Instantiate<GameObject>(this.pivot);
			particle.pivot.SetActive(true);
			particle.speed = 1f / (this.minTime + (this.maxTime - this.minTime) * UnityEngine.Random.value);
			return particle;
		}

		public GameObject pivot;

		public UIElement bounds;

		public float minTime;

		public float maxTime;

		public float probability = 1f;

		[NonSerialized]
		public float min;

		[NonSerialized]
		public float max;
	}

	public class Particle
	{
		public float GetNormalizedWidth(float length)
		{
			return (this.type.max - this.type.min) / length;
		}

		public float Initialize(float startPosition, float length)
		{
			this.position = startPosition;
			startPosition -= this.GetNormalizedWidth(length);
			return startPosition;
		}

		public bool Update(float dt, Vector3 start, Vector3 end, float length)
		{
			this.pivot.transform.localPosition = start + (end - start) * this.position + new Vector3((!LaneAnimation.IsFlipped(start, end)) ? (-this.type.min) : (-this.type.max), 0f, 0f);
			this.pivot.transform.localScale = new Vector3((float)((!LaneAnimation.IsFlipped(start, end)) ? 1 : -1), 1f, 1f);
			if (this.inFront != null && this.speed > this.inFront.speed)
			{
				float num = this.inFront.position - this.inFront.GetNormalizedWidth(length);
				float num2 = Mathf.Abs(num - this.position);
				if (num2 < 0.2f * this.speed)
				{
					this.speed = Mathf.Lerp(this.speed, this.inFront.speed, dt * 8f);
				}
			}
			this.position += this.speed * dt;
			return this.position < 1f + this.GetNormalizedWidth(length);
		}

		public bool IsFullyVisible(float length)
		{
			return this.position > this.GetNormalizedWidth(length);
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.pivot);
		}

		public LaneAnimation.ParticleType type;

		public GameObject pivot;

		public float speed;

		public float position;

		public LaneAnimation.Particle inFront;
	}
}
