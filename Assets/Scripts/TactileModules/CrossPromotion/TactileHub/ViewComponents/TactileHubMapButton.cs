using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.ComponentLifecycle;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.TactileHub.Models;
using TactileModules.CrossPromotion.TactileHub.ViewControllers;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.ViewComponents
{
	[RequireComponent(typeof(UIButton))]
	public class TactileHubMapButton : LifecycleBroadcaster
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnButtonClicked;



		public void Initialize(List<IHubGame> tactileHubGames)
		{
			this.hubGames = tactileHubGames;
			if (this.hubGames.Count > 0)
			{
				this.InstantiateIconGrid();
				base.GetComponent<UIButton>().Clicked += delegate(UIButton button)
				{
					this.OnButtonClicked();
				};
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		private void InstantiateIconGrid()
		{
			GameObject original = Resources.Load<GameObject>("CrossPromotion/TactileHub/Prefabs/TactileHubMapButtonIconGrid");
			TactileHubMapButtonIconGrid component = UnityEngine.Object.Instantiate<GameObject>(original).GetComponent<TactileHubMapButtonIconGrid>();
			component.Initialize(this.hubGames);
			component.gameObject.SetLayerRecursively(base.gameObject.layer);
			component.transform.parent = base.transform;
			component.transform.localPosition = new Vector3(0f, 0f, -5f);
		}

		private TactileHubViewControllerFactory viewControllerFactory;

		private IGeneralDataRetriever dataRetriever;

		private const string RESOURCE_PATH = "CrossPromotion/TactileHub/Prefabs/TactileHubMapButtonIconGrid";

		private List<IHubGame> hubGames;
	}
}
