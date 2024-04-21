using System;
using UnityEngine;

public class UIListPanel : UIListPanelBase
{
	public override void AddToContent(UIElement e)
	{
		e.transform.parent = base.ScrollRoot;
		e.transform.localPosition = new Vector3(e.Size.x * 0.5f, e.Size.y * 0.5f, this.cellZPosition) + this.elapsed;
		e.autoSizing = (UIAutoSizing.LeftAnchor | UIAutoSizing.RightAnchor);
		e.gameObject.SetLayerRecursively(base.gameObject.layer);
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal)
		{
			e.SetSizeAndDoLayout(new Vector2(e.Size.x, base.Size.y));
			this.elapsed.x = this.elapsed.x + (e.Size.x + this.paddingAmount);
		}
		else
		{
			e.SetSizeAndDoLayout(new Vector2(base.Size.x, e.Size.y));
			if (this.startFromTop)
			{
				this.elapsed.y = this.elapsed.y - (e.Size.y + this.paddingAmount);
			}
			else
			{
				this.elapsed.y = this.elapsed.y + (e.Size.y + this.paddingAmount);
			}
		}
	}
}
