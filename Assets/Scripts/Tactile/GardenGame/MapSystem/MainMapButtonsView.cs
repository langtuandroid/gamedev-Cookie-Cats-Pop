using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MainMapButtonsView : UIView
	{
		public GameObject GetButtonAreaRoot()
		{
			return base.gameObject;
		}

		public Camera UICamera
		{
			get
			{
				return base.ViewCamera;
			}
		}

		private void Update()
		{
		}
	}
}
