using System;
using TactileModules.Validation;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
	public UISprite icon;

	[OptionalSerializedField]
	public UILabel label;
}
