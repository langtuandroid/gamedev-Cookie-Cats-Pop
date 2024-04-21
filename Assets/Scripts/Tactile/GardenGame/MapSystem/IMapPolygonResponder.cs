using System;

namespace Tactile.GardenGame.MapSystem
{
	public interface IMapPolygonResponder
	{
		void PolygonChanged(MapPolygon mapPolygon);
	}
}
