using System;
using System.Collections;

namespace TactileModules.SagaCore
{
	public static class GameViews
	{
		public static void Initialize(IGameViewProvider gameViewProvider)
		{
			GameViews.gameViewProvider = gameViewProvider;
		}

		public static ViewHandle<T> ShowView<T>() where T : class, IGameInterface
		{
			Type type = GameImplementors.FindImplementationForType<T>();
			if (type == null)
			{
			}
			return GameViews.gameViewProvider.ShowView<T>(type);
		}

		public static IEnumerator WaitForClose(IViewHandle handle)
		{
			yield return GameViews.gameViewProvider.WaitForClose(handle);
			yield break;
		}

		public static void CloseView(IViewHandle handle)
		{
			GameViews.gameViewProvider.CloseView(handle);
		}

		private static IGameViewProvider gameViewProvider;
	}
}
