using System;
using UnityEngine;

public static class ZLayerExtension
{
	public static float Z(this ZLayer sorting)
	{
		return (float)(-(float)sorting) / 100f;
	}

	public static ZSorter ZSorter(this Component c)
	{
		return c.GetComponent<ZSorter>();
	}

	public static ZSorter ZSorter(this GameObject go)
	{
		return go.GetComponent<ZSorter>();
	}

	public static void AdjustTransform(this ZLayer layer, Transform t)
	{
		Vector3 position = t.position;
		position.z = layer.Z();
		t.position = position;
	}
}
