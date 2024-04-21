using System;
using System.Collections;
using Spine;
using UnityEngine;

public class SeagullEffect : SpawnedEffect
{
	private Vector3 GetWorldPositionInFlyArea(Vector2 normalizedPosition)
	{
		Vector3 position = new Vector3(-this.flyArea.Size.x * 0.5f + this.flyArea.Size.x * normalizedPosition.x, -this.flyArea.Size.y * 0.5f + this.flyArea.Size.y * normalizedPosition.y, 0f);
		return this.flyArea.transform.TransformPoint(position);
	}

	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.wasHit = false;
		this.seagullTransform.gameObject.SetActive(false);
		yield return null;
		this.seagullTransform.gameObject.SetActive(true);
		bool isBackgroundSeagull = parameters.Length == 0;
		if (!Singleton<SeagullManager>.Instance.IsSeagullReady || !isBackgroundSeagull)
		{
			SeagullEffect.SetupParameters s = (parameters.Length <= 0) ? Singleton<SeagullManager>.Instance.GetBackgroundParams() : (parameters[0] as SeagullEffect.SetupParameters);
			if (s == null)
			{
				s = new SeagullEffect.SetupParameters();
				s.distanceAway = UnityEngine.Random.Range(0.5f, 1f);
				s.flyDuration = UnityEngine.Random.Range(4f, 8f);
				s.flyLeft = (UnityEngine.Random.value < 0.5f);
				s.height = UnityEngine.Random.value;
			}
			this.spine.state.SetAnimation(0, "Idle", true);
			Vector3 source = this.GetWorldPositionInFlyArea(new Vector2((float)((!s.flyLeft) ? 0 : 1), s.height));
			Vector3 dest = this.GetWorldPositionInFlyArea(new Vector2((float)((!s.flyLeft) ? 1 : 0), s.height));
			Vector3 seagullScale = Vector3.one * Mathf.Lerp(1f, this.farAwayScale, s.distanceAway);
			if (!s.flyLeft)
			{
				seagullScale.x = -seagullScale.x;
			}
			this.seagullTransform.localScale = seagullScale;
			Color farColor = SingletonAsset<LevelVisuals>.Instance.GameSpineTintColor;
			this.spineRenderer.material.SetColor("_TintColor", farColor);
			this.spineRenderer.material.SetFloat("_TintAmount", s.distanceAway);
			float i = 0f;
			float rate = 1f / s.flyDuration;
			while ((double)i < 1.0 && !this.wasHit)
			{
				i += Time.deltaTime * rate;
				this.seagullTransform.position = Vector3.Lerp(source, dest, i);
				yield return null;
			}
			if (this.wasHit)
			{
				i = 0f;
				rate = 1f / (s.flyDuration / 4f);
				while ((double)i < 1.0)
				{
					i += Time.deltaTime * rate;
					this.seagullTransform.position = Vector3.Lerp(this.hitPosition, dest, i);
					yield return null;
				}
			}
		}
		yield break;
	}

	public override bool SpawnCountingEnabled
	{
		get
		{
			return false;
		}
	}

	public void Hit()
	{
		TrackEntry trackEntry = this.spine.state.SetAnimation(0, "Hit", false);
		this.spine.state.AddAnimation(0, "Idle", true, trackEntry.endTime - trackEntry.mixDuration);
		this.wasHit = true;
		this.hitPosition = this.seagullTransform.position;
	}

	public Transform seagullTransform;

	public SkeletonAnimation spine;

	public Renderer spineRenderer;

	public Color farAwayColor = Color.white;

	public float farAwayScale = 0.2f;

	public RewardGrid rewardGrid;

	public UIElement flyArea;

	public bool wasHit;

	public Vector3 hitPosition;

	public class SetupParameters
	{
		public float distanceAway;

		public float flyDuration;

		public float height;

		public bool flyLeft;
	}
}
