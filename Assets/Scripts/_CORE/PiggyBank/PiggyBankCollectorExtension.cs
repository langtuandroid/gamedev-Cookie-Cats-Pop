using System;
using Shared.PiggyBank.Module.Interfaces;
using UnityEngine;

public class PiggyBankCollectorExtension : MonoBehaviour, IPiggyBankCollector
{
	public void Activate(string animationName)
	{
		base.gameObject.SetActive(true);
		this.piggyInstantiator.GetInstance<SkeletonAnimation>().PlayAnimation(0, animationName, false, false);
	}

	[SerializeField]
	private SpineInstantiator piggyInstantiator;
}
