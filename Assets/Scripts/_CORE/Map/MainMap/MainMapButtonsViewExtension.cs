using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.LevelDash.Views;
using TactileModules.PuzzleGames.LevelDash.Views.Handlers;
using TactileModules.SagaCore;
using UnityEngine;

public class MainMapButtonsViewExtension : MonoBehaviour
{
	
	private ButtonWithBadge achievementsButtonRef
	{
		get
		{
			return this.achievementsButton.GetInstance<ButtonWithBadge>();
		}
	}
	

	private MainMapButtonsView mainMapButtonsView
	{
		get
		{
			return base.GetComponent<MainMapButtonsView>();
		}
	}

	private AchievementsManager AchievementsManager
	{
		get
		{
			return ManagerRepository.Get<AchievementsManager>();
		}
	}

	public SideButtonsArea SideButtonsArea
	{
		get
		{
			return this.buttonsArea;
		}
	}

	private void Start()
	{
		this.updateButtonsFiber.Start(this.UpdateButtons());
	}

	private void OnEnable()
	{
		this.buttonsArea.Init();
	}

	private void OnDisable()
	{
		this.updateButtonsFiber.Terminate();
		this.showViewFiber.Terminate();
		if (this.levelDashMapButtonHandler != null)
		{
			this.levelDashMapButtonHandler.Dispose();
		}
	}

	protected T ObtainOverlay<T>() where T : UIView
	{
		return UIViewManager.Instance.ObtainOverlay<T>(this.mainMapButtonsView);
	}

	[UsedImplicitly]
	protected void ViewDidDisappear()
	{
		foreach (Transform transform in this.instantiatedSideButtons)
		{
			this.buttonsArea.Destroy(transform.gameObject, false);
		}
	}

	[UsedImplicitly]
	protected void ViewWillAppear()
	{
		this.currencyOverlay = this.ObtainOverlay<CurrencyOverlay>();
		this.livesOverlay = this.ObtainOverlay<LivesOverlay>();
		this.livesOverlay.lifeBar.LivesType = "Life";
		this.InitializeButtonStates();
		foreach (SideMapButton sideMapButton in this.sideButtons)
		{
			Transform transform = UnityEngine.Object.Instantiate<GameObject>(sideMapButton.gameObject).transform;
			this.instantiatedSideButtons.Add(transform);
			SideMapButton component = transform.gameObject.GetComponent<SideMapButton>();
			this.buttonsArea.InitButton(transform, (int)component.Side, component.GetElementSize(), new Func<object, bool>(component.VisibilityChecker));
		}
		this.levelDashMapButtonHandler = new LevelDashMapButtonHandler(this.buttonsArea, this.levelDashMapButtonPrefab);
	}

	private Vector2 GetButtonSize(GameObject button)
	{
		if (!this.buttonSizes.ContainsKey(button))
		{
			this.buttonSizes.Add(button, button.GetComponent<UIElement>().Size);
		}
		return this.buttonSizes[button];
	}

	[UsedImplicitly]
	protected void ViewGotFocus()
	{
		if (this.currencyOverlay == null)
		{
			this.currencyOverlay = this.ObtainOverlay<CurrencyOverlay>();
		}
		if (this.currencyOverlay != null)
		{
			this.currencyOverlay.OnCoinButtonClicked = new Action(this.ClickedCurrencyOverlay);
			if (this.currencyOverlay.transform != null)
			{
				this.currencyOverlay.transform.localPosition = Vector3.back * 100f;
			}
		}
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = new Action(this.ClickedLivesOverlay);
			if (this.livesOverlay.transform != null)
			{
				this.livesOverlay.transform.localPosition = Vector3.back * 100f;
			}
		}
		this.UpdateUI();
	}

	[UsedImplicitly]
	protected void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
		this.livesOverlay.OnLifeButtonClicked = null;
	}

	private void InitializeButtonStates()
	{
	}

	public T FindSideButton<T>() where T : SideMapButton
	{
		foreach (Transform transform in this.instantiatedSideButtons)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
		}
		return (T)((object)null);
	}

	private void ClickedCurrencyOverlay()
	{
		UIViewManager.Instance.ShowView<ShopView>(new object[]
		{
			0
		});
	}

	private void ClickedLivesOverlay()
	{
		if (InventoryManager.Instance.Lives > 0)
		{
			UIViewManager.Instance.ShowView<GetMoreLivesView>(new object[0]);
		}
		else
		{
			UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[0]);
		}
	}

	[UsedImplicitly]
	private void ClickedSettings(UIEvent e)
	{
		UIViewManager.Instance.ShowView<SettingsView>(new object[0]);
	}
	

	[UsedImplicitly]
	private void ClickedAchievements(UIEvent e)
	{
		UIViewManager.Instance.ShowView<AchievementsView>(new object[0]);
	}

	[UsedImplicitly]
	private void ClickedHub(UIEvent e)
	{
	}

	public void UpdateUI()
	{
	}

	private IEnumerator UpdateButtons()
	{
		for (;;)
		{
			if (this.achievementsButtonRef != null)
			{
				int totalUnclaimedAchievements = this.AchievementsManager.GetTotalUnclaimedAchievements();
				if (totalUnclaimedAchievements > 0)
				{
					this.achievementsButtonRef.BadgeText = totalUnclaimedAchievements.ToString();
				}
				else
				{
					this.achievementsButtonRef.BadgeText = string.Empty;
				}
			}
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	private void CrossPromotionClicked(UIEvent e)
	{
		this.showViewFiber.Start(this.ShowCrossPromotionView());
	}

	private void UpdateCrossPromotionButton(GameObject button)
	{
		if (this.crossPromotionButton == null)
		{
			this.CreateCrossPromotionButton(button);
		}
		else
		{
			this.buttonsArea.Destroy(this.crossPromotionButton, true, delegate()
			{
				this.crossPromotionButton = null;
				this.CreateCrossPromotionButton(button);
			});
		}
	}

	private void CreateCrossPromotionButton(GameObject prototype)
	{
		if (prototype != null)
		{
			this.crossPromotionButton = UnityEngine.Object.Instantiate<GameObject>(prototype);
			this.crossPromotionButton.name = prototype.name;
			UIButton componentInChildren = this.crossPromotionButton.GetComponentInChildren<UIButton>();
			componentInChildren.receiver = base.gameObject;
			componentInChildren.methodName = "CrossPromotionClicked";
			this.crossPromotionButton.transform.SetParent(this.buttonsArea.transform, false);
			this.crossPromotionButton.SetLayerRecursively(base.gameObject.layer);
		}
	}

	private IEnumerator ShowCrossPromotionView()
	{
		yield return null;
		yield break;
	}

	[Header("Bottom Buttons")]
	
	public UIInstantiator achievementsButton;
	
	private readonly Dictionary<GameObject, Vector2> buttonSizes = new Dictionary<GameObject, Vector2>();

	[SerializeField]
	private LevelDashMapButton levelDashMapButtonPrefab;

	private CurrencyOverlay currencyOverlay;

	private LivesOverlay livesOverlay;

	[Header("Right Side Buttons")]
	private GameObject crossPromotionButton;

	private Fiber updateButtonsFiber = new Fiber();

	private readonly Fiber showViewFiber = new Fiber();

	private readonly Dictionary<string, GameObject> offerButtons = new Dictionary<string, GameObject>();

	[SerializeField]
	private SideButtonsArea buttonsArea;

	[SerializeField]
	private List<SideMapButton> sideButtons;

	private LevelDashMapButtonHandler levelDashMapButtonHandler;

	private List<Transform> instantiatedSideButtons = new List<Transform>();
}
