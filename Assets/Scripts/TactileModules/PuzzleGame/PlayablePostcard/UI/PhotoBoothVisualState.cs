using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
	public abstract class PhotoBoothVisualState : MonoBehaviour
	{
		public abstract IEnumerator AnimateIn();

		public abstract IEnumerator AnimateOut();
	}
}
