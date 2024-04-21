using System;
using UnityEngine;

public class UIVerticalListVariableHeightItems : UIListPanelBase
{
	public override void AddToContent(UIElement e)
	{
		this.AddToContent(e, true);
	}

	public void AddToContent(UIElement e, bool doLayout)
	{
		e.transform.parent = base.ScrollRoot;
		e.autoSizing = (UIAutoSizing.LeftAnchor | UIAutoSizing.RightAnchor);
		e.gameObject.SetLayerRecursively(base.gameObject.layer);
		if (doLayout)
		{
			e.SetSizeAndDoLayout(new Vector2(base.Size.x, e.Size.y));
		}
		this.PositionElementYAxis(e);
		this.CalculateNextEdgePositionYAxis(e);
	}

	private void PositionElementYAxis(UIElement currentElement)
	{
		int num = (!this.startFromTop) ? -1 : 1;
		currentElement.transform.localPosition = new Vector3(currentElement.Size.x * 0.5f, this.elapsed.y - currentElement.Size.y * 0.5f * (float)num, this.cellZPosition);
	}

	private void CalculateNextEdgePositionYAxis(UIElement currentElement)
	{
		Vector3 localPosition = currentElement.transform.localPosition;
		float num = currentElement.Size.y * 0.5f + this.paddingAmount;
		if (this.startFromTop)
		{
			this.elapsed = new Vector3(localPosition.x, localPosition.y - num, localPosition.z);
		}
		else
		{
			this.elapsed = new Vector3(localPosition.x, localPosition.y + num, localPosition.z);
		}
	}
}
