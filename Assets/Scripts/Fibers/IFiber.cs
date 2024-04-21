using System;
using System.Collections;

namespace Fibers
{
	public interface IFiber
	{
		void Start(IEnumerator method);

		void Terminate();

		bool Step();

		bool IsTerminated { get; }
	}
}
