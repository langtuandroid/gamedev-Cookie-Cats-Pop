using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Props
{
	public class ChoosePropFlow
	{
		public ChoosePropFlow(IUIController uiController, MapClickableManager mapClickableManager, PropsManager propsManager, IAssetModel assets)
		{
			this.uiController = uiController;
			this.mapClickableManager = mapClickableManager;
			this.propsManager = propsManager;
			this.assets = assets;
			this.selectedMaterial = assets.GetSelectedProp();
			this.settings = assets.GetChoosePropSettings();
		}

		private MapProp SelectedProp
		{
			get
			{
				return this.selectedProp;
			}
			set
			{
				if (this.selectedProp != null)
				{
					this.selectedProp.SetCurrentVariation(this.previouslySelectedVariationId);
					this.RestoreMaterials(this.selectedProp);
				}
				this.selectedProp = value;
				this.BlinkTimer = 0f;
				if (this.selectedProp != null)
				{
					this.previouslySelectedVariationId = this.selectedProp.CurrentVariationId;
					this.view.Refresh(this.selectedProp);
					this.StoreMaterials(this.selectedProp);
					this.ChangeMaterials(this.selectedProp, this.selectedMaterial);
				}
				if (this.SelectedPropChanged != null)
				{
					this.SelectedPropChanged(this.selectedProp);
				}
			}
		}

		private float BlinkTimer
		{
			get
			{
				return this.blinkTimer;
			}
			set
			{
				this.blinkTimer = value;
				Shader.SetGlobalFloat("_GlowStrength", Mathf.Lerp(0f, this.settings.BlinkStrength, 0.5f - Mathf.Cos(this.blinkTimer) * 0.5f));
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapProp> SelectedPropChanged;

		private void SelectionChanged(int variationId)
		{
			this.selectedProp.SetCurrentVariation(variationId);
			this.selectedAnimator.Run(this.selectedProp.PlaySelectionAnimation(), false);
		}

		private void DismissClicked()
		{
			this.SelectedProp = null;
			this.view.Close(0);
		}

		private void ConfirmClicked()
		{
			MapObjectID component = this.SelectedProp.gameObject.GetComponent<MapObjectID>();
			this.propsManager.SetPropSkin(component.Id, this.SelectedProp.CurrentVariationId);
			this.view.Close(0);
		}

		public IEnumerator Run(MapProp initialProp, bool forcedFlow, Action onStep)
		{
			using (ScopedView<ChoosePropView> scopedView = this.uiController.CreateScopedView<ChoosePropView>(this.assets.GetChoosePropView()))
			{
				this.view = scopedView.CreateView();
				MapClickableManager.ClickableEvent clicked = delegate(Vector3 e, MapClickable clickable, MapProp clickedProp)
				{
					if (clickedProp == null)
					{
						this.DismissClicked();
						return;
					}
					if (clickedProp != this.SelectedProp)
					{
						this.SelectedProp = clickedProp;
					}
				};
				this.mapClickableManager.Clicked += clicked;
				yield return new Fiber.OnExit(delegate()
				{
					this.mapClickableManager.Clicked -= clicked;
					if (this.selectedProp != null)
					{
						this.RestoreMaterials(this.selectedProp);
					}
				});
				this.view.Initialize(new Action<int>(this.SelectionChanged), new Action(this.ConfirmClicked), new Action(this.DismissClicked), !forcedFlow);
				this.SelectedProp = initialProp;
				while (!this.view.IsClosing)
				{
					this.BlinkTimer += this.settings.BlinkSpeed * Time.deltaTime;
					this.selectedAnimator.Step();
					onStep();
					yield return null;
				}
				if (this.SelectedProp != null && forcedFlow)
				{
					yield return this.SelectedProp.WaitForBuildAnimation();
				}
			}
			yield break;
		}

		private void ChangeMaterials(MapProp prop, Material material)
		{
			foreach (Renderer renderer in prop.IterateVariationPivotComponents<Renderer>())
			{
				if (!(renderer.sharedMaterial != GardenGameSetup.Get.propMaterial))
				{
					renderer.sharedMaterial = material;
				}
			}
		}

		private void StoreMaterials(MapProp prop)
		{
			this.storedMaterials.Clear();
			foreach (Renderer renderer in prop.IterateVariationPivotComponents<Renderer>())
			{
				if (!this.storedMaterials.ContainsKey(renderer))
				{
					this.storedMaterials.Add(renderer, renderer.sharedMaterial);
				}
			}
		}

		private void RestoreMaterials(MapProp prop)
		{
			foreach (Renderer renderer in prop.IterateVariationPivotComponents<Renderer>())
			{
				Material sharedMaterial;
				if (this.storedMaterials.TryGetValue(renderer, out sharedMaterial))
				{
					renderer.sharedMaterial = sharedMaterial;
				}
			}
		}

		private readonly IUIController uiController;

		private readonly MapClickableManager mapClickableManager;

		private readonly PropsManager propsManager;

		private readonly IAssetModel assets;

		private readonly Material selectedMaterial;

		private readonly ChoosePropSettings settings;

		private ChoosePropView view;

		private FiberRunner selectedAnimator = new FiberRunner(FiberBucket.Manual);

		private int previouslySelectedVariationId;

		private MapProp selectedProp;

		private readonly Dictionary<Renderer, Material> storedMaterials = new Dictionary<Renderer, Material>();

		private float blinkTimer;
	}
}
