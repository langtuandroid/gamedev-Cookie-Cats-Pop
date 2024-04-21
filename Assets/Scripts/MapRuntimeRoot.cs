using System;
using System.Collections.Generic;
using UnityEngine;

public class MapRuntimeRoot : MonoBehaviour
{
	[SerializeField]
	public MapIdentifier mapIdentifier;

	public List<Transform> prefabsUsedFromLocalAssets;
}
