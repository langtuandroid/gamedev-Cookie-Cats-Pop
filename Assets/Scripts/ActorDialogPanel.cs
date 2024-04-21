using System;
using System.Collections;
using System.Collections.Generic;
using Tactile.GardenGame.Story.Dialog;
using TactileModules.Validation;
using UnityEngine;

public class ActorDialogPanel : MonoBehaviour
{
	public UILabel Label
	{
		get
		{
			return this.label;
		}
	}

	private GameObject CreateCharacterVisuals(DialogCharacter dialogCharacter, string poseId)
	{
		SkeletonAnimation spinePose = dialogCharacter.GetSpinePose(poseId, null);
		if (spinePose == null)
		{
			return new GameObject();
		}
		GameObject gameObject = spinePose.gameObject;
		gameObject.transform.parent = this.characterParent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetLayerRecursively(base.gameObject.layer);
		return gameObject;
	}

	private void DestroyCharacterVisuals()
	{
		if (this.characterVisuals == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.characterVisuals);
		this.characterVisuals = null;
	}

	public void Initialize()
	{
		if (this.image != null)
		{
			this.image.gameObject.SetActive(false);
		}
		if (this.splitScreenPivot != null)
		{
			this.splitScreenPivotDefaultPos = this.splitScreenPivot.localPosition;
		}
		this.bubble.gameObject.SetActive(false);
		this.SetCharacterFocus(0f);
	}

	private void SetCharacterFocus(float focusProgress)
	{
		float time = this.anim.characterFocusScaleCurve.Duration() * focusProgress;
		this.characterPivot.localScale = Vector3.one * this.anim.characterFocusScaleCurve.Evaluate(time);
		this.characterColor = Color.Lerp(this.anim.CharacterUnfocusedColor, Color.white, focusProgress);
		if (this.characterVisuals != null)
		{
			SkeletonAnimation componentInChildren = this.characterVisuals.GetComponentInChildren<SkeletonAnimation>();
			if (componentInChildren != null)
			{
				componentInChildren.skeleton.SetColor(new Color(this.characterColor.r, this.characterColor.g, this.characterColor.b, componentInChildren.skeleton.a));
			}
		}
	}

	public IEnumerator Animate(DialogCharacter character, string pose, string text, string imageResourcePath, string splitScreenImagePath)
	{
		List<IEnumerator> enums = new List<IEnumerator>();
		bool wasInFocus = !string.IsNullOrEmpty(this.previousText) || !string.IsNullOrEmpty(this.previousImage);
		bool isInFocus = !string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(imageResourcePath);
		if (wasInFocus && !isInFocus)
		{
			enums.Add(this.ScaleSpeechBubble(true));
		}
		this.label.text = string.Empty;
		if (splitScreenImagePath != this.prevSplitScreenImagePath)
		{
			enums.Add(this.AnimateSplitScreenImage(splitScreenImagePath));
		}
		if (character != this.previousCharacter)
		{
			enums.Add(this.AmimateCharacter(this.previousCharacter, character, pose));
		}
		else if (pose != this.previousPose && character != null)
		{
			enums.Add(this.AnimatePose(character, this.previousPose, pose));
		}
		if (wasInFocus != isInFocus && character != null)
		{
			enums.Add(FiberAnimation.Animate(this.anim.characterFocusScaleCurve.Duration(), delegate(float t)
			{
				this.SetCharacterFocus((!isInFocus) ? (1f - t) : t);
			}));
		}
		this.namePlate.gameObject.SetActive(character != null);
		this.speechBubbleArrow.gameObject.SetActive(character != null);
		if (enums.Count > 0)
		{
			yield return FiberHelper.RunParallel(enums);
		}
		if (!wasInFocus && isInFocus)
		{
			yield return this.ScaleSpeechBubble(false);
		}
		this.label.text = text;
		if (Application.isPlaying)
		{
			this.label.typingProgress = 0f;
			this.label.typingLookahead = 0;
			this.label.typingOnStart = true;
		}
		else
		{
			this.label.typingProgress = 1f;
			this.label.typingLookahead = -1;
			this.label.typingOnStart = false;
		}
		if (this.image != null)
		{
			if (string.IsNullOrEmpty(imageResourcePath))
			{
				this.image.gameObject.SetActive(false);
			}
			else
			{
				this.image.gameObject.SetActive(true);
				this.image.TextureResource = imageResourcePath;
			}
		}
		this.previousCharacter = character;
		this.previousPose = pose;
		this.previousText = text;
		this.previousImage = imageResourcePath;
		yield break;
	}

