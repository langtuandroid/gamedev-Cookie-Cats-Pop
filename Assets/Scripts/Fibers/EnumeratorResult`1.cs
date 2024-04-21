using System;

namespace Fibers
{
	public class EnumeratorResult<T>
	{
		public static implicit operator T(EnumeratorResult<T> t)
		{
			return t.value;
		}

		public T value;
	}
}
