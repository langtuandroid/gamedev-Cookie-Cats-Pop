using System;
using UnityEngine;

public class EmbeddedPrefabs : MonoBehaviour
{
	private void Awake()
	{
		this.UpdateEnabledState();
	}

	private void Start()
	{
		this.UpdateEnabledState();
	}

	private void OnEnable()
	{
		this.UpdateEnabledState();
	}

	private void UpdateEnabledState()
	{
		base.gameObject.SetActive(false);
	}
}
