using System;
using System.Collections.Generic;
using UnityEngine;

public class KittenArea : MonoBehaviour
{
	private Vector3 GetRandomWalkingPosition()
	{
		float value = UnityEngine.Random.value;
		float value2 = UnityEngine.Random.value;
		Vector3 a = Vector3.Lerp(this.walkingAreaQuadVertices[0].localPosition, this.walkingAreaQuadVertices[1].localPosition, value);
		Vector3 b = Vector3.Lerp(this.walkingAreaQuadVertices[3].localPosition, this.walkingAreaQuadVertices[2].localPosition, value);
		return Vector3.Lerp(a, b, value2);
	}

	public void Initialize(int numberOfKittens = 5)
	{
		this.kittens = new List<WalkingKitten>();
		for (int i = 0; i < numberOfKittens; i++)
		{
			WalkingKitten component = UnityEngine.Object.Instantiate<GameObject>(this.walkingKittenPrefab).GetComponent<WalkingKitten>();
			component.transform.parent = base.transform;
			component.transform.localScale = Vector3.one;
			component.gameObject.SetLayerRecursively(base.gameObject.layer);
			component.Initialize(new Func<Vector3>(this.GetRandomWalkingPosition));
			this.kittens.Add(component);
		}
		this.walkingKittenPrefab.SetActive(false);
	}

	private void Update()
	{
		if (this.kittens == null)
		{
			return;
		}
		for (int i = 0; i < this.kittens.Count; i++)
		{
			this.kittens[i].Step();
		}
		this.kittens.Sort((WalkingKitten a, WalkingKitten b) => a.transform.localPosition.y.CompareTo(b.transform.localPosition.y));
		float num = 1f / (float)this.kittens.Count;
		for (int j = 0; j < this.kittens.Count; j++)
		{
			Vector3 localPosition = this.kittens[j].transform.localPosition;
			localPosition.z = Mathf.Lerp(this.zSortSpan.x, this.zSortSpan.y, num * (float)j);
			this.kittens[j].transform.localPosition = localPosition;
		}
	}

	public Transform[] walkingAreaQuadVertices;

	public GameObject walkingKittenPrefab;

	public Vector2 zSortSpan;

	private List<WalkingKitten> kittens;
}
