using System;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilderPuzzle : MapBuilderBase
{
	protected override bool ConfigureMapInstantiatorForDot(MapInstantiator mapInstantiator, int dotIndex, MapSettings settings, LevelDatabase levelDatabase)
	{
		if (dotIndex >= levelDatabase.EditorGetLevels().Count)
		{
			return false;
		}
		LevelProxy level = levelDatabase.GetLevel(dotIndex);
		int index = (!(level.LevelMetaData is GateMetaData)) ? 0 : 1;
		mapInstantiator.Prefab = this.MapDotPrefabs[index].gameObject;
		mapInstantiator.CreateInstance();
		MapDot instance = mapInstantiator.GetInstance<MapDot>();
		instance.LevelId = dotIndex;
		AnimatedButton component = instance.GetComponent<AnimatedButton>();
		if (component != null)
		{
			component.ConstantWobble = false;
		}
		return true;
	}

	[Header("Map Builder")]
	[SerializeField]
	private List<MapDot> MapDotPrefabs = new List<MapDot>();

	public int FarthestUnlockedLevel = 1;
}
