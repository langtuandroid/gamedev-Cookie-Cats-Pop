using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class TaskCompletedVisuals : ExtensibleVisual<ITaskCompletedVisualsExtension>
	{
		public void Initialize(MapTask t)
		{
			this.image.SetTexture(t.Icon);
			this.title.text = L.Get(t.Title);
		}

		public IEnumerator AnimateCompleted(Vector3 endPosition)
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateCompleted(endPosition);
			}
			yield break;
		}

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UITextureQuad image;
	}
}