	private IEnumerator AmimateCharacter(DialogCharacter prevCharacter, DialogCharacter currentCharacter, string currentPose)
	{
		if (prevCharacter != null)
		{
			yield return this.Animate(this.anim.characterRevealCurve, true, delegate(float t)
			{
				this.characterPivot.position = Vector3.Lerp(this.hiddenCharacterLocation.position, this.shownCharacterLocation.position, t);
			});
		}
		this.characterPivot.position = this.hiddenCharacterLocation.position;
		this.DestroyCharacterVisuals();
		this.nameLabel.text = string.Empty;
		if (currentCharacter != null)
		{
			this.characterVisuals = this.CreateCharacterVisuals(currentCharacter, currentPose);
			if (this.characterVisuals == null)
			{
				yield break;
			}
			this.SetCharacterFocus(0f);
			this.nameLabel.text = currentCharacter.LocalizedName;
			yield return this.Animate(this.anim.characterRevealCurve, false, delegate(float t)
			{
				this.characterPivot.position = Vector3.Lerp(this.hiddenCharacterLocation.position, this.shownCharacterLocation.position, t);
			});
		}
		yield break;
	}

	private IEnumerator AnimatePose(DialogCharacter currentCharacter, string prevPose, string currentPose)
	{
		GameObject newCharacterVisuals = this.CreateCharacterVisuals(currentCharacter, currentPose);
		if (newCharacterVisuals == null)
		{
			yield break;
		}
		SkeletonAnimation fadeOutMaterial = this.characterVisuals.GetComponentInChildren<SkeletonAnimation>();
		SkeletonAnimation fadeInMaterial = newCharacterVisuals.GetComponentInChildren<SkeletonAnimation>();
		if (currentCharacter.enableXFade)
		{
			yield return FiberAnimation.Animate(0.25f, delegate(float t)
			{
				fadeOutMaterial.skeleton.SetColor(this.SetAlpha(this.characterColor, 1f - t));
				fadeInMaterial.skeleton.SetColor(this.SetAlpha(this.characterColor, t));
			});
		}
		this.DestroyCharacterVisuals();
		this.characterVisuals = newCharacterVisuals;
		yield break;
	}

	private Color SetAlpha(Color color, float alpha)
	{
		color.a = alpha;
		return color;
	}

	private IEnumerator AnimateSplitScreenImage(string splitScreenImagePath)
	{
		if (splitScreenImagePath != this.prevSplitScreenImagePath)
		{
			if (this.splitScreenPivot != null)
			{
				if (!string.IsNullOrEmpty(this.prevSplitScreenImagePath))
				{
					yield return this.Animate(this.anim.characterRevealCurve, false, delegate(float t)
					{
						this.splitScreenPivot.localPosition = Vector3.Lerp(this.splitScreenPivot.localPosition, this.splitScreenPivotDefaultPos + this.splitScreenPivotRemoveOffset, t);
					});
				}
				if (this.splitScreenImage != null && !string.IsNullOrEmpty(splitScreenImagePath))
				{
					this.splitScreenImage.TextureResource = splitScreenImagePath;
				}
				bool showSplit = !string.IsNullOrEmpty(splitScreenImagePath);
				this.splitScreenPivot.gameObject.SetActive(showSplit);
				if (showSplit)
				{
					yield return this.Animate(this.anim.characterRevealCurve, false, delegate(float t)
					{
						this.splitScreenPivot.localPosition = Vector3.Lerp(this.splitScreenPivotDefaultPos + this.splitScreenPivotRemoveOffset, this.splitScreenPivotDefaultPos, t);
					});
				}
			}
			this.prevSplitScreenImagePath = splitScreenImagePath;
		}
		yield return null;
		yield break;
	}

