using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.SideMapButtons;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class SideMapButton : MonoBehaviour, ISideMapButton
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action Clicked;



	protected void Start()
	{
		UIButton componentInChildren = base.GetComponentInChildren<UIButton>();
		componentInChildren.Clicked += delegate(UIButton button)
		{
			this.Clicked();
		};
		this.Setup();
		this.fiber.Start(this.Looper());
	}

	protected void OnDestroy()
	{
		this.Destroy();
		this.fiber.Terminate();
	}

	private IEnumerator Looper()
	{
		for (;;)
		{
			this.UpdateOncePerSecond();
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	protected virtual void Setup()
	{
	}

	protected virtual void Destroy()
	{
	}

	protected virtual void UpdateOncePerSecond()
	{
	}

	public virtual SideMapButton.AreaSide Side
	{
		get
		{
			return this.side;
		}
	}

	public virtual bool VisibilityChecker(object data)
	{
		return true;
	}

	public virtual Vector2 Size
	{
		get
		{
			return Vector2.zero;
		}
	}

	public virtual object Data
	{
		get
		{
			return null;
		}
	}

	public virtual bool ShouldDestroy
	{
		get
		{
			return false;
		}
	}

	public virtual void OnButtonShown()
	{
	}

	public virtual void OnButtonHidden()
	{
	}

	[SerializeField]
	private SideMapButton.AreaSide side = SideMapButton.AreaSide.Left;

	private readonly Fiber fiber = new Fiber();

	public enum AreaSide
	{
		Left = -1,
		Right = 1
	}
}
