using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fibers;
using UnityEngine;

public class TutorialFreeInstaFillBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		this.cookies = new List<TutorialFreeInstaFillBoosterLogic.Cookie>();
		PowerSlot.CustomMovementPositions += this.CustomCookieMovementPositions;
		this.jarGlass.gameObject.SetActive(true);
		yield return new Fiber.OnExit(delegate()
		{
			foreach (TutorialFreeInstaFillBoosterLogic.Cookie cookie4 in this.cookies)
			{
				UnityEngine.Object.Destroy(cookie4.t.gameObject);
			}
			this.cookies.Clear();
			PowerSlot.CustomMovementPositions -= this.CustomCookieMovementPositions;
		});
		if (session.PurchasedPower == null || !session.Powers.AvailablePowers.Contains(session.PurchasedPower))
		{
			yield break;
		}
		int cookiesToGive = session.PurchasedPower.MaxValue - session.PurchasedPower.Value;
		for (int i = 0; i < cookiesToGive; i++)
		{
			this.CreateCookie(session.PurchasedPower);
		}
		if (this.cookies.Count == 0)
		{
			yield break;
		}
		Rect rect = this.cookieArea.GetRectInLocalPos();
		int numberOfStacks = 4;
		float yOffset = 0.11f;
		for (int j = 0; j < this.cookies.Count; j++)
		{
			TutorialFreeInstaFillBoosterLogic.Cookie cookie2 = this.cookies[j];
			int num = j % numberOfStacks;
			int num2 = Mathf.FloorToInt((float)(j / numberOfStacks));
			cookie2.t.localPosition = new Vector3(Mathf.Lerp(rect.xMin, rect.xMax, 1f / (float)numberOfStacks * (float)num + 0.5f / (float)numberOfStacks), Mathf.Lerp(rect.yMin, rect.yMax, (float)num2 * yOffset), 0f);
			cookie2.t.gameObject.SetActive(true);
		}
		float zOrder = 0f;
		foreach (TutorialFreeInstaFillBoosterLogic.Cookie cookie3 in this.cookies)
		{
			Vector3 localPosition = cookie3.t.localPosition;
			localPosition.z = zOrder;
			zOrder -= 0.01f;
			cookie3.t.localPosition = localPosition;
		}
		yield return FiberAnimation.ScaleTransform(this.jarTransform, Vector3.one * 0.001f, Vector3.one, this.jarScaleCurve, 0f);
		yield return FiberAnimation.MoveLocalTransform(this.jarGlass, Vector3.zero, this.glassTarget.localPosition, this.glassMovementCurve, 0f);
		float dWait = this.giveCookiesDuration / (float)this.cookies.Count;
		this.cookies.Reverse();
		foreach (TutorialFreeInstaFillBoosterLogic.Cookie cookie in this.cookies)
		{
			cookie.power.AddValue(1, cookie.power.Color, cookie.t.position);
			cookie.t.gameObject.SetActive(false);
			yield return FiberHelper.Wait(dWait, (FiberHelper.WaitFlag)0);
		}
		this.jarGlass.gameObject.SetActive(false);
		yield return FiberAnimation.ScaleTransform(this.jarTransform, Vector3.one, Vector3.zero, SingletonAsset<CommonCurves>.Instance.easeInOut, 0.2f);
		session.PurchasedPower.Activate();
		session.PurchasedPower = null;
		yield break;
	}

	private void CustomCookieMovementPositions(PowerSlot.CookieMovementPositions positions)
	{
		positions.startPos = positions.startWorldPosition;
		positions.endPos.z = positions.startWorldPosition.z;
	}

	private void CreateCookie(GamePowers.Power power)
	{
		string collectSpriteForColor = SingletonAsset<LevelVisuals>.Instance.GetCollectSpriteForColor(power.Color);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SingletonAsset<LevelVisuals>.Instance.collectPrefab, this.cookieArea.transform, true);
		gameObject.GetComponent<UISprite>().SpriteName = collectSpriteForColor;
		gameObject.gameObject.SetLayerRecursively(base.gameObject.layer);
		this.cookies.Add(new TutorialFreeInstaFillBoosterLogic.Cookie
		{
			t = gameObject.transform,
			power = power
		});
	}

	[SerializeField]
	private UIElement cookieArea;

	[SerializeField]
	private Transform jarTransform;

	[SerializeField]
	private Transform jarGlass;

	[SerializeField]
	private AnimationCurve jarScaleCurve;

	[SerializeField]
	private Transform glassTarget;

	[SerializeField]
	private AnimationCurve glassMovementCurve;

	[SerializeField]
	private float giveCookiesDuration = 1f;

	private List<TutorialFreeInstaFillBoosterLogic.Cookie> cookies;

	private class Cookie
	{
		public Transform t;

		public GamePowers.Power power;
	}
}
