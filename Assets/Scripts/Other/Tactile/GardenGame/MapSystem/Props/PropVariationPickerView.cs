using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class PropVariationPickerView : UIView
	{
		private void CreateButton(Texture2D icon, int index, Action clicked)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab);
			gameObject.name = index.ToString();
			gameObject.transform.parent = this.buttonArea.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.GetComponent<UIButton>().Clicked += delegate(UIButton b)
			{
				clicked();
			};
			gameObject.GetComponent<UIButton>().receiver = base.gameObject;
			UITextureQuad componentInChildren = gameObject.GetComponentInChildren<UITextureQuad>();
			componentInChildren.SetTexture(icon);
			gameObject.SetLayerRecursively(base.gameObject.layer);
		}

		public void Initialize(MapProp mapProp, Action<MapProp.Variation> selectionChanged, Action onCancel, Action onConfirm, bool cancelButtonVisible)
		{
			this.selectionChanged = selectionChanged;
			this.onCancel = onCancel;
			this.onConfirm = onConfirm;
			int num = 0;
			foreach (MapProp.Variation variation in mapProp.Variations)
			{
				if (variation.IsPickable)
				{
					MapProp.Variation currentVariationId = variation;
					this.CreateButton(variation.Icon.Texture, num, delegate
					{
						this.confirmButton.SetActive(true);
						selectionChanged(currentVariationId);
					});
					num++;
				}
			}
			this.buttonArea.DoLayout();
			this.confirmButton.SetActive(false);
			this.cancelButton.SetActive(cancelButtonVisible);
		}

		private void ConfirmClicked(UIEvent e)
		{
			this.onConfirm();
		}

		private void CancelClicked(UIEvent e)
		{
			this.Cancel();
		}

		public void Initialize(MapProp mapProp, Action<int> onSelected)
		{
		}

		public void Cancel()
		{
			this.onCancel();
		}

		public GameObject buttonPrefab;

		public UIElement buttonArea;

		public GameObject confirmButton;

		public GameObject cancelButton;

		private Action<MapProp.Variation> selectionChanged;

		private Action onCancel;

		private Action onConfirm;
	}
}
