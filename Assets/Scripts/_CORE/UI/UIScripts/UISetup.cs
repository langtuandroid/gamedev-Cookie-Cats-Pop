using System;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/UISetup.asset")]
public class UISetup : SingletonAsset<UISetup>
{
	public SoundDefinition soundButtonConfirm;

	public SoundDefinition soundButtonNormal;

	public SoundDefinition soundButtonDismiss;

	public SoundDefinition purchaseSuccessful;

	public SoundDefinition itemPutIntoInventory;

	public AnimationCurve buttonWobblyScaleIn;

	public AnimationCurve buttonWobblyPress;
}
