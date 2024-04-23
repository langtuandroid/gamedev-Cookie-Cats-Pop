using System;
using UnityEngine;

namespace TactileModules.SpecialOffers.Views
{
	public class SpecialOfferIapView : SpecialOfferView, ISpecialOfferIapView, ISpecialOfferView, IUIView
	{
		public void SetPriceNow(string price)
		{
			this.priceNow.text = price;
		}

		public void SetPriceBefore(string price)
		{
			this.priceBefore.text = price;
			if (string.IsNullOrEmpty(price))
			{
				this.priceBefore.gameObject.SetActive(false);
			}
		}

		[SerializeField]
		private UILabel priceNow;

		[SerializeField]
		private UILabel priceBefore;
	}
}
