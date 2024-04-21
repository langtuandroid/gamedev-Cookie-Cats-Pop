using System;

namespace Tactile.GardenGame.Story
{
	public interface IMapActionLocalizable
	{
		string GetLocalizableText();

		string GetContext();
	}
}
