using System;
using System.Collections;

namespace TactileModules.SagaCore
{
	public interface IGameViewProvider : IGameInterface
	{
		ViewHandle<T> ShowView<T>(Type viewClass) where T : class, IGameInterface;

		void CloseView(IViewHandle handle);

		IEnumerator WaitForClose(IViewHandle handle);
	}
}
