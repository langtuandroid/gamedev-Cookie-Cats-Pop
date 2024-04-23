using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile;
using UnityEngine;

public class BoosterBar : MonoBehaviour, IRewardTarget
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<InventoryItem> BoosterActivated = delegate (InventoryItem A_0)
    {
    };



    public void Initialize(IEnumerable<InventoryItem> boostersAvailable, LevelSession session)
	{
		this.session = session;
		this.BuildButtons(boostersAvailable);
		this.layoutArea.Layout();
	}

	private void BuildButtons(IEnumerable<InventoryItem> boosterIds)
	{
		this.buttons.Clear();
		IEnumerator enumerator = this.layoutArea.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		foreach (InventoryItem inventoryItem in boosterIds)
		{
			BoosterButton boosterButton = UnityEngine.Object.Instantiate<BoosterButton>(this.buttonPrefab);
			boosterButton.gameObject.SetLayerRecursively(base.gameObject.layer);
			boosterButton.transform.parent = this.layoutArea.transform;
			boosterButton.transform.localPosition = Vector3.zero;
			boosterButton.BoosterId = inventoryItem;
			boosterButton.GetButton().receiver = base.gameObject;
			boosterButton.GetButton().payload = inventoryItem;
			boosterButton.GetButton().methodName = "ButtonClicked";
			this.buttons.Add(boosterButton);
		}
		if (this.buttons.Count > 0)
		{
			this.plank.Size = new Vector2((float)(this.buttons.Count * 100 + 50), this.plank.Size.y);
			this.plank.gameObject.SetActive(true);
		}
		else
		{
			this.plank.gameObject.SetActive(false);
		}
	}

	public void DisableAllButtons()
	{
		this.DisableAllButtons(null);
	}

	public void DisableAllButtons(InventoryItem exceptThis)
	{
		foreach (BoosterButton boosterButton in this.buttons)
		{
			boosterButton.Disabled = (exceptThis != boosterButton.BoosterId);
			boosterButton.Free = (exceptThis == boosterButton.BoosterId);
		}
	}

	public void EnableAllButtons()
	{
		foreach (BoosterButton boosterButton in this.buttons)
		{
			boosterButton.Disabled = this.disabledButtons.Contains(boosterButton);
			boosterButton.Free = false;
		}
	}

	public void DisableButtonWithBoosterId(string boosterId)
	{
		BoosterButton boosterButton = this.FindButton(boosterId);
		if (boosterButton != null)
		{
			this.disabledButtons.Add(boosterButton);
			boosterButton.Disabled = true;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void ButtonClicked(UIEvent e)
	{
        UnityEngine.Debug.Log("ButtonClicked " + e);
        if (this.ButtonsEnabled == null || !this.ButtonsEnabled())
		{
			return;
		}
		BoosterButton btn = e.sender.GetComponent<BoosterButton>();
        
        if (!btn.Disabled)
		{
            
            InventoryItem boosterId = btn.BoosterId;
			if (this.session != null && this.session.SpecialPieceToShoot != PieceId.Empty)
			{
				CPBoosterMetaData metaData = InventoryManager.Instance.GetMetaData<CPBoosterMetaData>(boosterId);
				if (metaData.IsThrownAsBall)
				{
					return;
				}
			}
			int numberOfBoosters = BoosterManagerBase<BoosterManager>.Instance.GetNumberOfBoosters(boosterId);
			if (numberOfBoosters > 0 || btn.Free)
			{
				this.fiber.Start(BoosterBar.UseBoosterFlow(boosterId, !btn.Free, delegate
				{
					if (btn.Free)
					{
						btn.Free = false;
					}
					this.BoosterActivated(boosterId);
				}));
			}
			else
			{
				this.fiber.Start(BoosterBar.BuyBooster(btn.BoosterId, delegate
				{
					this.BoosterActivated(boosterId);
				}));
			}
		}
	}

	public List<BoosterButton> GetBoosterButtons()
	{
		return new List<BoosterButton>(this.layoutArea.GetComponentsInChildren<BoosterButton>());
	}

	public BoosterButton FindButton(InventoryItem boosterId)
	{
		foreach (BoosterButton boosterButton in this.GetBoosterButtons())
		{
			if (boosterButton.BoosterId == boosterId)
			{
				return boosterButton;
			}
		}
		return null;
	}

	public static IEnumerator UseBoosterFlow(InventoryItem boosterId, bool askForConfirmation, Action onBoosterUsed)
	{
		GameEventManager.Instance.Emit(3, boosterId, 1);
		if (askForConfirmation)
		{
			UIViewManager.UIViewStateGeneric<BoosterConfirmUseView> vs = UIViewManager.Instance.ShowView<BoosterConfirmUseView>(new object[]
			{
				boosterId
			});
			yield return vs.WaitForClose();
			if ((int)vs.ClosingResult != 0)
			{
				yield break;
			}
			InventoryManager.Instance.Consume(boosterId, 1, "BoosterBar");
		}
		GameEventManager.Instance.Emit(4, boosterId, 1);
		if (onBoosterUsed != null)
		{
			onBoosterUsed();
		}
		yield break;
	}

	public static IEnumerator BuyBooster(InventoryItem boosterId, Action onBoosterBoughtAndUsed)
	{
		BoosterMetaData boosterInfo = InventoryManager.Instance.GetMetaData<BoosterMetaData>(boosterId);
		ShopItem item = ShopManager.Instance.GetShopItem(boosterInfo.InGameShopItemIdentifier);
		UIViewManager.UIViewStateGeneric<BuyShopItemView> vs = UIViewManager.Instance.ShowView<BuyShopItemView>(new object[]
		{
			item,
			false
		});
		yield return vs.WaitForClose();
		if ((BuyShopItemView.Result)vs.ClosingResult == BuyShopItemView.Result.SuccessfullyBought)
		{
			InventoryManager.Instance.Consume(boosterId, 1, "BoosterBar");
			yield return BoosterBar.UseBoosterFlow(boosterId, false, onBoosterBoughtAndUsed);
		}
		yield break;
	}

	Transform IRewardTarget.TryGetTransformTarget(InventoryItem item)
	{
		BoosterButton boosterButton = this.FindButton(item);
		if (boosterButton != null)
		{
			return boosterButton.transform;
		}
		return null;
	}

	UILabel IRewardTarget.TryGetAmountLabel(InventoryItem item)
	{
		BoosterButton boosterButton = this.FindButton(item);
		return (!(boosterButton != null)) ? null : boosterButton.amountLabel;
	}

	void IRewardTarget.Initialize()
	{
		this.Initialize(BoosterManagerBase<BoosterManager>.Instance.GetUnlockedBoosters(), null);
	}

	void IRewardTarget.DisableInventoryListeners()
	{
		for (int i = 0; i < this.buttons.Count; i++)
		{
			this.buttons[i].EnableForcedAmounts();
		}
	}

	void IRewardTarget.ReEnableInventoryListeners()
	{
		for (int i = 0; i < this.buttons.Count; i++)
		{
			this.buttons[i].DisableForcedAmounts();
		}
	}

	private LevelSession session;

	public UIWidget plank;

	public UILayout layoutArea;

	public BoosterButton buttonPrefab;

	public Func<bool> ButtonsEnabled ;

	private Fiber fiber = new Fiber();

	private List<BoosterButton> buttons = new List<BoosterButton>();

	private HashSet<BoosterButton> disabledButtons = new HashSet<BoosterButton>();

	private const string INVENTORY_CONTEXT = "BoosterBar";
}
