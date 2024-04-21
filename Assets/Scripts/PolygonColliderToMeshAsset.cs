using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonColliderToMeshAsset : MonoBehaviour
{
	public PolygonCollider2D PolygonCollider
	{
		get
		{
			return base.GetComponent<PolygonCollider2D>();
		}
	}

	public string savePath = "Assets/MaskMeshes/";

	public string assetName = string.Empty;
}
