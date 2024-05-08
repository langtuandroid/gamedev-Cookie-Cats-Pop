using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGame.PlayablePostcard.Analytics;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.PlayablePostcard.UI;
using TactileModules.PuzzleGame.PlayablePostcard.Views;

namespace TactileModules.PuzzleGame.PlayablePostcard.Controllers
{
	public class PhotoBoothController
	{
		public PhotoBoothController(PlayablePostcardProgress model, PlayablePostcardActivation activation, UIViewManager uiViewManager)
		{
			this.model = model;
			this.activation = activation;
			this.uiViewManager = uiViewManager;
			this.itemContainer = activation.GetAssetBundle();
		}

		public IEnumerator ShowBooth(string optionalLevelSessionId = "")
		{
			this.levelSessionId = optionalLevelSessionId;
			this.currentItemType = this.model.GetCurrentItemType();
			UIViewManager.UIViewStateGeneric<PlayablePostcardCreationView> vs = this.uiViewManager.ShowView<PlayablePostcardCreationView>(new object[0]);
			this.view = vs.View;
			this.view.Initialize(this.itemContainer, this.model.GetPersistedPostcard());
			this.view.OnConfirmItem += this.ConfirmItem;
			this.view.OnFinalizedPostcard += this.FinalizedPostcard;
			this.view.OnSharePostcard += this.SharePostcard;
			this.view.StartFlow(this.currentItemType);
			yield return vs.WaitForClose();
			yield break;
		}

		public IEnumerator ShowResultPostcard()
		{
			UIViewManager.UIViewStateGeneric<PlayablePostcardCreationView> vs = this.uiViewManager.ShowView<PlayablePostcardCreationView>(new object[0]);
			this.view = vs.View;
			this.view.Initialize(this.itemContainer, this.model.GetPersistedPostcard());
			this.view.ShowFinalResult(0.4f);
			this.view.OnSharePostcard += this.SharePostcard;
			yield return vs.WaitForClose();
			yield break;
		}

		private void ConfirmItem(string id)
		{
			this.model.UpdatePersistedPostcard(this.currentItemType, id);
		}

		private void FinalizedPostcard(List<PostcardItemTypeAndId> postcardData)
		{
			string postcardId = this.GetPostcardId(postcardData);
		}

		private void SharePostcard(List<PostcardItemTypeAndId> postcardData)
		{
			string postcardId = this.GetPostcardId(postcardData);
		}

		private string GetPostcardId(List<PostcardItemTypeAndId> postcardData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PostcardItemTypeAndId postcardItemTypeAndId in postcardData)
			{
				stringBuilder.Append(postcardItemTypeAndId.Type);
				stringBuilder.Append("_");
				stringBuilder.Append(postcardItemTypeAndId.Id);
				if (postcardItemTypeAndId != postcardData.Last<PostcardItemTypeAndId>())
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		private readonly PlayablePostcardProgress model;

		private readonly PostcardItemContainer itemContainer;

		private readonly PlayablePostcardActivation activation;

		private readonly UIViewManager uiViewManager;

		private PlayablePostcardCreationView view;

		private PostcardItemType currentItemType;

		private string levelSessionId;
	}
}
