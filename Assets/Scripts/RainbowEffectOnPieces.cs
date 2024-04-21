using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class RainbowEffectOnPieces
{
	public RainbowEffectOnPieces(LevelSession levelSession)
	{
		this.levelSession = levelSession;
		levelSession.TurnLogic.ShotFired += this.TurnLogic_ShotFired;
		this.rainbowSprites = new List<RainbowEffectOnPieces.RainbowSprite>();
		this.rainbowMaterial = new Material(SingletonAsset<PowerVisualSettings>.Instance.rainbowHitEffectMaterial);
	}

	private void TurnLogic_ShotFired(TurnLogic.Shot[] shots, ResolveState resolveState)
	{
		this.levelSession.TurnLogic.ShotFired -= this.TurnLogic_ShotFired;
		this.resolveState = resolveState;
		this.resolveState.AllHitsApplied += this.AllHitsApplied;
	}

	private void AllHitsApplied(List<HitMark> allHits)
	{
		this.resolveState.AllHitsApplied -= this.AllHitsApplied;
		this.rainbowPiece = null;
		HashSet<CPPiece> hashSet = new HashSet<CPPiece>();
		foreach (HitMark hitMark in allHits)
		{
			if (this.resolveState.IsPieceMarkedForRemoval(hitMark.piece) && !(hitMark.piece is RainbowPiece))
			{
				hashSet.Add(hitMark.piece);
				if (this.rainbowPiece == null && hitMark.causedBy is RainbowPiece)
				{
					this.rainbowPiece = hitMark.causedBy;
				}
			}
		}
		if (this.rainbowPiece != null)
		{
			hashSet.Add(this.rainbowPiece);
		}
		foreach (CPPiece piece in hashSet)
		{
			this.ApplyRainbowEffectToPiece(piece);
		}
		if (hashSet.Count > 0)
		{
			this.resolveState.PreExecute = new Func<IEnumerator>(this.PreResolveExecute);
		}
		FiberCtrl.Pool.Run(this.DoRainbowEffect(), false);
	}

	private void ApplyRainbowEffectToPiece(CPPiece piece)
	{
		UISprite[] componentsInChildren = piece.GetComponentsInChildren<UISprite>();
		foreach (UISprite uisprite in componentsInChildren)
		{
			this.rainbowSprites.Add(new RainbowEffectOnPieces.RainbowSprite
			{
				sprite = uisprite,
				atlasMaterial = uisprite.renderer.sharedMaterial
			});
			uisprite.renderer.sharedMaterial = this.rainbowMaterial;
		}
	}

	private IEnumerator PreResolveExecute()
	{
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		this.resolveState.PreExecute = null;
		yield break;
	}

	private IEnumerator DoRainbowEffect()
	{
		if (this.rainbowSprites.Count > 0)
		{
			if (this.rainbowPiece != null)
			{
				UICamera uicamera = UIViewManager.Instance.FindCameraFromObjectLayer(this.rainbowPiece.gameObject.layer);
				if (uicamera != null && uicamera.cachedCamera != null)
				{
					Vector3 vector = uicamera.cachedCamera.WorldToViewportPoint(this.rainbowPiece.transform.TransformPoint(new Vector3(0f, -10f, 0f)));
					this.rainbowMaterial.SetFloat("_PositionX", -1f + 2f * vector.x);
					this.rainbowMaterial.SetFloat("_PositionY", -1f + 2f * vector.y);
					this.rainbowMaterial.SetFloat("_ScreenAspect", uicamera.cachedCamera.aspect);
				}
			}
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.Animate(2f, delegate(float t)
				{
					this.rainbowMaterial.SetFloat("_LocationOffset", Mathf.Lerp(-0.5f, 0.8f, t));
				}),
				RainbowEffectOnPieces.FlashScreen(Color.white, 0.5f)
			});
			foreach (RainbowEffectOnPieces.RainbowSprite rainbowSprite in this.rainbowSprites)
			{
				if (rainbowSprite.sprite != null && rainbowSprite.sprite.renderer != null)
				{
					rainbowSprite.sprite.renderer.sharedMaterial = rainbowSprite.atlasMaterial;
				}
			}
			this.rainbowSprites.Clear();
		}
		UnityEngine.Object.Destroy(this.rainbowMaterial);
		yield break;
	}

	private static IEnumerator FlashScreen(Color color, float alpha)
	{
		Color orgColor = UIViewManager.Instance.FrontFillColor;
		UIViewManager.Instance.FrontFillColor = color;
		yield return new Fiber.OnExit(delegate()
		{
			UIViewManager.Instance.FrontFill = 0f;
			UIViewManager.Instance.FrontFillColor = orgColor;
		});
		yield return FiberAnimation.Animate(0f, SingletonAsset<PowerVisualSettings>.Instance.flashScreenCurve, delegate(float t)
		{
			UIViewManager.Instance.FrontFill = t * alpha;
		}, false);
		yield break;
	}

	private const int rainbowMaterialIndex = 5;

	private ResolveState resolveState;

	private LevelSession levelSession;

	private CPPiece rainbowPiece;

	private Material rainbowMaterial;

	private List<RainbowEffectOnPieces.RainbowSprite> rainbowSprites;

	private struct RainbowSprite
	{
		public UISprite sprite;

		public Material atlasMaterial;
	}
}
