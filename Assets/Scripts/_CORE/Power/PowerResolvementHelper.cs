using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class PowerResolvementHelper
{
	public static void DoPower(Tile origin, IHitResolver resolver, TopologyHelper topologyHelper, PowerResolvementHelper.TileAnimation tileAnimation, float delay = 0f, float effectDelay = 0f)
	{
		foreach (TopologyHelper.DistanceTile distanceTile in topologyHelper.GetDistanceTiles(origin))
		{
			resolver.MarkHit(distanceTile.tile, HitCause.Power, delay + PowerResolvementHelper.DistanceToTime(distanceTile.distance));
			if (tileAnimation != null)
			{
				resolver.QueueEffect(tileAnimation(distanceTile.tile), effectDelay + PowerResolvementHelper.DistanceToTime(distanceTile.distance));
			}
		}
	}

	public static void DoEffects(Tile origin, IHitResolver resolver, TopologyHelper topologyHelper, PowerResolvementHelper.TileAnimation tileAnimation, float delay = 0f)
	{
		foreach (TopologyHelper.DistanceTile distanceTile in topologyHelper.GetDistanceTiles(origin))
		{
			resolver.QueueEffect(tileAnimation(distanceTile.tile), delay + PowerResolvementHelper.DistanceToTime(distanceTile.distance));
		}
	}

	public static float DistanceToTime(float distance)
	{
		return distance / 100f * 0.1f;
	}

	private static IEnumerator AnimateNotesHitEffect(Tile origin, TopologyHelper topology, int width, int height)
	{
		Dictionary<int, List<Vector3>> dictionary = new Dictionary<int, List<Vector3>>();
		foreach (Tile tile in topology.GetTiles())
		{
			int y = tile.Coord.y;
			List<Vector3> list;
			if (!dictionary.TryGetValue(y, out list))
			{
				list = new List<Vector3>();
				dictionary.Add(y, list);
			}
			list.Add(tile.WorldPosition);
		}
		NoteHitEffect noteHitEffect = PowerResolvementHelper.InstantiateEffect(origin, "NoteHitEffect") as NoteHitEffect;
		noteHitEffect.noteMeshNodesWorldPositions = new Vector3[dictionary.Count];
		float num = 0f;
		int num2 = 0;
		float num3 = -1000f;
		foreach (KeyValuePair<int, List<Vector3>> keyValuePair in dictionary)
		{
			Vector3 vector = default(Vector3);
			float x = keyValuePair.Value[0].x;
			float num4 = x;
			foreach (Vector3 vector2 in keyValuePair.Value)
			{
				if (vector2.x < x)
				{
					x = vector2.x;
				}
				if (vector2.x > num4)
				{
					num4 = vector2.x;
				}
			}
			num = Mathf.Max(num4 - x, num);
			vector = keyValuePair.Value[0];
			if (width < 4)
			{
				if (num3 < -999f)
				{
					num3 = (x + num4) * 0.5f;
				}
				vector.x = num3 + Mathf.Sin((float)keyValuePair.Key * 90f * 0.0174532924f) * ((width != 1) ? 5f : 20f);
			}
			else
			{
				vector.x = (x + num4) * 0.5f;
			}
			noteHitEffect.noteMeshNodesWorldPositions[num2++] = vector;
		}
		noteHitEffect.numberOfNotes = height;
		if (num < 50f)
		{
			noteHitEffect.noteMeshWidth = 50f;
		}
		else
		{
			if (width > 3)
			{
				for (int i = 0; i < noteHitEffect.noteMeshNodesWorldPositions.Length; i++)
				{
					noteHitEffect.noteMeshNodesWorldPositions[i].x = noteHitEffect.noteMeshNodesWorldPositions[0].x;
				}
			}
			noteHitEffect.noteMeshWidth = num;
		}
		yield break;
	}

	public static IEnumerator AnimateTileSplashEffect(Tile atTile, string effectPrefab)
	{
		PowerResolvementHelper.InstantiateEffect(atTile, effectPrefab);
		yield break;
	}

	public static SpawnedEffect InstantiateEffect(Tile atTile, string effectPrefab)
	{
		SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect(effectPrefab, Vector3.zero, PowerResolvementHelper.Layer, new object[0]);
		spawnedEffect.transform.localScale = PowerResolvementHelper.scale;
		spawnedEffect.transform.localPosition = PowerResolvementHelper.panningRoot.InverseTransformPoint(atTile.WorldPosition) + Vector3.back;
		spawnedEffect.gameObject.SetLayerRecursively(PowerResolvementHelper.Layer);
		return spawnedEffect;
	}

	public static IEnumerator BurnEffect(Tile atTile)
	{
		return PowerResolvementHelper.AnimateTileSplashEffect(atTile, "BurnEffect");
	}

	public static IEnumerator FrogHitEffect(Tile atTile)
	{
		return PowerResolvementHelper.AnimateTileSplashEffect(atTile, "FrogHitEffect");
	}

	public static IEnumerator NinjaStarHitEffect(Tile atTile)
	{
		return PowerResolvementHelper.AnimateTileSplashEffect(atTile, "NinjaStarHitEffect");
	}

	public static IEnumerator NinjaStarSingleHitEffect(Tile atTile)
	{
		return PowerResolvementHelper.AnimateTileSplashEffect(atTile, "NinjaStarSingleHitEffect");
	}

	public static void DoExplosion(Tile origin, IHitResolver resolver, int radius)
	{
		TopologyHelper topologyHelper = new TopologyHelper();
		topologyHelper.AddCircle(origin, radius);
		resolver.QueueEffect(FiberHelper.RunDelayed(0f, delegate
		{
			SingletonAsset<SoundDatabase>.Instance.burn.Play();
		}), 0f);
		TopologyHelper topologyHelper2 = topologyHelper;
		if (PowerResolvementHelper._003C_003Ef__mg_0024cache0 == null)
		{
			PowerResolvementHelper._003C_003Ef__mg_0024cache0 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.BurnEffect);
		}
		PowerResolvementHelper.DoPower(origin, resolver, topologyHelper2, PowerResolvementHelper._003C_003Ef__mg_0024cache0, 1.5f, 0f);
	}

	public static void DoFrogLine(Tile origin, IHitResolver resolver, int height = 1, int distance = 0)
	{
		TopologyHelper topologyHelper = new TopologyHelper();
		topologyHelper.AddRow(origin, height, distance);
		resolver.QueueEffect(FiberHelper.RunDelayed(0.5f, delegate
		{
			if (SingletonAsset<SoundDatabase>.Instance.frog != null)
			{
				SingletonAsset<SoundDatabase>.Instance.frog.Play();
			}
		}), 0f);
		resolver.QueueEffect(PowerResolvementHelper.FrogHitEffect(origin), 0f);
		float num = 1.2f;
		foreach (TopologyHelper.DistanceTile distanceTile in topologyHelper.GetDistanceTiles(origin))
		{
			resolver.MarkHit(distanceTile.tile, HitCause.Power, num + PowerResolvementHelper.DistanceToTime(distanceTile.distance) * 0.7f);
		}
	}

	public static void DoNotesColumn(Tile origin, IHitResolver resolver, int width, int height, bool hitTiles = true, float delay = 0f)
	{
		TopologyHelper topologyHelper = new TopologyHelper();
		topologyHelper.AddColumn(origin, width, height);
		resolver.QueueEffect(PowerResolvementHelper.AnimateNotesHitEffect(origin, topologyHelper, width, height), delay);
		if (hitTiles)
		{
			delay += 0.1f;
			foreach (TopologyHelper.DistanceTile distanceTile in topologyHelper.GetDistanceTiles(origin))
			{
				resolver.MarkHit(distanceTile.tile, HitCause.Power, delay + PowerResolvementHelper.DistanceToTime(distanceTile.distance) * 1.2f);
			}
		}
	}

	public static void DoNinjaStarCircle(Tile origin, IHitResolver resolver, int radius)
	{
		TopologyHelper topologyHelper = new TopologyHelper();
		topologyHelper.AddCircle(origin, radius);
		resolver.QueueEffect(PowerResolvementHelper.AnimateTileSplashEffect(origin, "NinjaStarHitEffect"), 0f);
		float num = 0f;
		foreach (TopologyHelper.DistanceTile distanceTile in topologyHelper.GetDistanceTiles(origin))
		{
			resolver.MarkHit(distanceTile.tile, HitCause.Power, num + PowerResolvementHelper.DistanceToTime(distanceTile.distance));
		}
	}

	public static void DoFinalPower(Tile origin, IHitResolver resolver, int height)
	{
		TopologyHelper topologyHelper = new TopologyHelper();
		topologyHelper.AddRow(origin, height, 0);
		FinalPowerHitEffect finalPowerHitEffect = PowerResolvementHelper.InstantiateEffect(origin, "FinalPowerHitEffect") as FinalPowerHitEffect;
		finalPowerHitEffect.transform.localScale = Vector3.one;
		Vector3 boardRowNrWorldPosition = (origin.Board as GameBoard).GetBoardRowNrWorldPosition(origin.Coord.y);
		finalPowerHitEffect.SetCenter(boardRowNrWorldPosition);
		foreach (TopologyHelper.DistanceTile distanceTile in topologyHelper.GetDistanceTiles(origin))
		{
			float delayFromWorldPos = finalPowerHitEffect.GetDelayFromWorldPos(distanceTile.tile.WorldPosition);
			resolver.MarkHit(distanceTile.tile, HitCause.Power, delayFromWorldPos);
			if (distanceTile.tile.Piece != null)
			{
				resolver.QueueAnimation(PowerResolvementHelper.AnimatePop(distanceTile.tile.WorldPosition), delayFromWorldPos);
			}
		}
	}

	private static IEnumerator AnimatePop(Vector3 worldPosition)
	{
		EffectPool.Instance.SpawnEffect("SharkEatBubbleEffect", worldPosition, PowerResolvementHelper.Layer, new object[0]);
		yield break;
	}

	public static int Layer;

	public static Vector3 scale = Vector3.one;

	public static Transform panningRoot;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache0;

	public delegate IEnumerator TileAnimation(Tile tile);
}
