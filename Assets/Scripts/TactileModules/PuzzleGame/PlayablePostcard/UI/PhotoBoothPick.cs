using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
	public class PhotoBoothPick : PhotoBoothVisualState
	{
		public void Initialize(GameObject postcard, PostcardItemType postcardItemType, AnimationCurve animationMoveCurve)
		{
			this.currentPostcard = postcard;
			this.originalPostcardPos = this.currentPostcard.transform.localPosition;
			this.originalTopBarPos = this.topBar.transform.localPosition;
			this.originalBottomBarPos = this.bottomBar.transform.localPosition;
			this.originalLeftArrowPos = this.leftArrow.transform.localPosition;
			this.originalRightArrowPos = this.rightArrow.transform.localPosition;
			this.originalListPos = this.listPanel.transform.localPosition;
			this.title.text = this.GetTitleText(postcardItemType);
			this.moveCurve = animationMoveCurve;
		}

		private string GetTitleText(PostcardItemType itemType)
		{
			switch (itemType)
			{
			case PostcardItemType.Background:
				return L.Get("Pick a background!");
			case PostcardItemType.Character:
				return L.Get("Pick a character!");
			case PostcardItemType.Costume:
				return L.Get("Pick a costume!");
			case PostcardItemType.Prop:
				return L.Get("Pick a prop!");
			case PostcardItemType.Text:
				return L.Get("Pick a text!");
			default:
				return null;
			}
		}

		public override IEnumerator AnimateIn()
		{
			this.leftArrow.transform.localPosition = this.originalLeftArrowPos - new Vector3(100f, 0f, 0f);
			this.rightArrow.transform.localPosition = this.originalRightArrowPos + new Vector3(100f, 0f, 0f);
			List<IEnumerator> enums = new List<IEnumerator>();
			if (this.numberOfAnimateIn < 1)
			{
				enums.Add(this.AnimatePostcardIn());
				enums.Add(this.AnimateListPanelIn());
			}
			enums.Add(this.AnimateTopBarIn());
			enums.Add(this.AnimateBottomBarIn());
			yield return FiberHelper.RunParallel(enums.ToArray());
			yield return this.AnimateArrowsIn();
			this.numberOfAnimateIn++;
			yield break;
		}

		public override IEnumerator AnimateOut()
		{
			yield return this.AnimateArrowsOut();
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.AnimateTopBarOut(),
				this.AnimateBottomBarOut()
			});
			yield break;
		}

		private IEnumerator AnimateArrowsIn()
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveLocalTransform(this.leftArrow.transform, this.originalLeftArrowPos - new Vector3(100f, 0f, 0f), this.originalLeftArrowPos, null, 0.4f),
				FiberAnimation.MoveLocalTransform(this.rightArrow.transform, this.originalRightArrowPos + new Vector3(100f, 0f, 0f), this.originalRightArrowPos, null, 0.4f)
			});
			yield break;
		}

		private IEnumerator AnimateArrowsOut()
		{
			Vector3 leftOriginalPos = this.leftArrow.transform.localPosition;
			Vector3 rightOriginalPos = this.rightArrow.transform.localPosition;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveLocalTransform(this.leftArrow.transform, leftOriginalPos, leftOriginalPos - new Vector3(100f, 0f, 0f), null, 0.2f),
				FiberAnimation.MoveLocalTransform(this.rightArrow.transform, rightOriginalPos, rightOriginalPos + new Vector3(100f, 0f, 0f), null, 0.2f)
			});
			yield break;
		}

		public void SetLeftArrowActive(bool isActive)
		{
			this.leftArrow.gameObject.SetActive(isActive);
		}

		public void SetRightArrowActive(bool isActive)
		{
			this.rightArrow.gameObject.SetActive(isActive);
		}

		private IEnumerator AnimateListPanelIn()
		{
			this.listPanel.gameObject.SetActive(true);
			yield return FiberAnimation.MoveLocalTransform(this.listPanel.transform, this.originalListPos + new Vector3(800f, 0f, 0f), this.originalListPos, this.listPanelMoveCurve, 0.6f);
			yield break;
		}

		private IEnumerator AnimatePostcardIn()
		{
			this.currentPostcard.gameObject.SetActive(true);
			yield return FiberAnimation.MoveLocalTransform(this.currentPostcard.transform, this.originalPostcardPos + new Vector3(800f, 0f, 0f), this.originalPostcardPos, this.listPanelMoveCurve, 0.6f);
			yield break;
		}

		private IEnumerator AnimateTopBarIn()
		{
			yield return FiberAnimation.MoveLocalTransform(this.topBar.transform, this.originalTopBarPos + new Vector3(0f, 300f, 0f), this.originalTopBarPos, this.moveCurve, 0.6f);
			yield break;
		}

		private IEnumerator AnimateTopBarOut()
		{
			yield return FiberAnimation.MoveLocalTransform(this.topBar.transform, this.originalTopBarPos, this.originalTopBarPos + new Vector3(0f, 300f, 0f), this.moveCurve, 0.4f);
			yield break;
		}

		private IEnumerator AnimateBottomBarIn()
		{
			yield return FiberAnimation.MoveLocalTransform(this.bottomBar.transform, this.originalBottomBarPos + new Vector3(0f, -250f, 0f), this.originalBottomBarPos, this.moveCurve, 0.6f);
			yield break;
		}

		private IEnumerator AnimateBottomBarOut()
		{
			yield return FiberAnimation.MoveLocalTransform(this.bottomBar.transform, this.originalBottomBarPos, this.originalBottomBarPos + new Vector3(0f, -250f, 0f), this.moveCurve, 0.4f);
			yield break;
		}

		[SerializeField]
		private GameObject topBar;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private GameObject bottomBar;

		[SerializeField]
		private UIListPanel listPanel;

		[SerializeField]
		private AnimationCurve listPanelMoveCurve;

		[SerializeField]
		private GameObject leftArrow;

		[SerializeField]
		private GameObject rightArrow;

		private GameObject currentPostcard;

		private Vector3 originalPostcardPos;

		private Vector3 originalTopBarPos;

		private Vector3 originalBottomBarPos;

		private Vector3 originalLeftArrowPos;

		private Vector3 originalRightArrowPos;

		private Vector3 originalListPos;

		private int numberOfAnimateIn;

		private AnimationCurve moveCurve;
	}
}
