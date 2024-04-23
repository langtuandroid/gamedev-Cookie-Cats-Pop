using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIElement))]
public abstract class MapBuilderBase : MonoBehaviour
{
	public List<GameObject> DotPrefabs
	{
		get
		{
			return this.dotPrefabs;
		}
	}

	protected abstract bool ConfigureMapInstantiatorForDot(MapInstantiator mapInstantiator, int dotIndex, MapSettings settings, LevelDatabase levelDatabase);

	public MapIdentifier mapIdentifier;

	public Transform mapSegmentsRoot;

	public Transform dotsRoot;

	public bool showPortraitBorderGizmos = true;

	[SerializeField]
	protected float mapRootPositionY;

	[SerializeField]
	[HideInInspector]
	private List<GameObject> dotPrefabs;
}
