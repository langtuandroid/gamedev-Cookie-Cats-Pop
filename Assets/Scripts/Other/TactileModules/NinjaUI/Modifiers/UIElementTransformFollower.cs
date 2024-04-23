using System;
using UnityEngine;

namespace TactileModules.NinjaUI.Modifiers
{
	public class UIElementTransformFollower : MonoBehaviour
	{
		public Transform Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		private void LateUpdate()
		{
			Vector3 position = this.Target.transform.position;
			position.x = base.transform.position.x;
			position.y = base.transform.position.y;
			this.Target.transform.position = position;
		}

		[SerializeField]
		private Transform target;
	}
}
