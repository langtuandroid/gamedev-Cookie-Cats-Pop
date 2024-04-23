using System;
using System.Collections;

namespace Fibers
{
	public interface IFiberRunnable
	{
		IEnumerator Run();

		void OnExit();
	}
}
