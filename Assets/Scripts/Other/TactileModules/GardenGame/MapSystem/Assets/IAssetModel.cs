using System;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.MapSystem.Props;
using Tactile.GardenGame.MapSystem.Tiles;
using UnityEngine;

namespace TactileModules.GardenGame.MapSystem.Assets
{
	public interface IAssetModel
	{
		PropVariationPickerView GetPropVariationPickerView();

		MainMapButtonsView GetStoryMapButtonsView();

		MapTileDatabase GetMapTileDatabase();

		ActivateChoosePropIndicatorView GetActivateChoosePropIndicatorView();

		ChoosePropView GetChoosePropView();

		Material GetSelectedProp();

		ChoosePropSettings GetChoosePropSettings();

		QuitGameFromMapView GetQuitGameFromMapView();
	}
}
