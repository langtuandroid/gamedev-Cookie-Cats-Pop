using System;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class FontStyleStateHandler : MonoBehaviour
{
	private void Awake()
	{
		this.label = base.GetComponent<UILabel>();
		this.SetState(this.isInitiallyActive);
	}

	public void SetState(bool isActive)
	{
		this.label.fontStyle = ((!isActive) ? this.inactiveState : this.activeState);
	}

	[SerializeField]
	private UIFontStyle activeState;

	[SerializeField]
	private UIFontStyle inactiveState;

	[SerializeField]
	private bool isInitiallyActive;

	private UILabel label;
}
