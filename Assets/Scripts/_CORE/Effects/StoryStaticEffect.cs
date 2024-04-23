using System;
using UnityEngine;

public class StoryStaticEffect : MonoBehaviour
{
	private void Start()
	{
		this.material = base.GetComponent<MeshRenderer>().material;
	}

	private void Update()
	{
		if (this.timer < 0f)
		{
			this.timer = this.randomEvery;
			this.material.SetTextureOffset("_MainTex", new Vector2(this.randomScrollOffset.x * UnityEngine.Random.value, this.randomScrollOffset.y * UnityEngine.Random.value));
		}
		else
		{
			this.timer -= Time.deltaTime;
		}
	}

	public float randomEvery;

	public Vector2 randomScrollOffset;

	private Material material;

	private float timer;
}
