using System;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.MapSystem.Props;
using Tactile.GardenGame.MapSystem.Tiles;
using UnityEngine;

namespace TactileModules.GardenGame.MapSystem.Assets
{
	public class AssetModel : IAssetModel
	{
		public PropVariationPickerView GetPropVariationPickerView()
		{
			return Resources.Load<PropVariationPickerView>("ScapesMap/Assets/PropVariationPickerView");
		}

		public MainMapButtonsView GetStoryMapButtonsView()
		{
			return Resources.Load<MainMapButtonsView>("ScapesMap/Assets/StoryMapButtonsView");
		}

		public MapTileDatabase GetMapTileDatabase()
		{
			return Resources.Load<MapTileDatabase>("ScapesMap/Assets/MapTileDatabase");
		}

		public ActivateChoosePropIndicatorView GetActivateChoosePropIndicatorView()
		{
			return Resources.Load<ActivateChoosePropIndicatorView>("ScapesMap/Assets/ActivateChoosePropIndicatorView");
		}

		public ChoosePropView GetChoosePropView()
		{
			return Resources.Load<ChoosePropView>("ScapesMap/Assets/ChoosePropView");
		}

		public Material GetSelectedProp()
		{
			return Resources.Load<Material>("ScapesMap/Assets/SelectedProp");
		}

		public ChoosePropSettings GetChoosePropSettings()
		{
			return Resources.Load<ChoosePropSettings>("ScapesMap/Assets/ChoosePropSettings");
		}

		public QuitGameFromMapView GetQuitGameFromMapView()
		{
			return Resources.Load<QuitGameFromMapView>("ScapesMap/Assets/QuitGameFromMapView");
		}
	}
}
