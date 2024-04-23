using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class PowerCombinationLogic
{
	public static void DoPower(PowerCombination combination, Tile tile, IHitResolver resolver)
	{
		PowerCombinationLogic.combinationLogic[combination](tile, resolver);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static PowerCombinationLogic()
	{
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary = new Dictionary<PowerCombination, Action<Tile, IHitResolver>>();
		dictionary.Add(new PowerCombination(new PowerColor[1]), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoNinjaStarCircle(tile, resolver, 1);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Green
		}), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoFrogLine(tile, resolver, 1, 0);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Blue
		}), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoNotesColumn(tile, resolver, 1, 7, true, 0f);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Red
		}), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoExplosion(tile, resolver, 2);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Red,
			PowerColor.Green
		}), delegate(Tile tile, IHitResolver resolver)
		{
			TopologyHelper topologyHelper = new TopologyHelper();
			topologyHelper.AddCircle(tile, 2);
			topologyHelper.AddRow(tile, 3, 0);
			TopologyHelper topologyHelper2 = topologyHelper;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache0 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache0 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.BurnEffect);
			}
			PowerResolvementHelper.DoPower(tile, resolver, topologyHelper2, PowerCombinationLogic._003C_003Ef__mg_0024cache0, 1.5f, 0f);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Red,
			PowerColor.Blue
		}), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoExplosion(tile, resolver, 2);
			PowerResolvementHelper.DoNotesColumn(tile, resolver, 3, 7, true, 0f);
		});
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary2 = dictionary;
		PowerColor[] array = new PowerColor[2];
		array[0] = PowerColor.Red;
		dictionary2.Add(new PowerCombination(array), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoExplosion(tile, resolver, 2);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Green,
			PowerColor.Blue
		}), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoFrogLine(tile, resolver, 3, 0);
			PowerResolvementHelper.DoNotesColumn(tile, resolver, 3, 7, true, 0f);
		});
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary3 = dictionary;
		PowerColor[] array2 = new PowerColor[2];
		array2[0] = PowerColor.Green;
		dictionary3.Add(new PowerCombination(array2), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoNinjaStarCircle(tile, resolver, 1);
			PowerResolvementHelper.DoFrogLine(tile, resolver, 1, 0);
		});
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary4 = dictionary;
		PowerColor[] array3 = new PowerColor[2];
		array3[0] = PowerColor.Blue;
		dictionary4.Add(new PowerCombination(array3), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoNinjaStarCircle(tile, resolver, 1);
			PowerResolvementHelper.DoNotesColumn(tile, resolver, 1, 7, true, 0f);
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Red,
			PowerColor.Green,
			PowerColor.Blue
		}), delegate(Tile tile, IHitResolver resolver)
		{
			TopologyHelper topologyHelper = new TopologyHelper();
			topologyHelper.AddRow(tile, 5, 0);
			List<Tile> sortedTiles = topologyHelper.GetSortedTiles(delegate(Tile a, Tile b)
			{
				if (a.Coord.y == b.Coord.y)
				{
					return Mathf.Abs(a.Coord.x - 5).CompareTo(Mathf.Abs(b.Coord.x - 5));
				}
				return b.Coord.y.CompareTo(a.Coord.y);
			});
			if (sortedTiles.Count > 0)
			{
				PowerResolvementHelper.DoNotesColumn(sortedTiles[0], resolver, 20, 5, false, 0f);
			}
			foreach (List<Tile> list in topologyHelper.GetRowTiles())
			{
				list.Sort((Tile a, Tile b) => Mathf.Abs(a.Coord.x - 5).CompareTo(Mathf.Abs(b.Coord.x - 5)));
				if (list.Count != 0)
				{
					resolver.QueueEffect(PowerResolvementHelper.FrogHitEffect(list[0]), 1f);
				}
			}
			TopologyHelper topologyHelper2 = new TopologyHelper();
			topologyHelper2.AddRow(tile, 5, 0);
			Tile leftMostTile = topologyHelper2.GetLeftMostTile(tile.LocalPosition.y);
			Tile origin = leftMostTile;
			TopologyHelper topologyHelper3 = topologyHelper2;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache1 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache1 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.BurnEffect);
			}
			PowerResolvementHelper.DoPower(origin, resolver, topologyHelper3, PowerCombinationLogic._003C_003Ef__mg_0024cache1, 2.5f, 2.5f);
		});
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary5 = dictionary;
		PowerColor[] array4 = new PowerColor[3];
		array4[0] = PowerColor.Red;
		array4[1] = PowerColor.Green;
		dictionary5.Add(new PowerCombination(array4), delegate(Tile tile, IHitResolver resolver)
		{
			TopologyHelper topologyHelper = new TopologyHelper();
			topologyHelper.AddRow(tile, 7, 0);
			Tile origin = tile;
			TopologyHelper topologyHelper2 = topologyHelper;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache2 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache2 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.NinjaStarSingleHitEffect);
			}
			PowerResolvementHelper.DoEffects(origin, resolver, topologyHelper2, PowerCombinationLogic._003C_003Ef__mg_0024cache2, 0f);
			TopologyHelper topologyHelper3 = new TopologyHelper();
			topologyHelper3.AddRow(tile, 7, 0);
			foreach (List<Tile> list in topologyHelper3.GetRowTiles())
			{
				list.Sort((Tile a, Tile b) => Mathf.Abs(a.Coord.x - 5).CompareTo(Mathf.Abs(b.Coord.x - 5)));
				if (list.Count != 0)
				{
					resolver.QueueEffect(PowerResolvementHelper.FrogHitEffect(list[0]), 0.75f);
				}
			}
			TopologyHelper topologyHelper4 = new TopologyHelper();
			topologyHelper4.AddRow(tile, 7, 0);
			Tile leftMostTile = topologyHelper4.GetLeftMostTile(tile.LocalPosition.y);
			Tile origin2 = leftMostTile;
			TopologyHelper topologyHelper5 = topologyHelper4;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache3 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache3 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.BurnEffect);
			}
			PowerResolvementHelper.DoPower(origin2, resolver, topologyHelper5, PowerCombinationLogic._003C_003Ef__mg_0024cache3, 2.25f, 2.25f);
		});
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary6 = dictionary;
		PowerColor[] array5 = new PowerColor[3];
		array5[0] = PowerColor.Red;
		array5[1] = PowerColor.Blue;
		dictionary6.Add(new PowerCombination(array5), delegate(Tile tile, IHitResolver resolver)
		{
			TopologyHelper topologyHelper = new TopologyHelper();
			topologyHelper.AddRow(tile, 7, 0);
			Tile origin = tile;
			TopologyHelper topologyHelper2 = topologyHelper;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache4 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache4 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.NinjaStarSingleHitEffect);
			}
			PowerResolvementHelper.DoEffects(origin, resolver, topologyHelper2, PowerCombinationLogic._003C_003Ef__mg_0024cache4, 0f);
			List<Tile> sortedTiles = topologyHelper.GetSortedTiles(delegate(Tile a, Tile b)
			{
				if (a.Coord.y == b.Coord.y)
				{
					return Mathf.Abs(a.Coord.x - 5).CompareTo(Mathf.Abs(b.Coord.x - 5));
				}
				return b.Coord.y.CompareTo(a.Coord.y);
			});
			if (sortedTiles.Count > 0)
			{
				PowerResolvementHelper.DoNotesColumn(sortedTiles[0], resolver, 20, 7, false, 0.75f);
			}
			TopologyHelper topologyHelper3 = new TopologyHelper();
			topologyHelper3.AddRow(tile, 7, 0);
			Tile leftMostTile = topologyHelper3.GetLeftMostTile(tile.LocalPosition.y);
			Tile origin2 = leftMostTile;
			TopologyHelper topologyHelper4 = topologyHelper3;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache5 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache5 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.BurnEffect);
			}
			PowerResolvementHelper.DoPower(origin2, resolver, topologyHelper4, PowerCombinationLogic._003C_003Ef__mg_0024cache5, 2f, 2f);
		});
		Dictionary<PowerCombination, Action<Tile, IHitResolver>> dictionary7 = dictionary;
		PowerColor[] array6 = new PowerColor[3];
		array6[0] = PowerColor.Green;
		array6[1] = PowerColor.Blue;
		dictionary7.Add(new PowerCombination(array6), delegate(Tile tile, IHitResolver resolver)
		{
			TopologyHelper topologyHelper = new TopologyHelper();
			topologyHelper.AddRow(tile, 5, 0);
			TopologyHelper topologyHelper2 = topologyHelper;
			if (PowerCombinationLogic._003C_003Ef__mg_0024cache6 == null)
			{
				PowerCombinationLogic._003C_003Ef__mg_0024cache6 = new PowerResolvementHelper.TileAnimation(PowerResolvementHelper.NinjaStarSingleHitEffect);
			}
			PowerResolvementHelper.DoEffects(tile, resolver, topologyHelper2, PowerCombinationLogic._003C_003Ef__mg_0024cache6, 0f);
			List<Tile> sortedTiles = topologyHelper.GetSortedTiles(delegate(Tile a, Tile b)
			{
				if (a.Coord.y == b.Coord.y)
				{
					return Mathf.Abs(a.Coord.x - 5).CompareTo(Mathf.Abs(b.Coord.x - 5));
				}
				return b.Coord.y.CompareTo(a.Coord.y);
			});
			if (sortedTiles.Count > 0)
			{
				PowerResolvementHelper.DoNotesColumn(sortedTiles[0], resolver, 20, 5, false, 0.75f);
			}
			TopologyHelper topologyHelper3 = new TopologyHelper();
			topologyHelper3.AddRow(tile, 5, 0);
			foreach (List<Tile> list in topologyHelper3.GetRowTiles())
			{
				list.Sort((Tile a, Tile b) => Mathf.Abs(a.Coord.x - 5).CompareTo(Mathf.Abs(b.Coord.x - 5)));
				if (list.Count != 0)
				{
					float num = 1.75f;
					resolver.QueueEffect(PowerResolvementHelper.FrogHitEffect(list[0]), num);
					num += 1f;
					foreach (Tile at in list)
					{
						resolver.MarkHit(at, HitCause.Power, num);
						num += 0.035f;
					}
				}
			}
		});
		dictionary.Add(new PowerCombination(new PowerColor[]
		{
			PowerColor.Green,
			PowerColor.Blue,
			PowerColor.Yellow,
			PowerColor.Red
		}), delegate(Tile tile, IHitResolver resolver)
		{
			PowerResolvementHelper.DoFinalPower(tile, resolver, 11);
		});
		PowerCombinationLogic.combinationLogic = dictionary;
	}

	private static Dictionary<PowerCombination, Action<Tile, IHitResolver>> combinationLogic;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache5;

	[CompilerGenerated]
	private static PowerResolvementHelper.TileAnimation _003C_003Ef__mg_0024cache6;
}
