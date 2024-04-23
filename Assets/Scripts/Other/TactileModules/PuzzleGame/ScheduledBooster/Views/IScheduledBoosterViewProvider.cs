using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.Views
{
	public interface IScheduledBoosterViewProvider
	{
		IEnumerator AnimateCoins(UIView uiView, Vector3 buttonPos);

		IEnumerator AnimateCoinsBack(UIView uiView, Vector3 buttonPos);

		IEnumerator ShowShopView();
	}
}
