using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAspectLayout : UILayout
{
	public override void Layout()
	{
		List<UIElement> childElements = this.GetChildElements((UILayout.QueryFlags)0);
		Vector2 elementSize = this.GetElementSize();
		foreach (UIElement uielement in childElements)
		{
			uielement.Size = UIUtility.CorrectSizeToAspect(elementSize, uielement.Size.Aspect(), this.correction);
			uielement.LocalPosition = Vector2.zero;
		}
	}

	public AspectCorrection correction;
}