	private IEnumerator ScaleSpeechBubble(bool fadeOut)
	{
		if (!fadeOut)
		{
			this.bubble.gameObject.SetActive(true);
			this.onAnimateSound.Play();
		}
		Transform oldParent = this.bubble.transform.parent;
		this.bubble.transform.parent = this.bubbleScalePivot.transform;
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.Animate(this.anim.speechBoxScaleCurve, fadeOut, delegate(float t)
			{
				this.bubbleScalePivot.localScale = Vector3.one * t;
			}),
			this.Animate(this.anim.speechBoxAlphaCurve, fadeOut, delegate(float t)
			{
				for (int i = 0; i < this.fadingWidgets.Length; i++)
				{
					this.fadingWidgets[i].Alpha = t;
				}
			})
		});
		this.bubbleScalePivot.localScale = Vector3.one;
		this.bubble.transform.parent = oldParent;
		if (fadeOut)
		{
			this.bubble.gameObject.SetActive(false);
		}
		yield break;
	}

	public bool IsTextAnimating
	{
		get
		{
			return !string.IsNullOrEmpty(this.label.text) && this.label.typingProgress < 1f;
		}
	}

	public bool HasTextFinishedAnimating
	{
		get
		{
			return !string.IsNullOrEmpty(this.label.text) && this.label.typingProgress >= 1f;
		}
	}

	public void ShowTextInstantly()
	{
		if (string.IsNullOrEmpty(this.label.text))
		{
			return;
		}
		this.label.typingProgress = 1f;
	}

	private IEnumerator Animate(AnimationCurve curve, bool backwards, Action<float> lerpFunc)
	{
		float curveDuration = curve.Duration();
		yield return FiberAnimation.Animate(curveDuration, delegate(float t)
		{
			float time = backwards ? ((1f - t) * curveDuration) : (t * curveDuration);
			float obj = curve.Evaluate(time);
			lerpFunc(obj);
		});
		yield break;
	}

	private IEnumerator PlayTextWriterSound()
	{
		while (this.IsTextAnimating)
		{
			this.typeWriterSound.Play();
			yield return FiberHelper.WaitForFrames(1, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	public ISoundDefinition SoundDefinition
	{
		get
		{
			return this.onAnimateSound;
		}
	}

	[SerializeField]
	private UILabel label;

	[SerializeField]
	[OptionalSerializedField]
	private UIResourceQuad image;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private Transform bubble;

	[SerializeField]
	private Transform bubbleScalePivot;

	[SerializeField]
	private Transform characterPivot;

	[SerializeField]
	private Transform characterParent;

	[SerializeField]
	private Transform namePlate;

	[SerializeField]
	private Transform speechBubbleArrow;

	[SerializeField]
	private Transform shownCharacterLocation;

	[SerializeField]
	private Transform hiddenCharacterLocation;

	[SerializeField]
	private UIWidget[] fadingWidgets;

	[SerializeField]
	private ActorDialogPanelAnimation anim;

	[SerializeField]
	[OptionalSerializedField]
	private UIResourceQuad splitScreenImage;

	[SerializeField]
	[OptionalSerializedField]
	private Transform splitScreenPivot;

	[SerializeField]
	private Vector3 splitScreenPivotRemoveOffset = new Vector3(1000f, 0f, 0f);

	[SerializeField]
	private SoundDefinition onAnimateSound;

	[SerializeField]
	private SoundDefinition typeWriterSound;

	private DialogCharacter previousCharacter;

	private string previousPose;

	private string previousText;

	private string previousImage;

	private string prevSplitScreenImagePath;

	private GameObject characterVisuals;

	private Color characterColor;

	private float characterAlpha;

	private Vector3 splitScreenPivotDefaultPos;
}
