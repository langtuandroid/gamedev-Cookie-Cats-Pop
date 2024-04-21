using System;
using UnityEngine;

[Serializable]
public class MapElementData
{
	public string prefabName;

	public Vector3 position;

	public float topEdge;

	public float bottomEdge;

	public Vector3 scale;

	public Quaternion rotation;

	public string instantiatorDataAsJSON;

	public bool usingAssetBundle;
}
