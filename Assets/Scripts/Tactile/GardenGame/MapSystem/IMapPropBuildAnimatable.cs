using System;
using System.Collections;

namespace Tactile.GardenGame.MapSystem
{
	public interface IMapPropBuildAnimatable
	{
		void BuildInitialize(bool isTeardown);

		IEnumerator Build(bool isTeardown);
	}
}
