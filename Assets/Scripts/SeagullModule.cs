using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using UnityEngine;

public class SeagullModule : LogicModule
{
    public override void Begin(LevelSession session)
    {
        this.session = session;
        session.StateChanged += this.Session_StateChanged;
        this.isTutorialLevel = (session.Tutorial != null && session.Tutorial.CurrentStep != null);
        if (Singleton<SeagullManager>.Instance.IsSeagullActive && !this.isTutorialLevel && Singleton<SeagullManager>.Instance.IsSeagullReady)
        {
            this.seagullFiber.Start(this.SeagullLogic());
        }
    }

    private bool ShouldStartSeagullYet()
    {
        return !this.session.IsTurnResolving && this.session.SessionState == LevelSessionState.Playing && this.session.FirstShots != null && this.session.FirstShots.Count == 0;
    }

    public override void End(LevelSession session)
    {
        session.StateChanged -= this.Session_StateChanged;
    }

    private void Session_StateChanged(LevelSession session)
    {
        if (this.seagullFiber != null)
        {
            this.seagullFiber.Terminate();
        }
        if (session.SessionState == LevelSessionState.Completed || session.SessionState == LevelSessionState.Failed || session.SessionState == LevelSessionState.Abandoned)
        {
            if (session.SessionState == LevelSessionState.Completed || session.SessionState == LevelSessionState.Failed)
            {
                Singleton<SeagullManager>.Instance.MarkCount();
            }
            session.StateChanged -= this.Session_StateChanged;
        }
    }

    private IEnumerator SeagullLogic()
    {
        while (!this.ShouldStartSeagullYet())
        {
            yield return new WaitForSeconds(1f);
        }
        float randomTime = UnityEngine.Random.Range(15f, 60f);
        yield return FiberHelper.Wait(randomTime, (FiberHelper.WaitFlag)0);
        while (!this.ShouldStartSeagullYet())
        {
            yield return new WaitForSeconds(1f);
        }
        while (UICamera.InputDisabled)
        {
            yield return null;
        }
        Singleton<SeagullManager>.Instance.ResetCount();
        GameView gameView = UIViewManager.Instance.FindView<GameView>();
        SeagullEffect effect = EffectPool.Instance.SpawnEffect("SeagullEffect", Vector3.zero, gameView.gameObject.layer, new object[]
        {
            new SeagullEffect.SetupParameters
            {
                distanceAway = 0f,
                height = 1f,
                flyDuration = 6f,
                flyLeft = (UnityEngine.Random.value < 0.5f)
            }
        }) as SeagullEffect;
        List<ItemAmount> rewards = Singleton<SeagullManager>.Instance.GetRewards();
        effect.rewardGrid.Clear();
        effect.rewardGrid.Initialize(rewards, true);
        effect.rewardGrid.gameObject.SetActive(false);
        bool seagullEffectEnded = false;
        effect.onFinished = delegate ()
        {
            seagullEffectEnded = true;
        };
        List<CPPiece> flyingPieces = new List<CPPiece>();
        Action<bool> isDestroyedByIntercept = null;
        Action<CPPiece, Action<bool>> shotStarted = delegate (CPPiece p, Action<bool> a)
        {
            flyingPieces.Add(p);
            isDestroyedByIntercept = a;
        };
        Action<CPPiece> shotEnded = delegate (CPPiece p)
        {
            flyingPieces.Remove(p);
        };
        yield return new Fiber.OnExit(delegate ()
        {
            effect.Terminate();
            this.session.TurnLogic.ShotMovementStarted -= shotStarted;
            this.session.TurnLogic.ShotMovementEnded -= shotEnded;
        });
        this.session.TurnLogic.ShotMovementStarted += shotStarted;
        this.session.TurnLogic.ShotMovementEnded += shotEnded;
        bool hit = false;
        while (!seagullEffectEnded)
        {
            Vector2 seagullPosition = effect.seagullTransform.position;
            foreach (CPPiece cppiece in flyingPieces)
            {
                if ((cppiece.transform.position - (Vector3)seagullPosition).magnitude < 100f)
                {
                    hit = true;
                    break;
                }
            }
            if (hit)
            {
                if (isDestroyedByIntercept != null)
                {
                    isDestroyedByIntercept(true);
                }
                break;
            }
            yield return null;
        }
        this.session.TurnLogic.ShotMovementStarted -= shotStarted;
        this.session.TurnLogic.ShotMovementEnded -= shotEnded;
        if (hit)
        {
            if (SingletonAsset<SoundDatabase>.Instance.seagullHit != null)
            {
                SingletonAsset<SoundDatabase>.Instance.seagullHit.PlaySequential();
            }
            effect.Hit();
            foreach (ItemAmount itemAmount in rewards)
            {
                InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "Seagull");
            }
            effect.rewardGrid.gameObject.SetActive(true);
            yield return effect.rewardGrid.Animate(false, false);
        }
        while (!seagullEffectEnded)
        {
            yield return null;
        }
        yield break;
    }

    private readonly Fiber seagullFiber = new Fiber();

    private LevelSession session;

    private bool isTutorialLevel;
}
