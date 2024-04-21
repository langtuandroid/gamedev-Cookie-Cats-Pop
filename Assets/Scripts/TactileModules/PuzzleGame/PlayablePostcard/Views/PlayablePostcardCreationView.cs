using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fibers;
using JetBrains.Annotations;
using TactileModules.NativeSharing;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.PlayablePostcard.UI;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.Views
{
	public class PlayablePostcardCreationView : UIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> OnConfirmItem;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<PostcardItemTypeAndId>> OnFinalizedPostcard;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<PostcardItemTypeAndId>> OnSharePostcard;



		private void Update()
		{
			if (Input.GetMouseButtonDown(0) && !this.initialFlowFiber.IsTerminated && this.canSkipInitialFlow)
			{
				this.initialFlowFiber.Terminate();
			}
		}

		public void Initialize(PostcardItemContainer itemContainer, List<PostcardItemTypeAndId> postcardData)
		{
			this.postcardItemContainer = itemContainer;
			this.currentPostcardData = postcardData;
			this.currentPostcard = this.SpawnCurrentPostcard();
			this.initialPivot.gameObject.SetActive(false);
			this.pickPivot.gameObject.SetActive(false);
			this.confirmPivot.gameObject.SetActive(false);
			this.resultPivot.gameObject.SetActive(false);
			this.finalResultPivot.gameObject.SetActive(false);
			this.currentPostcard.SetActive(false);
			this.listPanel.gameObject.SetActive(false);
			this.cameraFlash.Color = new Color(1f, 1f, 1f, 0f);
			this.cameraFlash.gameObject.SetActive(true);
			this.overlay.gameObject.SetActive(false);
		}

		public void StartFlow(PostcardItemType itemType)
		{
			this.currentItemType = itemType;
			this.InitializeList();
			this.initialPivot.Initialize(this.moveAnimationCurve);
			this.pickPivot.Initialize(this.currentPostcard, itemType, this.moveAnimationCurve);
			this.confirmPivot.Initialize(this.moveAnimationCurve);
			this.initialFlowFiber.Start(this.InitialFlow());
		}

		public void ShowFinalResult(float duration = 0.4f)
		{
			this.finalResultPivot.Initialize(this.currentPostcard, duration, this.moveAnimationCurve);
			this.SetVisualState(VisualState.FinalResult);
		}

		private GameObject SpawnCurrentPostcard()
		{
			List<UITextureQuad> list = new List<UITextureQuad>();
			foreach (PostcardItemTypeAndId postcardItemTypeAndId in this.currentPostcardData)
			{
				if (postcardItemTypeAndId.Type != PostcardItemType.Character)
				{
					if (postcardItemTypeAndId.Type == PostcardItemType.Text)
					{
						UITextureQuad value = this.postcardItemContainer.GetItems(PostcardItemType.Frame, this.currentPostcardData).First<KeyValuePair<string, UITextureQuad>>().Value;
						list.Add(value);
						list.Add(this.logoTextureQuad);
					}
					UITextureQuad item = this.postcardItemContainer.GetItems(postcardItemTypeAndId.Type, this.currentPostcardData)[postcardItemTypeAndId.Id];
					list.Add(item);
				}
			}
			GameObject gameObject = this.SpawnPostcard(list.ToArray());
			gameObject.transform.parent = base.transform;
			gameObject.SetLayerRecursively(base.gameObject.layer);
			gameObject.transform.localPosition = this.listPanel.transform.localPosition + Vector3.forward * 15f;
			return gameObject;
		}

		private void InitializeList()
		{
			Dictionary<string, UITextureQuad> items = this.postcardItemContainer.GetItems(this.currentItemType, this.currentPostcardData);
			this.numberOfItems = items.Count;
			Vector2 vector = this.CalculateCellSize();
			this.listPanel.BeginAdding();
			this.listPanel.AddToContent(this.CreateMargin(vector));
			foreach (KeyValuePair<string, UITextureQuad> keyValuePair in items)
			{
				GameObject gameObject = new GameObject("PostcardCell");
				UIElement uielement = gameObject.AddComponent<UIElement>();
				uielement.Size = vector;
				GameObject gameObject2 = this.SpawnPostcard(new UITextureQuad[]
				{
					keyValuePair.Value
				});
				gameObject2.transform.parent = uielement.transform;
				this.itemList.Add(keyValuePair.Key);
				this.listPanel.AddToContent(uielement);
			}
			this.listPanel.AddToContent(this.CreateMargin(vector));
			this.listPanel.EndAdding();
		}

		private GameObject SpawnPostcard(UITextureQuad[] items)
		{
			GameObject gameObject = new GameObject("Postcard");
			float num = 0f;
			foreach (UITextureQuad uitextureQuad in items)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(uitextureQuad.gameObject);
				gameObject2.SetActive(true);
				gameObject2.transform.parent = gameObject.transform;
				Vector3 localPosition = gameObject2.transform.localPosition;
				gameObject2.transform.localPosition = new Vector3(localPosition.x, localPosition.y, num);
				this.UpdateShaderForTextureQuads(gameObject2);
				num -= 0.2f;
			}
			gameObject.transform.localScale = Vector3.one * (float)this.postcardSize;
			gameObject.transform.eulerAngles = new Vector3(0f, 0f, 2f);
			return gameObject;
		}

		private void UpdateShaderForTextureQuads(GameObject go)
		{
			foreach (UITextureQuad uitextureQuad in go.GetComponentsInChildren<UITextureQuad>(true))
			{
				uitextureQuad.Material.shader = Shader.Find(uitextureQuad.Material.shader.name);
			}
		}

		private Vector2 CalculateCellSize()
		{
			Dictionary<string, UITextureQuad> items = this.postcardItemContainer.GetItems(PostcardItemType.Background, this.currentPostcardData);
			Vector2 size = items.First<KeyValuePair<string, UITextureQuad>>().Value.Size;
			float num = size.x / size.y;
			float y = this.listPanel.Size.y;
			return new Vector2(num * y, y);
		}

		private IEnumerator InitialFlow()
		{
			this.SetVisualState(VisualState.Initial);
			while (!this.visualStateFiber.IsTerminated)
			{
				yield return null;
			}
			this.canSkipInitialFlow = true;
			yield return new Fiber.OnExit(delegate()
			{
				this.SetVisualState(VisualState.Pick);
				this.MoveToItem(0);
			});
			yield return FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0);
			yield break;
		}

		private PhotoBoothVisualState GetVisualState(VisualState state)
		{
			switch (state)
			{
			case VisualState.Initial:
				return this.initialPivot;
			case VisualState.Pick:
				return this.pickPivot;
			case VisualState.Confirm:
				return this.confirmPivot;
			case VisualState.Result:
				return this.resultPivot;
			case VisualState.FinalResult:
				return this.finalResultPivot;
			default:
				return null;
			}
		}

		private void SetVisualState(VisualState state)
		{
			this.visualStateFiber.Start(this.SetVisualStateCr(state));
		}

		private IEnumerator SetVisualStateCr(VisualState state)
		{
			PhotoBoothVisualState visualState = this.GetVisualState(state);
			if (this.currentVisualState != null)
			{
				yield return this.currentVisualState.AnimateOut();
				this.currentVisualState.gameObject.SetActive(false);
			}
			visualState.gameObject.SetActive(true);
			yield return visualState.AnimateIn();
			this.currentVisualState = visualState;
			yield break;
		}

		private UIElement CreateMargin(Vector2 cellSize)
		{
			GameObject gameObject = new GameObject();
			UIElement uielement = gameObject.AddComponent<UIElement>();
			uielement.Size = cellSize;
			return uielement;
		}

		private IEnumerator AnimateToItem()
		{
			int indexIncludingMargin = this.itemIndex + 1;
			yield return this.listPanel.AnimateScrollToItem(indexIncludingMargin, 0.2f, this.moveToItemCurve);
			this.pickPivot.SetLeftArrowActive(this.itemIndex > 0);
			this.pickPivot.SetRightArrowActive(this.itemIndex < this.numberOfItems - 1);
			yield break;
		}

		private void MoveToItem(int addAmount)
		{
			if (this.moveToItemFiber.IsTerminated)
			{
				this.itemIndex += addAmount;
				this.itemIndex = Mathf.Clamp(this.itemIndex, 0, this.numberOfItems - 1);
				this.moveToItemFiber.Start(this.AnimateToItem());
			}
		}

		private IEnumerator EndFlow()
		{
			string chosenItemId = this.itemList[this.itemIndex];
			this.OnConfirmItem(chosenItemId);
			if (this.currentItemType == PostcardItemType.Text)
			{
				this.listPanel.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(this.currentPostcard.gameObject);
				this.currentPostcardData.Add(new PostcardItemTypeAndId
				{
					Type = PostcardItemType.Text,
					Id = chosenItemId
				});
				this.currentPostcard = this.SpawnCurrentPostcard();
				this.ShowFinalResult(0.05f);
				this.OnFinalizedPostcard(this.currentPostcardData);
				yield return this.CameraFlash();
			}
			else
			{
				this.SetVisualState(VisualState.Result);
				yield return this.CameraFlash();
				yield return FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0);
				yield return this.FadeToBlack();
				base.Close(0);
			}
			yield break;
		}

		private IEnumerator CameraFlash()
		{
			yield return FiberAnimation.Animate(this.cameraFlashStartDur, null, delegate(float t)
			{
				this.cameraFlash.Color = new Color(1f, 1f, 1f, t);
			}, false);
			yield return FiberAnimation.Animate(this.cameraFlashEndDur, this.cameraFlashDisappearCurve, delegate(float t)
			{
				this.cameraFlash.Color = new Color(1f, 1f, 1f, 1f - t);
			}, false);
			yield break;
		}

		private IEnumerator FadeToBlack()
		{
			this.overlay.gameObject.SetActive(true);
			yield return FiberAnimation.Animate(1f, null, delegate(float t)
			{
				this.overlay.Color = new Color(0f, 0f, 0f, t);
			}, false);
			yield break;
		}

		[UsedImplicitly]
		private void PressRight(UIEvent e)
		{
			this.MoveToItem(1);
		}

		[UsedImplicitly]
		private void PressLeft(UIEvent e)
		{
			this.MoveToItem(-1);
		}

		[UsedImplicitly]
		private void RetryPick(UIEvent e)
		{
			this.SetVisualState(VisualState.Pick);
		}

		[UsedImplicitly]
		private void PickItem(UIEvent e)
		{
			if (!this.visualStateFiber.IsTerminated)
			{
				return;
			}
			this.SetVisualState(VisualState.Confirm);
		}

		[UsedImplicitly]
		private void ConfirmItem(UIEvent e)
		{
			if (!this.visualStateFiber.IsTerminated)
			{
				return;
			}
			this.endFlowFiber.Start(this.EndFlow());
		}

		[UsedImplicitly]
		private void SharePostcard(UIEvent e)
		{
			PostcardImageCapture postcardImageCapture = new PostcardImageCapture();
			Texture2D texture = postcardImageCapture.CapturePostcard(this.currentPostcard, this.captureShader);
			SharingService.Share(string.Empty, string.Empty, string.Empty, texture);
			this.OnSharePostcard(this.currentPostcardData);
		}

		[UsedImplicitly]
		private void Close(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UIListPanel listPanel;

		[SerializeField]
		private AnimationCurve moveToItemCurve;

		[SerializeField]
		private PhotoBoothInitial initialPivot;

		[SerializeField]
		private PhotoBoothPick pickPivot;

		[SerializeField]
		private PhotoBoothConfirm confirmPivot;

		[SerializeField]
		private PhotoBoothResult resultPivot;

		[SerializeField]
		private PhotoBoothFinalResult finalResultPivot;

		[SerializeField]
		private int postcardSize = 3;

		[SerializeField]
		private UISprite cameraFlash;

		[SerializeField]
		private UISprite overlay;

		[SerializeField]
		private float cameraFlashStartDur;

		[SerializeField]
		private float cameraFlashEndDur;

		[SerializeField]
		private AnimationCurve cameraFlashDisappearCurve;

		[SerializeField]
		private AnimationCurve moveAnimationCurve;

		[SerializeField]
		private Shader captureShader;

		[SerializeField]
		private UITextureQuad logoTextureQuad;

		private readonly Fiber moveToItemFiber = new Fiber();

		private readonly Fiber initialFlowFiber = new Fiber();

		private readonly Fiber visualStateFiber = new Fiber();

		private readonly Fiber endFlowFiber = new Fiber();

		private PhotoBoothVisualState currentVisualState;

		private PostcardItemType currentItemType;

		private GameObject currentPostcard;

		private int itemIndex;

		private int numberOfItems;

		private PostcardItemContainer postcardItemContainer;

		private bool canSkipInitialFlow;

		private List<PostcardItemTypeAndId> currentPostcardData;

		private readonly List<string> itemList = new List<string>();
	}
}
