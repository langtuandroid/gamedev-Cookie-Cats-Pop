using System;
using JetBrains.Annotations;

namespace TactileModules.SagaCore
{
	[UsedImplicitly]
	public class ViewHandle<T> : IViewHandle where T : IGameInterface
	{
		public object Handle { get; set; }

		public T view;
	}
}
