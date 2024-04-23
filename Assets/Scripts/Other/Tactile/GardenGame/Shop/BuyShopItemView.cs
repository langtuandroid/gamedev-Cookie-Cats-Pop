using System;
using System.Diagnostics;
using UnityEngine;

namespace Tactile.GardenGame.Shop
{
	public class BuyShopItemView : UIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ItemPurchased;



		public void Initialize(ShopItem item, ShopItemMetaData itemData, string quantityText)
		{
			this.dialogFrame.GetInstance<DialogFrame>().Title = L.Get(itemData.Title);
			this.description.text = L.Get(itemData.Description);
			this.quantity.text = quantityText;
			this.price.text = string.Format("{0} [C]", L.FormatNumber(item.CurrencyPrice));
			this.sprite.SpriteName = itemData.ImageSpriteName;
		}

		private void BuyButtonClicked(UIEvent e)
		{
			this.ItemPurchased();
		}

		private void CloseClicked(UIEvent e)
		{
			base.Close(0);
		}

		[SerializeField]
		private UIInstantiator dialogFrame;

		[SerializeField]
		private UILabel description;

		[SerializeField]
		private UILabel quantity;

		[SerializeField]
		private UILabel price;

		[SerializeField]
		private UISprite sprite;

		public Transform buyButtonTransform;
	}
}
