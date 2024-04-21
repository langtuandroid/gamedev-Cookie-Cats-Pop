using System;
using System.Collections;
using UnityEngine;

public class BossCannonController
{
	public BossCannonController(CannonOperator cannonOperator, CannonBallQueue cannonBallQueue)
	{
		this.cannonOperator = cannonOperator;
		this.cannonBallQueue = cannonBallQueue;
		this.originalCannonOperatorPos = cannonOperator.transform.position;
		this.originalCannonBallQueuePos = cannonBallQueue.transform.position;
		cannonOperator.transform.Translate(Vector3.down * 100f);
		cannonBallQueue.transform.Translate(Vector3.down * 100f);
	}

	public IEnumerator GetIntroZoomAndMovementAnimation()
	{
		Vector3 cannonOperatorStartPos = this.cannonOperator.transform.position;
		Vector3 cannonBallQueueStartPos = this.cannonBallQueue.transform.position;
		return FiberAnimation.Animate(1.5f, delegate(float t)
		{
			this.cannonOperator.transform.position = Vector3.Lerp(cannonOperatorStartPos, this.originalCannonOperatorPos, t);
			this.cannonBallQueue.transform.position = Vector3.Lerp(cannonBallQueueStartPos, this.originalCannonBallQueuePos, t);
		});
	}

	private readonly CannonOperator cannonOperator;

	private readonly CannonBallQueue cannonBallQueue;

	private readonly Vector3 originalCannonOperatorPos;

	private readonly Vector3 originalCannonBallQueuePos;
}
