using System;
using System.Collections;
using Fibers;
using Tactile;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public abstract class BoosterLogic : MonoBehaviour
{
	private void RunAndDestroy(LevelSession session, Action whenDone)
	{
		this.logicFiber.Start(this.RunInternal(session, whenDone));
	}

	private IEnumerator RunInternal(LevelSession session, Action whenDone)
	{
		UICamera.DisableInput();
		yield return this.Logic(session);
		UICamera.EnableInput();
		whenDone();
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	protected abstract IEnumerator Logic(LevelSession session);

	public static IEnumerator ResolveBooster(InventoryItem boosterId, LevelSession levelSession, GameView gameView)
	{
		CPBoosterMetaData metaData = InventoryManager.Instance.GetMetaData<CPBoosterMetaData>(boosterId);
		return BoosterLogic.ResolveBooster(metaData.LogicPrefab, levelSession, gameView);
	}

	public static IEnumerator ResolveBooster(BoosterLogic boosterLogicPrefab, LevelSession levelSession, GameView gameView)
	{
		BoosterLogic boosterLogic = UnityEngine.Object.Instantiate<BoosterLogic>(boosterLogicPrefab);
		boosterLogic.GetElement().SetSizeAndDoLayout(gameView.GetElementSize());
		boosterLogic.transform.parent = gameView.transform;
		boosterLogic.transform.position = Vector3.back * 290f;
		boosterLogic.gameObject.SetLayerRecursively(gameView.gameObject.layer);
		bool done = false;
		boosterLogic.RunAndDestroy(levelSession, delegate
		{
			done = true;
		});
		while (!done)
		{
			yield return null;
		}
		yield break;
	}

	private readonly Fiber logicFiber = new Fiber();
}
