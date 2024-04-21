using System;
using UnityEngine;

namespace TactileModules.AnimationComponents
{
	public class RotationAnimation : MonoBehaviour
	{
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			Quaternion rhs = Quaternion.Euler(this.xDegreesPerSecond * deltaTime, this.yDegreesPerSecond * deltaTime, this.zDegreesPerSecond * deltaTime);
			if (this.localSpace)
			{
				base.transform.localRotation = base.transform.localRotation * rhs;
			}
			else
			{
				base.transform.rotation = base.transform.rotation * rhs;
			}
		}

		public float xDegreesPerSecond;

		public float yDegreesPerSecond;

		public float zDegreesPerSecond = 30f;

		public bool localSpace;
	}
}
