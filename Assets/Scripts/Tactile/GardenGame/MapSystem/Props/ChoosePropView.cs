using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class ChoosePropView : UIView
	{
		public void Initialize(Action<int> selectionChanged, Action confirm, Action cancel, bool cancelButtonVisible)
		{
			this.selectionChanged = selectionChanged;
			this.confirm = confirm;
			this.cancel = cancel;
			this.cancelButton.SetActive(cancelButtonVisible);
			this.confirmButton.SetActive(false);
		}

		public void Refresh(MapProp prop)
		{
			this.ClearButtons();
			foreach (MapProp.Variation variation in prop.Variations)
			{
				if (variation.IsPickable)
				{
					MapProp.Variation p = variation;
					this.CreateButton(variation.Icon.Texture, p.ID, delegate
					{
						this.selectionChanged(p.ID);
						this.RefreshSelection(p.ID);
						this.confirmButton.SetActive(true);
					});
				}
			}
			this.layoutContainer.DoLayout();
			this.RefreshSelection(prop.CurrentVariationId);
		}

		private void RefreshSelection(int variationId)
		{
			foreach (ChoosePropVariationButton choosePropVariationButton in this.buttons)
			{
				choosePropVariationButton.Selected = (choosePropVariationButton.Id == variationId);
			}
		}

		private void CreateButton(Texture2D icon, int id, Action onClicked)
		{
			ChoosePropVariationButton choosePropVariationButton = UnityEngine.Object.Instantiate<ChoosePropVariationButton>(this.buttonPrefab);
			choosePropVariationButton.Id = id;
			this.buttons.Add(choosePropVariationButton);
			choosePropVariationButton.Button.Clicked += delegate(UIButton b)
			{
				onClicked();
			};
			choosePropVariationButton.Refresh(icon);
			choosePropVariationButton.name = this.buttons.Count.ToString();
			choosePropVariationButton.transform.parent = this.layoutContainer.transform;
		}

		private void ClearButtons()
		{
			foreach (ChoosePropVariationButton choosePropVariationButton in this.buttons)
			{
				UnityEngine.Object.Destroy(choosePropVariationButton.gameObject);
			}
			this.buttons.Clear();
		}

		private void ConfirmClicked(UIEvent e)
		{
			this.confirm();
		}

		private void CancelClicked(UIEvent e)
		{
			this.cancel();
		}

		[SerializeField]
		private ChoosePropVariationButton buttonPrefab;

		[SerializeField]
		private UIElement layoutContainer;

		[SerializeField]
		private GameObject confirmButton;

		[SerializeField]
		private GameObject cancelButton;

		private Action<int> selectionChanged;

		private Action confirm;

		private Action cancel;

		private readonly List<ChoosePropVariationButton> buttons = new List<ChoosePropVariationButton>();
	}
}
