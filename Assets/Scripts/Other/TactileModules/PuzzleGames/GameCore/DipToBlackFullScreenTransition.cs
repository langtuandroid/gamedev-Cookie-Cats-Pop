using System;
using System.Collections;

namespace TactileModules.PuzzleGames.GameCore
{
	public class DipToBlackFullScreenTransition : IFullScreenTransition
	{
		public IEnumerator TransitionOut()
		{
			UIViewManager.Instance.FrontFill = 0f;
			yield return UIViewManager.Instance.FadeCameraFrontFill(1f, 0f, 0);
			yield break;
		}

		public IEnumerator TransitionIn()
		{
			yield return UIViewManager.Instance.FadeCameraFrontFill(0f, 0f, 0);
			yield break;
		}

		public void FullyOut()
		{
			UIViewManager.Instance.CloseAll(new IUIView[0]);
		}
	}
}
