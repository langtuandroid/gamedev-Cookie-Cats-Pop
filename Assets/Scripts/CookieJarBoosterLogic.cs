using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class CookieJarBoosterLogic : BoosterLogic
{
	protected virtual Dictionary<MatchFlag, int> CookiesToGive(List<GamePowers.Power> availablePowers)
	{
		Dictionary<MatchFlag, int> dictionary = new Dictionary<MatchFlag, int>();
		int num = availablePowers[0].MaxValue / 2 + 1;
		foreach (GamePowers.Power power in availablePowers)
		{
			dictionary[power.Color] = num;
			num /= 2;
		}
		return dictionary;
	}

	protected override IEnumerator Logic(LevelSession session)
	{
		this.cookies = new List<CookieJarBoosterLogic.Cookie>();
		PowerSlot.CustomMovementPositions += this.CustomCookieMovementPositions;
		yield return new Fiber.OnExit(delegate()
		{
			foreach (CookieJarBoosterLogic.Cookie cookie4 in this.cookies)
			{
				UnityEngine.Object.Destroy(cookie4.t.gameObject);
			}
			this.cookies.Clear();
			PowerSlot.CustomMovementPositions -= this.CustomCookieMovementPositions;
		});
		List<GamePowers.Power> availablePowers = new List<GamePowers.Power>(session.Powers.AvailablePowers);
		if (availablePowers.Count == 0)
		{
			yield break;
		}
		availablePowers.Shuffle<GamePowers.Power>();
		Dictionary<MatchFlag, int> cookiesToGive = this.CookiesToGive(availablePowers);
		foreach (GamePowers.Power power in availablePowers)
		{
			for (int i = 0; i < cookiesToGive[power.Color]; i++)
			{
				this.CreateCookie(power);
			}
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
			CookieJarBoosterLogic.Cookie cookie2 = this.cookies[j];
			int num = j % numberOfStacks;
			int num2 = Mathf.FloorToInt((float)(j / numberOfStacks));
			cookie2.t.localPosition = new Vector3(Mathf.Lerp(rect.xMin, rect.xMax, 1f / (float)numberOfStacks * (float)num + 0.5f / (float)numberOfStacks), Mathf.Lerp(rect.yMin, rect.yMax, (float)num2 * yOffset), 0f);
		}
		float zOrder = 0f;
		foreach (CookieJarBoosterLogic.Cookie cookie3 in this.cookies)
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
		foreach (CookieJarBoosterLogic.Cookie cookie in this.cookies)
		{
			cookie.power.AddValue(1, cookie.power.Color, cookie.t.position);
			cookie.t.gameObject.SetActive(false);
			yield return FiberHelper.Wait(dWait, (FiberHelper.WaitFlag)0);
		}
		this.jarGlass.gameObject.SetActive(false);
		yield return FiberAnimation.ScaleTransform(this.jarTransform, Vector3.one, Vector3.zero, SingletonAsset<CommonCurves>.Instance.easeInOut, 0.2f);
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
		this.cookies.Add(new CookieJarBoosterLogic.Cookie
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
	private float giveCookiesDuration = 2f;

	private List<CookieJarBoosterLogic.Cookie> cookies;

	private class Cookie
	{
		public Transform t;

		public GamePowers.Power power;
	}
}
