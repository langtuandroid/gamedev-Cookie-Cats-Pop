using System;
using UnityEngine;

public abstract class MapDotBase : MonoBehaviour
{
	public abstract bool IsUnlocked { get; }

	public abstract bool IsCompleted { get; }

	public abstract int LevelId { get; set; }

	public abstract void Initialize();

	public abstract void UpdateUI();
}
