using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class BoosterPieceCollectModule : LogicModule
{
	public override void Begin(LevelSession session)
	{
		this.levelSession = session;
		this.levelSession.TurnLogic.PieceCleared += this.HandlePieceCleared;
		this.levelSession.ShieldActivated += this.RemoveBoosterShield;
		this.levelSession.Cannon.SuperAimActivated += this.RemoveSuperAim;
		this.levelSession.BallQueue.TripleQueueActivated += this.RemoveTripleQueue;
		this.giveOneTurnBoosters = new List<InventoryItem>();
		this.activeOneTurnBoosters = new List<InventoryItem>();
		this.activeOneTurnBoostersTurns = new Dictionary<InventoryItem, int>
		{
			{
				"BoosterShield",
				0
			},
			{
				"BoosterSuperAim",
				0
			},
			{
				"BoosterSuperQueue",
				0
			}
		};
	}

	public override void End(LevelSession session)
	{
		this.levelSession.TurnLogic.PieceCleared -= this.HandlePieceCleared;
		this.levelSession.ShieldActivated -= this.RemoveBoosterShield;
		this.levelSession.Cannon.SuperAimActivated -= this.RemoveSuperAim;
		this.levelSession.BallQueue.TripleQueueActivated -= this.RemoveTripleQueue;
	}

	private void RemoveBoosterShield()
	{
		this.RemoveOneTurnBooster("BoosterShield");
	}

	private void RemoveSuperAim()
	{
		this.RemoveOneTurnBooster("BoosterSuperAim");
	}

	private void RemoveTripleQueue()
	{
		this.RemoveOneTurnBooster("BoosterSuperQueue");
	}

	private void RemoveOneTurnBooster(InventoryItem item)
	{
		if (!this.giveOneTurnBoosters.Contains(item))
		{
			this.activeOneTurnBoosters.Remove(item);
		}
	}

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		if (piece is BoosterPiece && hit.cause != HitCause.Unknown)
		{
			string val = null;
			if (piece is ShieldBoosterPiece)
			{
				val = "BoosterShield";
			}
			if (piece is SuperaimBoosterPiece)
			{
				val = "BoosterSuperAim";
			}
			if (piece is SuperqueueBoosterPiece)
			{
				val = "BoosterSuperQueue";
			}
			this.TryGiveOneTurnBooster(val);
		}
	}

	private void TryGiveOneTurnBooster(InventoryItem boosterType)
	{
		if (this.IsAPermanentBooster(boosterType))
		{
			return;
		}
		if (this.giveOneTurnBoosters.Contains(boosterType))
		{
			return;
		}
		this.giveOneTurnBoosters.Add(boosterType);
		this.activeOneTurnBoostersTurns[boosterType] = 0;
	}

	private bool IsAPermanentBooster(InventoryItem boosterType)
	{
		if (boosterType == "BoosterShield")
		{
			return this.levelSession.ShieldActive && !this.activeOneTurnBoosters.Contains(boosterType);
		}
		if (boosterType == "BoosterSuperAim")
		{
			return this.levelSession.Cannon.HasSuperAim && !this.activeOneTurnBoosters.Contains(boosterType);
		}
		return boosterType == "BoosterSuperQueue" && this.levelSession.BallQueue.HasTripleQueue && !this.activeOneTurnBoosters.Contains(boosterType);
	}

	public override void TurnCompleted(LevelSession session)
	{
		this.fiber.Start(this.TurnCompletedCr());
	}

	private IEnumerator TurnCompletedCr()
	{
		foreach (InventoryItem inventoryItem in this.activeOneTurnBoosters)
		{
			Dictionary<InventoryItem, int> dictionary;
			InventoryItem key;
			(dictionary = this.activeOneTurnBoostersTurns)[key = inventoryItem] = dictionary[key] + 1;
		}
		if (this.activeOneTurnBoosters.Contains("BoosterShield") && this.activeOneTurnBoostersTurns["BoosterShield"] >= 1)
		{
			this.levelSession.DeactivateShield();
			this.activeOneTurnBoosters.Remove("BoosterShield");
		}
		if (this.activeOneTurnBoosters.Contains("BoosterSuperAim") && this.activeOneTurnBoostersTurns["BoosterSuperAim"] >= 1)
		{
			this.levelSession.Cannon.DeactivateSuperAim();
			this.activeOneTurnBoosters.Remove("BoosterSuperAim");
		}
		if (this.activeOneTurnBoosters.Contains("BoosterSuperQueue") && this.activeOneTurnBoostersTurns["BoosterSuperQueue"] >= 1)
		{
			this.levelSession.BallQueue.DeactivateTripleQueue();
			this.activeOneTurnBoosters.Remove("BoosterSuperQueue");
		}
		foreach (InventoryItem oneTurnBooster in this.giveOneTurnBoosters)
		{
			this.activeOneTurnBoosters.Add(oneTurnBooster);
			EffectPool.Instance.SpawnEffect("OneTurnBoosterStampEffect", Vector3.zero, UIViewManager.Instance.FindView<GameView>().gameObject.layer, new object[0]);
			yield return BoosterLogic.ResolveBooster(oneTurnBooster, this.levelSession, UIViewManager.Instance.FindView<GameView>());
		}
		if (this.giveOneTurnBoosters.Count > 0)
		{
			foreach (InventoryItem a in this.giveOneTurnBoosters)
			{
				if (a == "BoosterShield")
				{
					this.levelSession.ModifyShield(LevelSession.ShieldModification.Blink);
				}
				if (a == "BoosterSuperAim")
				{
					this.levelSession.Cannon.ModifySuperAim(GameCannon.SuperAimModification.Blink);
				}
				if (a == "BoosterSuperQueue")
				{
					this.levelSession.BallQueue.ModifyTripleQueue(GameBallQueue.TripleQueueModification.Blink);
				}
			}
		}
		this.giveOneTurnBoosters.Clear();
		yield break;
	}

	private readonly Fiber fiber = new Fiber();

	private LevelSession levelSession;

	private List<InventoryItem> giveOneTurnBoosters;

	private List<InventoryItem> activeOneTurnBoosters;

	private Dictionary<InventoryItem, int> activeOneTurnBoostersTurns;
}
