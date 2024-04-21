using System;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class SpriteStateHandler : MonoBehaviour
{
	private void Awake()
	{
		this.sprite = base.GetComponentInChildren<UISprite>();
		this.SetState(this.isInitiallyActive);
	}

	public void SetState(bool isActive)
	{
		this.sprite.SpriteName = ((!isActive) ? this.inactiveState : this.activeState);
	}

	[SerializeField]
	private string activeState;

	[SerializeField]
	private string inactiveState;

	[SerializeField]
	private bool isInitiallyActive;

	private UISprite sprite;
}
