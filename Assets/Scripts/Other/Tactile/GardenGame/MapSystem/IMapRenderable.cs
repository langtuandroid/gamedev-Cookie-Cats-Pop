using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public interface IMapRenderable
	{
		GameObject RenderObject(MapQuadRenderer renderer);
	}
}
