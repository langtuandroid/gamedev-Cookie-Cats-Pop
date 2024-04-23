using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(BoxCollider2D))]
	public class MapCharacterZSorter : MapZSorter
	{
		public BoxCollider2D Collider
		{
			get
			{
				if (this.collider == null)
				{
					this.collider = base.GetComponent<BoxCollider2D>();
				}
				return this.collider;
			}
		}

		protected override int CalculateZOrder()
		{
			int num = this.Collider.OverlapCollider(this.contactFilter, this.colliderResults);
			int num2 = 0;
			int num3 = 0;
			if (num > 0)
			{
				MapWall x = null;
				float num4 = float.MaxValue;
				int num5 = int.MinValue;
				int num6 = int.MaxValue;
				bool flag = false;
				for (int i = 0; i < num; i++)
				{
					Collider2D collider2D = this.colliderResults[i];
					if (!(collider2D == this.Collider))
					{
						MapCharacterZSorter component = collider2D.GetComponent<MapCharacterZSorter>();
						if (component != null)
						{
							float y = component.Origin().y;
							float y2 = base.Origin().y;
							if (y != y2)
							{
								if (y > y2)
								{
									num3++;
								}
								else
								{
									num2++;
								}
							}
						}
						else
						{
							MapWall component2 = collider2D.GetComponent<MapWall>();
							if (component2 != null)
							{
								float magnitude = (component2.Origin() - base.Origin()).magnitude;
								bool flag2 = component2.IsBehindWall(base.Origin());
								if (magnitude < num4 || x == null)
								{
									x = component2;
									num4 = magnitude;
									flag = flag2;
								}
								MapZSorter component3 = component2.GetComponent<MapZSorter>();
								int order = component3.Order;
								if (!flag2 && order > num5)
								{
									num5 = order;
								}
								if (flag2 && order < num6)
								{
									num6 = order;
								}
							}
						}
					}
				}
				if (x != null)
				{
					if (flag)
					{
						return num6 - 10 + num3;
					}
					return num5 + 10 - num2;
				}
			}
			return base.CalculateZOrder();
		}

		private BoxCollider2D collider;

		private ContactFilter2D contactFilter = default(ContactFilter2D);

		private const int maxHits = 20;

		private Collider2D[] colliderResults = new Collider2D[20];
	}
}
