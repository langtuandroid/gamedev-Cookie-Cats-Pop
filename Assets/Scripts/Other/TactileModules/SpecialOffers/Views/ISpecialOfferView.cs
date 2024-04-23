using System;
using UnityEngine;

namespace TactileModules.SpecialOffers.Views
{
	public interface ISpecialOfferView : IUIView
	{
		event Action OnAcceptButtonClicked;

		event Action OnDismissButtonClicked;

		void SetTexture(Texture2D texture);

		void SetTimeLeft(string time);
	}
}
