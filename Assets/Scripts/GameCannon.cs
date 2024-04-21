using System;
using System.Diagnostics;
using UnityEngine;

public class GameCannon : MonoBehaviour
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<AimingState> AimingStarted = delegate (AimingState A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<AimingState> AimingEnded = delegate (AimingState A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<AimingState> Shoot = delegate (AimingState A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action Swapped = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action SuperAimActivated = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action SuperAimDeactivated = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<GameCannon.SuperAimModification> SuperAimModified = delegate (GameCannon.SuperAimModification A_0)
    {
    };



    public bool HasSuperAim { get; private set; }

	public AimingState CurrentAimingState
	{
		get
		{
			return this.aimingState;
		}
	}

	public void Initialize(GameBoard board, Func<AimingState, bool> aimValidFunction)
	{
		this.board = board;
		this.storedTouchPositions = new BetterList<Vector2>();
		this.aimValidFunction = aimValidFunction;
	}

	private void OnPress(bool pressed)
	{
		this.isPressed = pressed;
		if (!pressed)
		{
			if (this.aimIsActive && !this.isTouchInsideSwapArea && this.isAimValid)
			{
				this.Shoot(this.aimingState);
			}
			else if (this.isTouchInsideSwapArea && !this.hasAimed)
			{
				this.Swapped();
			}
			this.hasAimed = false;
		}
		else
		{
			this.storedTouchPositions.Clear();
			this.Update();
		}
		this.isAiming = false;
	}

	public void StartAiming()
	{
		this.isAiming = true;
	}

	public void EndAiming()
	{
		this.isAiming = false;
	}

	private void Update()
	{
		if (this.board == null)
		{
			return;
		}
		if (UICamera.CurrentCamera != null)
		{
			this.isTouchInsideSwapArea = this.IsTouchPositionInsideSwapArea();
			if (this.isPressed)
			{
				this.CalculateAimFromTouchPosition();
			}
		}
		else
		{
			this.isTouchInsideSwapArea = false;
		}
		this.isAimValid = this.aimValidFunction(this.aimingState);
		this.aimIsActive = (this.InputEnabled && this.isAimValid && (this.isAiming || (this.isPressed && !this.isTouchInsideSwapArea)) && this.IsTouchPositionInsideBounds());
		if (this.aimIsActive && !this.aimWasActive)
		{
			if (this.AimingStarted != null)
			{
				this.AimingStarted(this.aimingState);
			}
			this.hasAimed = true;
		}
		else if (!this.aimIsActive && this.aimWasActive && this.AimingEnded != null)
		{
			this.AimingEnded(this.aimingState);
		}
		this.aimWasActive = this.aimIsActive;
	}

	private bool IsTouchPositionInsideBounds()
	{
		Vector3 point = UICamera.CurrentCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		return this.GetElement().GetRectInWorldPos().Contains(point);
	}

	private bool IsTouchPositionInsideSwapArea()
	{
		Vector3 point = UICamera.CurrentCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		return this.swapArea.GetRectInWorldPos().Contains(point);
	}

	private void CalculateAimFromTouchPosition()
	{
		Vector3 v = UICamera.CurrentCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
		Vector2 vector = Vector2.zero;
		if (Application.isMobilePlatform)
		{
			this.storedTouchPositions.Add(v);
			while (this.storedTouchPositions.size > 10)
			{
				this.storedTouchPositions.RemoveAt(0);
			}
			int num = Mathf.Min(5, this.storedTouchPositions.size);
			for (int i = 0; i < num; i++)
			{
				vector += this.storedTouchPositions[i];
			}
			vector.x /= (float)num;
			vector.y /= (float)num;
		}
		else
		{
			vector = v;
		}
		this.CalculateAim(vector);
	}

	public void ActivateSuperAim()
	{
		this.HasSuperAim = true;
		this.SuperAimActivated();
	}

	public void DeactivateSuperAim()
	{
		this.HasSuperAim = false;
		this.SuperAimDeactivated();
	}

	public void ModifySuperAim(GameCannon.SuperAimModification modification)
	{
		this.SuperAimModified(modification);
	}

	public void CalculateAim(Vector2 target)
	{
		Vector3 position = this.muzzlePivot.position;
		for (int i = 0; i < this.aimingState.Aims.Count; i++)
		{
			this.aimingState.Aims[i].Calculate(position, target, this.board);
		}
	}

	public void ClearAims()
	{
		this.aimingState.Clear();
	}

	public void AddAim(float angleOffset)
	{
		this.aimingState.Aims.Add(new AimInfo(angleOffset));
	}

	[SerializeField]
	public Transform muzzlePivot;

	public NestedEnabler InputEnabled;

	[SerializeField]
	private UIElement swapArea;

	private readonly AimingState aimingState = new AimingState();

	private GameBoard board;

	private bool isAiming;

	private bool isPressed;

	private BetterList<Vector2> storedTouchPositions;

	private const int maxStoredTouchPositions = 10;

	private Func<AimingState, bool> aimValidFunction;

	private bool aimIsActive;

	private bool aimWasActive;

	private bool isTouchInsideSwapArea;

	private bool hasAimed;

	private bool isAimValid;

	public enum SuperAimModification
	{
		Blink
	}
}
