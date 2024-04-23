using System;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelAreas : MonoBehaviour
{
	public Vector3 IntroStartPos
	{
		get
		{
			return this.introStartPoint.position;
		}
	}

	public Vector3 IntroSuckKittensPos
	{
		get
		{
			return this.introSuckKittensPoint.position;
		}
	}

	public Vector3 IntroCompletePos
	{
		get
		{
			return this.introCompletePoint.position;
		}
	}

	public void EnableKittens(int count)
	{
		int num = Mathf.Clamp(count, 0, this.kittenInstantiators.Count);
		for (int i = 0; i < num; i++)
		{
			this.kittenInstantiators[i].gameObject.SetActive(true);
		}
	}

	public BossLevelIntroKitten GetKitten(int idx)
	{
		return this.kittenInstantiators[idx].GetInstance<BossLevelIntroKitten>();
	}

	[SerializeField]
	private List<Instantiator> kittenInstantiators;

	[SerializeField]
	private Transform introStartPoint;

	[SerializeField]
	private Transform introSuckKittensPoint;

	[SerializeField]
	private Transform introCompletePoint;
}
