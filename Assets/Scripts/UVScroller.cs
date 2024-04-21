using System;
using UnityEngine;

public class UVScroller : MonoBehaviour
{
	private void Start()
	{
		this.material = base.GetComponent<MeshRenderer>().material;
	}

	private void Update()
	{
		this.scrollOffset += this.scrollSpeed * Time.deltaTime;
		this.material.SetTextureOffset("_MainTex", this.scrollOffset);
	}

	public Vector2 scrollSpeed;

	private Material material;

	private Vector2 scrollOffset;
}
