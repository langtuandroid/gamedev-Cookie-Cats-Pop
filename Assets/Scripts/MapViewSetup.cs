using System;
using System.Collections.Generic;
using UnityEngine;

public class MapViewSetup : ScriptableObject
{
	public AssetBundle AssetBundle { get; set; }

	public List<MapElementData> mapElementData = new List<MapElementData>();

	public List<Vector2> dotPositions = new List<Vector2>();

	public Vector2 totalContentSize;

	public MapIdentifier mapIdentifier;

	public List<string> optionalLowResSegmentsPaths = new List<string>();
}
