using System;
using System.Collections;

public interface IHitResolver
{
	void QueueAnimation(IEnumerator func, float delay = 0f);

	void MarkForRemoval(float delay = 0f, int pointsOverride = -1);

	void MarkForReplacement(PieceId newId, float delay = 0f);

	bool MarkHit(Tile at, HitCause cause, float delay = 0f);

	HitMark Hit { get; }

	void QueueEffect(IEnumerator func, float delay = 0f);
}
