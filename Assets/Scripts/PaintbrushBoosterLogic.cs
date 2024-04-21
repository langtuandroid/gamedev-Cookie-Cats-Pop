using System;
using System.Collections;
using UnityEngine;

public class PaintbrushBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		this.effectPivot.gameObject.SetActive(false);
		int layer = session.TurnLogic.Board.Root.gameObject.layer;
		SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("ColorChangeHelicopterEffect", this.helicopterSpawnPivot.position, layer, new object[]
		{
			session
		});
		yield return spawnedEffect.WaitUntilFinished();
		yield break;
	}

	[SerializeField]
	private GameObject effectPivot;

	[SerializeField]
	private Transform helicopterSpawnPivot;
}
