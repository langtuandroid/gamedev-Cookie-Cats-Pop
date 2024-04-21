using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedKittens : MonoBehaviour
{
	public void Initialize(LevelSession session)
	{
		this.session = session;
		this.kittens = new List<SavedKitten>();
		session.TurnLogic.PieceCleared += this.HandlePieceCleared;
		session.BossLevelController.OnBossHit += this.OnBossHit;
	}

	private void OnDestroy()
	{
		this.Disable();
	}

	public void Disable()
	{
		this.session.TurnLogic.PieceCleared -= this.HandlePieceCleared;
		this.session.BossLevelController.OnBossHit -= this.OnBossHit;
	}

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		GoalPiece goalPiece = piece as GoalPiece;
		if (goalPiece != null)
		{
			this.SpawnKitten(goalPiece.transform.position);
		}
	}

	private void OnBossHit(Vector3 position)
	{
		this.SpawnKitten(position);
	}

	private void SpawnKitten(Vector3 pos)
	{
		SavedKitten savedKitten = UnityEngine.Object.Instantiate<SavedKitten>(this.savedKittenPrefab);
		savedKitten.Initialize(this, pos);
		this.kittens.Add(savedKitten);
		if (this.KittenSpawned != null)
		{
			this.KittenSpawned(savedKitten);
		}
	}

	public Vector3 GetRandomWalkingPosition()
	{
		float value = UnityEngine.Random.value;
		float value2 = UnityEngine.Random.value;
		Vector3 a = Vector3.Lerp(this.walkingAreaQuadVertices[0].localPosition, this.walkingAreaQuadVertices[1].localPosition, value);
		Vector3 b = Vector3.Lerp(this.walkingAreaQuadVertices[3].localPosition, this.walkingAreaQuadVertices[2].localPosition, value);
		return Vector3.Lerp(a, b, value2);
	}

	public IEnumerator WaitForAllLanded()
	{
		for (;;)
		{
			bool allLanded = true;
			foreach (SavedKitten savedKitten in this.kittens)
			{
				if (!savedKitten.IsLanded)
				{
					allLanded = false;
				}
			}
			if (allLanded)
			{
				break;
			}
			yield return null;
		}
		yield break;
	}

	private void Update()
	{
		if (this.kittens == null)
		{
			return;
		}
		if (this.kittens.Count == 0)
		{
			return;
		}
		this.onUpdate();
		this.kittens.Sort(delegate(SavedKitten a, SavedKitten b)
		{
			Vector3 localPosition2 = a.transform.localPosition;
			Vector3 localPosition3 = b.transform.localPosition;
			if (Mathf.Approximately(localPosition2.y, localPosition3.y))
			{
				return localPosition2.x.CompareTo(localPosition3.x);
			}
			return localPosition3.y.CompareTo(localPosition2.y);
		});
		float num = 1f / (float)this.kittens.Count;
		for (int i = 0; i < this.kittens.Count; i++)
		{
			Vector3 localPosition = this.kittens[i].transform.localPosition;
			localPosition.z = Mathf.Lerp(this.zSortSpan.x, this.zSortSpan.y, num * (float)i);
			this.kittens[i].transform.localPosition = localPosition;
		}
	}

	private List<SavedKitten> kittens;

	private LevelSession session;

	public SavedKitten savedKittenPrefab;

	public Transform[] walkingAreaQuadVertices;

	public Vector2 zSortSpan;

	public Action onUpdate = delegate()
	{
	};

	public Action<SavedKitten> KittenSpawned;
}
