using System;
using System.Collections;
using Fibers;

namespace TactileModules.MapFeature
{
	public interface IMapFeatureProvider
	{
		void SwitchToFeatureMapView();

		IEnumerator ShowFeatureEndedView();

		IEnumerator ShowFeatureStartView(EnumeratorResult<bool> viewClosingResult);

		IEnumerator ShowFeatureStartSessionView(EnumeratorResult<bool> viewClosingResult);

		void Save();
	}
}
