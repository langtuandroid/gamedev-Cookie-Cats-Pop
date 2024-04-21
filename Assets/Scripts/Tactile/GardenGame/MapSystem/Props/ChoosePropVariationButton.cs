using System;
using TactileModules.Validation;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class ChoosePropVariationButton : MonoBehaviour
	{
		public UIButton Button
		{
			get
			{
				return this.button;
			}
		}

		public int Id { get; set; }

		public void Refresh(Texture2D icon)
		{
			this.iconQuad.SetTexture(icon);
			if (this.alphaTexture != null && this.alphaSplitShader != null)
			{
				this.UpdateSplitAlpha(icon);
			}
		}

		public bool Selected
		{
			get
			{
				return this.selectedPivot.activeSelf;
			}
			set
			{
				this.selectedPivot.SetActive(value);
				this.deselectedPivot.SetActive(!value);
			}
		}

		private void UpdateSplitAlpha(Texture2D icon)
		{
			Material sharedMaterial = this.iconQuad.GetComponent<Renderer>().sharedMaterial;
			sharedMaterial.shader = this.alphaSplitShader;
			sharedMaterial.SetTexture("_MainTex", icon);
			sharedMaterial.SetTexture("_AlphaTex", this.alphaTexture);
		}

		[SerializeField]
		private UITextureQuad iconQuad;

		[SerializeField]
		private GameObject selectedPivot;

		[SerializeField]
		private GameObject deselectedPivot;

		[SerializeField]
		private UIButton button;

		[SerializeField]
		[OptionalSerializedField]
		private Texture2D alphaTexture;

		[SerializeField]
		[OptionalSerializedField]
		private Shader alphaSplitShader;
	}
}
