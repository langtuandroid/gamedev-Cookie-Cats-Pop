using System;
using System.Collections;
using Fibers;

namespace TactileModules.MapFeature
{
	public abstract class MapFeatureHandler
	{
		public abstract bool IsParticipating { get; }

		public abstract void SwitchToFeatureMapView();

		public abstract IEnumerator ShowEndPopup();

		public abstract IEnumerator ShowStartPopup(EnumeratorResult<bool> viewClosingResult);

		public abstract IEnumerator ShowStartSessionPopup(EnumeratorResult<bool> viewClosingResult);

		public abstract string GetTimeLeftAsText();
	}
}
