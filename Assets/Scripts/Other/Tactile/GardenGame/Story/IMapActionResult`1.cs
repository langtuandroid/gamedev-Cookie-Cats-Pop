using System;

namespace Tactile.GardenGame.Story
{
	public interface IMapActionResult<T> : IMapActionResultBase
	{
		void SendResult(T result);
	}
}
