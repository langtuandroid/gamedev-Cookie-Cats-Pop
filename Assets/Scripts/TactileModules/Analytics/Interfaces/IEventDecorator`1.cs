using System;

namespace TactileModules.Analytics.Interfaces
{
	public interface IEventDecorator<T> : IEventDecorator
	{
		void Decorate(T eventObject);
	}
}
