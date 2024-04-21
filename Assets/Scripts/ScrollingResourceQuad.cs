using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollingResourceQuad : MonoBehaviour
{
	private void Start()
	{
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.sharedMaterial;
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
