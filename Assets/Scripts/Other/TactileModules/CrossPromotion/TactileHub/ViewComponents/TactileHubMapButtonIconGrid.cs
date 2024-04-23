using System;
using System.Collections.Generic;
using TactileModules.CrossPromotion.TactileHub.Models;
using UnityEngine;

namespace TactileModules.CrossPromotion.TactileHub.ViewComponents
{
	public class TactileHubMapButtonIconGrid : MonoBehaviour
	{
		public void Initialize(List<IHubGame> hubGames)
		{
			foreach (IHubGame hubGame in hubGames)
			{
				UITextureQuad uitextureQuad = UnityEngine.Object.Instantiate<UITextureQuad>(this.iconPrefab);
				uitextureQuad.gameObject.SetLayerRecursively(base.gameObject.layer);
				uitextureQuad.transform.parent = base.transform;
				uitextureQuad.SetTexture(hubGame.GetIconTexture());
			}
		}

		[SerializeField]
		private UITextureQuad iconPrefab;
	}
}
