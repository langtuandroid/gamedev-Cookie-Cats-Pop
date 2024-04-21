using System;
using System.Collections.Generic;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/LevelVisuals.asset")]
public class LevelVisuals : SingletonAsset<LevelVisuals>
{
	public Color GetAimColorFromMatchColor(MatchFlag color)
	{
		string text = color;
		if (text != null)
		{
			if (text == "Blue")
			{
				return this.blueAim;
			}
			if (text == "Green")
			{
				return this.greenAim;
			}
			if (text == "Yellow")
			{
				return this.yellowAim;
			}
			if (text == "Red")
			{
				return this.redAim;
			}
			if (text == "Purple")
			{
				return this.pinkAim;
			}
		}
		return Color.white;
	}

	public string GetCollectSpriteForColor(MatchFlag color)
	{
		string text = color;
		if (text != null)
		{
			if (text == "Blue")
			{
				return this.blueCollectSprite;
			}
			if (text == "Green")
			{
				return this.greenCollectSprite;
			}
			if (text == "Yellow")
			{
				return this.yellowCollectSprite;
			}
			if (text == "Red")
			{
				return this.redCollectSprite;
			}
		}
		return string.Empty;
	}

	public string GetAwesomeText(int multiplier)
	{
		multiplier = Mathf.Clamp(multiplier - 1, 0, this.multiplierAwesomeTexts.Count - 1);
		return L.Get(this.multiplierAwesomeTexts[multiplier].texts.GetRandom<string>());
	}

	public UIFontStyle GetFontStyleFromMatchColor(MatchFlag color)
	{
		string text = color;
		if (text != null)
		{
			if (text == "Blue")
			{
				return this.fontStyleBlue;
			}
			if (text == "Green")
			{
				return this.fontStyleGreen;
			}
			if (text == "Yellow")
			{
				return this.fontStyleYellow;
			}
			if (text == "Red")
			{
				return this.fontStyleRed;
			}
			if (text == "Purple")
			{
				return this.fontStylePurple;
			}
		}
		return this.fontStyleWhite;
	}

	public GameObject topLine;

	public Transform aimDot;

	public float unitsPerDot = 50f;

	public float shotSpeed = 3000f;

	public float resolveTimingInterval = 0.5f;

	public GameObject collectPrefab;

	public AnimationCurve collectCurve;

	public AnimationCurve collectScaleCurve;

	public AnimationCurve bigBubblePopScale;

	public AnimationCurve boxJumpCurve;

	public float tracerBounceLength = 100f;

	public float clusterPopInterval = 0.05f;

	public float piecesSpringCoefficient = 1f;

	public float piecesSpringFriction = 0.1f;

	public float piecesSpringForce = 1f;

	public float piecesSpringWobble = 0.01f;

	public float kittenWalkHeight = 10f;

	public float kittenWalkStepLength = 10f;

	public float piecesSpringArea = 500f;

	public float boardPanningInUnitsPerSecond = 400f;

	public List<LevelVisuals.FreebieEntry> freebeeBoosters;

	public List<LevelVisuals.FreebieEntry> hardModeFreebeeBoosters;

	public BoosterLogic catPowerPurchaseBoosterLogic;

	public BoosterLogic cookieJarBoosterLogic;

	public BoosterLogic cookieJarFillUpBoosterLogic;

	public string NoMatchBallSwapText;

	public string FreebieInstafeedText;

	public string powercatReady;

	public AnimationCurve powerCatShimmerScale;

	[UISpriteName]
	public string redCollectSprite;

	[UISpriteName]
	public string blueCollectSprite;

	[UISpriteName]
	public string yellowCollectSprite;

	[UISpriteName]
	public string greenCollectSprite;

	public Color redAim;

	public Color blueAim;

	public Color yellowAim;

	public Color greenAim;

	public Color pinkAim;

	public AnimationCurve fallingCurve;

	public AnimationCurve aftermathCurve;

	public Color GameUITintColor = new Color(1f, 1f, 1f);

	public Color GameSpineTintColor = new Color(1f, 1f, 1f);

	public List<LevelVisuals.AwesomeTexts> multiplierAwesomeTexts;

	public LevelVisuals.LevelResultVisuals levelResultVisuals;

	public UIFontStyle fontStyleBlue;

	public UIFontStyle fontStyleGreen;

	public UIFontStyle fontStylePurple;

	public UIFontStyle fontStyleRed;

	public UIFontStyle fontStyleWhite;

	public UIFontStyle fontStyleYellow;

	[Serializable]
	public class AwesomeTexts
	{
		public List<string> texts;
	}

	[Serializable]
	public class FreebieEntry
	{
		public BoosterLogic logicPrefab;

		public float chance;

		public bool availableWithBoosters;
	}

	[Serializable]
	public class LevelResultVisuals
	{
		public AnimationCurve starScaleCurve;

		public AnimationCurve starColorCurve;

		public AnimationCurve starMoveCurve;

		public AnimationCurve glowCurve;

		public float starHitDelay;

		public float starAnimationDuration;
	}
}
