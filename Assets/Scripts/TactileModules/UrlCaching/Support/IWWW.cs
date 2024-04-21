using System;
using System.Collections;

namespace TactileModules.UrlCaching.Support
{
	public interface IWWW
	{
		string Error { get; }

		byte[] Bytes { get; }

		IEnumerator WaitForCompletion();
	}
}
