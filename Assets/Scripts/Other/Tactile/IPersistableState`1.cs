using System;

namespace Tactile
{
	public interface IPersistableState<T> : IPersistableState
	{
		void MergeFromOther(T newest, T last);
	}
}
