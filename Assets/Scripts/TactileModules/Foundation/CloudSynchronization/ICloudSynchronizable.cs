using System;
using System.Collections;

namespace TactileModules.Foundation.CloudSynchronization
{
	public interface ICloudSynchronizable
	{
		IEnumerator Synchronize();
	}
}
