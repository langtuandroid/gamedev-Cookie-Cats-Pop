using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story.Views
{
	public class BlackBar
	{
		public BlackBar(UIWidget widget, bool isTop)
		{
			this.widget = widget;
			this.direction = (float)((!isTop) ? -1 : 1);
			UIElement element = widget.GetElement();
			element.autoSizing = (UIAutoSizing.LeftAnchor | UIAutoSizing.RightAnchor);
			element.autoSizing |= ((!isTop) ? UIAutoSizing.BottomAnchor : UIAutoSizing.TopAnchor);
			this.barHeight = element.Size.y;
			element.SetSizeAndDoLayout(new Vector2(widget.transform.parent.GetElementSize().x, this.barHeight));
			element.LocalPosition = this.GetOutTarget();
			this.zPos = widget.transform.position.z;
		}

		private Vector3 GetInTarget()
		{
			Vector2 elementSize = this.widget.transform.parent.GetElementSize();
			return Vector3.up * ((elementSize.y - this.barHeight) * 0.5f) * this.direction;
		}

		private Vector3 GetOutTarget()
		{
			Vector2 elementSize = this.widget.transform.parent.GetElementSize();
			return Vector3.up * ((elementSize.y + this.barHeight) * 0.5f) * this.direction;
		}

		public IEnumerator SlideOut()
		{
			Vector3 target = this.GetOutTarget();
			yield return FiberAnimation.MoveLocalTransform(this.widget.transform, this.widget.transform.localPosition, target, null, 0.3f);
			yield break;
		}

		public IEnumerator SlideIn()
		{
			Vector3 target = this.GetInTarget();
			yield return FiberAnimation.MoveLocalTransform(this.widget.transform, this.widget.transform.localPosition, target, null, 0.3f);
			yield break;
		}

		public void SnapIn()
		{
			Vector3 inTarget = this.GetInTarget();
			inTarget.z = this.zPos;
			this.widget.transform.position = inTarget;
		}

		public void SnapOut()
		{
			Vector3 outTarget = this.GetOutTarget();
			outTarget.z = this.zPos;
			this.widget.transform.position = outTarget;
		}

		private readonly UIWidget widget;

		private readonly float direction;

		private readonly float barHeight;

		private float zPos;
	}
}
