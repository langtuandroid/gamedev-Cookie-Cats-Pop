using System;
using UnityEngine;

namespace TactileModules.NinjaUi.Modifiers
{
	public class UISizeModifier : UIWidgetModifier
	{
		public UIElement TargetToScale
		{
			get
			{
				return this.targetToScale;
			}
			set
			{
				this.targetToScale = value;
			}
		}

		public Vector2 SizeModifier
		{
			get
			{
				return this.sizeModifier;
			}
			set
			{
				this.sizeModifier = value;
			}
		}

		protected override void Modify(MeshData mesh)
		{
			if (this.targetToScale == null)
			{
				return;
			}
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(0f, 0f);
			base.GetMinMax(mesh.verts, ref vector, ref vector2);
			Vector2 vector3 = new Vector2(vector2.x - vector.x, vector2.y - vector.y);
			vector3 += this.SizeModifier;
			this.targetToScale.SetSizeAndDoLayout(vector3);
		}

		[SerializeField]
		private UIElement targetToScale;

		[SerializeField]
		private Vector2 sizeModifier = Vector2.zero;
	}
}
