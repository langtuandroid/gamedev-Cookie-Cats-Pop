using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile;
using UnityEngine;

public class BuySpecialBooster : MonoBehaviour
{
	public InventoryItem BoosterType
	{
		get
		{
			return this.boosterType;
		}
		set
		{
			if (this.boosterType == value)
			{
				return;
			}
			this.boosterType = value;
			this.ChangeIcon();
		}
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<InventoryItem> BoosterActivated = delegate (InventoryItem A_0)
    {
    };




    public bool IsEnabled
	{
		get
		{
			return this.BoosterType != null;
		}
	}

	public void Initialize(LevelSession levelSession)
	{
		this.levelSession = levelSession;
		levelSession.BallQueue.BallsLeftChanged += this.UpdateState;
		levelSession.StateChanged += this.LevelSession_StateChanged;
		this.animatedPivot.gameObject.SetActive(false);
		this.animatedPivot.localPosition = this.disabledLocation.localPosition;
		this.UpdateState();
		levelSession.ShieldDeactivated += delegate()
		{
			this.inGameBoosters.Add("BoosterShield");
			this.UpdateState();
		};
		levelSession.ShieldActivated += delegate()
		{
			this.inGameBoosters.Remove("BoosterShield");
			this.UpdateState();
		};
		levelSession.Cannon.SuperAimDeactivated += delegate()
		{
			this.inGameBoosters.Add("BoosterSuperAim");
			this.UpdateState();
		};
		levelSession.Cannon.SuperAimActivated += delegate()
		{
			this.inGameBoosters.Remove("BoosterSuperAim");
			this.UpdateState();
		};
		levelSession.BallQueue.TripleQueueDeactivated += delegate()
		{
			this.inGameBoosters.Add("BoosterSuperQueue");
			this.UpdateState();
		};
		levelSession.BallQueue.TripleQueueActivated += delegate()
		{
			this.inGameBoosters.Remove("BoosterSuperQueue");
			this.UpdateState();
		};
	}

	private void ChangeIcon()
	{
		this.animationFiber.Start(this.ChangeIconCr());
	}

	private IEnumerator ChangeIconCr()
	{
		yield return this.FadeOut();
		if (this.BoosterType != null)
		{
			BoosterMetaData info = InventoryManager.Instance.GetMetaData<BoosterMetaData>(this.BoosterType);
			this.spriteIcon.SpriteName = info.IconSpriteName;
			yield return this.FadeIn();
		}
		yield break;
	}

	private void LevelSession_StateChanged(LevelSession obj)
	{
		this.UpdateState();
	}

	private void UpdateState()
	{
		if (this.levelSession.SessionState == LevelSessionState.Playing)
		{
			if (this.levelSession.BallQueue.BallsLeft < 10)
			{
				this.BoosterType = "BoosterExtraMoves";
			}
			else
			{
				this.BoosterType = this.GetInGameBooster();
			}
		}
	}

	private InventoryItem GetInGameBooster()
	{
		if (this.inGameBoosters.Count == 0)
		{
			return null;
		}
		return this.inGameBoosters.GetRandom<InventoryItem>();
	}

	private void Update()
	{
		if (this.animationFiber != null)
		{
			this.animationFiber.Step();
		}
	}

	private IEnumerator FadeIn()
	{
		this.animatedPivot.gameObject.SetActive(true);
		yield return FiberAnimation.MoveLocalTransform(this.animatedPivot, this.animatedPivot.localPosition, this.enabledLocation.localPosition, this.animateCurve, 0f);
		yield break;
	}

	private IEnumerator FadeOut()
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.animatedPivot.gameObject.SetActive(false);
		});
		yield return FiberAnimation.MoveLocalTransform(this.animatedPivot, this.animatedPivot.localPosition, this.disabledLocation.localPosition, this.animateCurve, 0f);
		yield break;
	}

	private void BuyClicked(UIEvent e)
	{
		if (!this.IsEnabled)
		{
			return;
		}
		if (!this.fiber.IsTerminated)
		{
			return;
		}
		if (this.levelSession.SessionState != LevelSessionState.Playing)
		{
			return;
		}
		if (this.levelSession.IsTurnResolving)
		{
			return;
		}
		if (this.levelSession.IsDeathTriggered)
		{
			return;
		}
		if (this.levelSession.BallQueue.BallsLeft <= 0)
		{
			return;
		}
		this.fiber.Start(BoosterBar.BuyBooster(this.BoosterType, delegate
		{
			this.BoosterActivated(this.BoosterType);
		}));
	}

	public Transform enabledLocation;

	public Transform disabledLocation;

	public Transform animatedPivot;

	public AnimationCurve animateCurve;

	private InventoryItem boosterType = null;

	private List<InventoryItem> inGameBoosters = new List<InventoryItem>();

	[SerializeField]
	private UISprite spriteIcon;

	private Fiber fiber = new Fiber();

	private LevelSession levelSession;

	private Fiber animationFiber = new Fiber(FiberBucket.Manual);
}
