using System;
using JetBrains.Annotations;

using UnityEngine;

namespace TactileModules.Profiling
{
	[UsedImplicitly]
	public class TactileProfilerCommandHandler : BaseCommandHandler
	{
		[UsedImplicitly]

		private static void LogSamples()
		{
			UnityEngine.Debug.Log(TactileProfiler.ConstructProfileData());
		}
	}
}
