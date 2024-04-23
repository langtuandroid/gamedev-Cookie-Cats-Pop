using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapProp : MapComponent
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapProp> VariationChanged;

		public List<MapProp.Variation> Variations
		{
			get
			{
				return this.variations;
			}
		}

		public bool AnimateRemoval
		{
			get
			{
				return this.animateRemoval;
			}
			set
			{
				this.animateRemoval = value;
			}
		}

		public bool TeardownAndBuildInParallel
		{
			get
			{
				return this.teardownAndBuildInParallel;
			}
			set
			{
				this.teardownAndBuildInParallel = value;
			}
		}

		public bool OnlyAnimateFromDefault
		{
			get
			{
				return this.onlyAnimateFromDefault;
			}
			set
			{
				this.onlyAnimateFromDefault = value;
			}
		}

		public MapProp.Variation CurrentVariation
		{
			get
			{
				List<Transform> childTransforms = this.GetChildTransforms();
				for (int i = 0; i < this.variations.Count; i++)
				{
					if (this.variations[i].IsEnabled(childTransforms))
					{
						return this.variations[i];
					}
				}
				return null;
			}
			set
			{
				this.UpdateVariations(value);
			}
		}

		public int CurrentVariationId
		{
			get
			{
				return (this.CurrentVariation == null) ? -1 : this.CurrentVariation.ID;
			}
		}

		public void SetCurrentVariation(int id)
		{
			foreach (MapProp.Variation variation in this.Variations)
			{
				if (variation.ID == id)
				{
					this.CurrentVariation = variation;
					break;
				}
			}
		}

		public MapProp.Variation GetVariation(int id)
		{
			foreach (MapProp.Variation variation in this.Variations)
			{
				if (variation.ID == id)
				{
					return variation;
				}
			}
			return null;
		}

		public MapProp.Variation DefaultVariation
		{
			get
			{
				for (int i = 0; i < this.variations.Count; i++)
				{
					if (this.variations[i].ID == this.defaultVariationID)
					{
						return this.variations[i];
					}
				}
				return null;
			}
			set
			{
				this.defaultVariationID = ((value == null) ? -1 : value.ID);
			}
		}

		private void UpdateVariations(MapProp.Variation variation)
		{
			this.buildAnimatables.Clear();
			this.teardownAnimatables.Clear();
			this.selectionAnimatables.Clear();
			if (variation != null)
			{
				variation.ApplyPivotStates(this.GetChildTransforms(), delegate(GameObject go)
				{
					IMapPropBuildAnimatable component = go.GetComponent<IMapPropBuildAnimatable>();
					if (component != null)
					{
						this.buildAnimatables.Add(component);
					}
					IMapPropSelectionAnimatable component2 = go.GetComponent<IMapPropSelectionAnimatable>();
					if (component2 != null)
					{
						this.selectionAnimatables.Add(component2);
					}
				}, delegate(GameObject go)
				{
					IMapPropBuildAnimatable component = go.GetComponent<IMapPropBuildAnimatable>();
					if (component != null)
					{
						this.teardownAnimatables.Add(component);
					}
				});
			}
			if (this.VariationChanged != null)
			{
				this.VariationChanged(this);
			}
		}

		protected override void Initialized()
		{
			this.UpdateVariations(this.DefaultVariation);
		}

		public int CalculateNewVariationId()
		{
			int num = 0;
			for (int i = 0; i < this.variations.Count; i++)
			{
				num = Mathf.Max(this.variations[i].ID, num);
			}
			return num + 1;
		}

		public List<Transform> GetChildTransforms()
		{
			MapProp.getTransformsList.Clear();
			this.FindTransforms(base.transform, MapProp.getTransformsList);
			return MapProp.getTransformsList;
		}

		private void FindTransforms(Transform t, List<Transform> transforms)
		{
			if (t.hideFlags != HideFlags.None)
			{
				return;
			}
			if (t != base.transform)
			{
				transforms.Add(t);
			}
			for (int i = 0; i < t.childCount; i++)
			{
				this.FindTransforms(t.GetChild(i), transforms);
			}
		}

		public MapProp.Variation CreateSnapshot()
		{
			MapProp.Variation variation = new MapProp.Variation();
			variation.StorePivotStates(this.GetChildTransforms());
			return variation;
		}

		private IEnumerator[] GetAnimationList(List<IMapPropBuildAnimatable> animatables, bool isTeardown)
		{
			IEnumerator[] array = new IEnumerator[animatables.Count];
			for (int i = 0; i < animatables.Count; i++)
			{
				array[i] = animatables[i].Build(isTeardown);
			}
			return array;
		}

		public IEnumerator PlaySelectionAnimation()
		{
			IEnumerator[] list = new IEnumerator[this.selectionAnimatables.Count];
			for (int i = 0; i < this.selectionAnimatables.Count; i++)
			{
				list[i] = this.selectionAnimatables[i].PlaySelectionAnimation();
			}
			yield return FiberHelper.RunParallel(list);
			yield break;
		}

		public IEnumerator WaitForBuildAnimation()
		{
			IEnumerator[] teardowns = null;
			IEnumerator[] builds = this.GetAnimationList(this.buildAnimatables, false);
			if (this.AnimateRemoval)
			{
				teardowns = this.GetAnimationList(this.teardownAnimatables, true);
				for (int i = 0; i < this.teardownAnimatables.Count; i++)
				{
					this.teardownAnimatables[i].BuildInitialize(true);
				}
			}
			for (int j = 0; j < this.buildAnimatables.Count; j++)
			{
				this.buildAnimatables[j].BuildInitialize(false);
			}
			if (!this.AnimateRemoval)
			{
				yield return FiberHelper.RunParallel(builds);
			}
			else if (this.teardownAndBuildInParallel)
			{
				IEnumerator[] allAnims = new IEnumerator[teardowns.Length + builds.Length];
				int count = 0;
				for (int k = 0; k < teardowns.Length; k++)
				{
					allAnims[count] = teardowns[k];
					count++;
				}
				for (int l = 0; l < builds.Length; l++)
				{
					allAnims[count] = builds[l];
					count++;
				}
				yield return FiberHelper.RunParallel(allAnims);
			}
			else
			{
				yield return FiberHelper.RunParallel(teardowns);
				yield return FiberHelper.RunParallel(builds);
			}
			if (this.VariationChanged != null)
			{
				this.VariationChanged(this);
			}
			yield break;
		}

		public IEnumerable<GameObject> IterateVariationPivots()
		{
			foreach (MapProp.Variation variation in this.variations)
			{
				foreach (GameObject pivot in variation.IteratePivots())
				{
					yield return pivot;
				}
			}
			yield break;
		}

		public IEnumerable<T> IterateVariationPivotComponents<T>() where T : Component
		{
			HashSet<T> uniqueComponents = new HashSet<T>();
			List<T> getComponentsList = new List<T>();
			foreach (GameObject gameObject in this.IterateVariationPivots())
			{
				gameObject.GetComponentsInChildren<T>(true, getComponentsList);
				for (int i = 0; i < getComponentsList.Count; i++)
				{
					uniqueComponents.Add(getComponentsList[i]);
				}
			}
			foreach (T component in uniqueComponents)
			{
				yield return component;
			}
			yield break;
		}

		public MapProp.Variation FirstPickableVariation
		{
			get
			{
				for (int i = 0; i < this.variations.Count; i++)
				{
					if (this.variations[i].IsPickable)
					{
						return this.variations[i];
					}
				}
				return null;
			}
		}

		public bool VariationIdPickable(int pickableId)
		{
			for (int i = 0; i < this.variations.Count; i++)
			{
				if (this.variations[i].ID == pickableId && this.variations[i].IsPickable)
				{
					return true;
				}
			}
			return false;
		}

		[SerializeField]
		private List<MapProp.Variation> variations = new List<MapProp.Variation>();

		[SerializeField]
		private int defaultVariationID;

		[SerializeField]
		private bool animateRemoval = true;

		[SerializeField]
		private bool teardownAndBuildInParallel;

		[SerializeField]
		private bool onlyAnimateFromDefault;

		private static readonly List<Transform> getTransformsList = new List<Transform>();

		private readonly List<IMapPropSelectionAnimatable> selectionAnimatables = new List<IMapPropSelectionAnimatable>();

		private readonly List<IMapPropBuildAnimatable> buildAnimatables = new List<IMapPropBuildAnimatable>();

		private readonly List<IMapPropBuildAnimatable> teardownAnimatables = new List<IMapPropBuildAnimatable>();

		[Serializable]
		public class PivotState
		{
			public GameObject Pivot;

			public bool Enabled;
		}

		[Serializable]
		public class Variation
		{
			public TextureResource Icon
			{
				get
				{
					return this.image;
				}
				set
				{
					this.image = value;
				}
			}

			public int ID
			{
				get
				{
					return this.id;
				}
				set
				{
					this.id = value;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			public bool IsPickable
			{
				get
				{
					return !string.IsNullOrEmpty(this.Icon.path);
				}
			}

			private void StorePivotState(GameObject pivot)
			{
				this.pivotStates.Add(new MapProp.PivotState
				{
					Pivot = pivot,
					Enabled = pivot.activeSelf
				});
			}

			public void StorePivotStates(List<Transform> transforms)
			{
				this.pivotStates.Clear();
				for (int i = 0; i < MapProp.getTransformsList.Count; i++)
				{
					this.StorePivotState(MapProp.getTransformsList[i].gameObject);
				}
			}

			public bool TryGetPivot(GameObject go, out MapProp.PivotState pivot)
			{
				if (Application.isPlaying)
				{
					if (this.gameObjectToPivotStates == null)
					{
						this.gameObjectToPivotStates = new Dictionary<GameObject, MapProp.PivotState>();
						for (int i = 0; i < this.pivotStates.Count; i++)
						{
							if (!(this.pivotStates[i].Pivot == null))
							{
								if (!this.gameObjectToPivotStates.ContainsKey(this.pivotStates[i].Pivot))
								{
									this.gameObjectToPivotStates.Add(this.pivotStates[i].Pivot, this.pivotStates[i]);
								}
							}
						}
					}
					return this.gameObjectToPivotStates.TryGetValue(go, out pivot);
				}
				for (int j = 0; j < this.pivotStates.Count; j++)
				{
					if (this.pivotStates[j].Pivot == go)
					{
						pivot = this.pivotStates[j];
						return true;
					}
				}
				pivot = null;
				return false;
			}

			public void ApplyPivotStates(List<Transform> transforms, Action<GameObject> enabled = null, Action<GameObject> disabled = null)
			{
				for (int i = 0; i < transforms.Count; i++)
				{
					GameObject gameObject = transforms[i].gameObject;
					bool activeSelf = gameObject.activeSelf;
					MapProp.PivotState pivotState;
					bool flag = this.TryGetPivot(gameObject, out pivotState) && pivotState.Enabled;
					if (flag != activeSelf)
					{
						gameObject.SetActive(flag);
						if (flag)
						{
							if (enabled != null)
							{
								enabled(gameObject);
							}
						}
						else if (disabled != null)
						{
							disabled(gameObject);
						}
					}
				}
			}

			public bool IsEnabled(List<Transform> transforms)
			{
				for (int i = 0; i < transforms.Count; i++)
				{
					GameObject gameObject = transforms[i].gameObject;
					MapProp.PivotState pivotState;
					if (!this.TryGetPivot(gameObject, out pivotState))
					{
						if (gameObject.activeSelf)
						{
							return false;
						}
					}
					else if (gameObject.activeSelf != pivotState.Enabled)
					{
						return false;
					}
				}
				return true;
			}

			public IEnumerable<GameObject> IteratePivots()
			{
				for (int i = 0; i < this.pivotStates.Count; i++)
				{
					if (this.pivotStates[i].Enabled && this.pivotStates[i].Pivot != null)
					{
						yield return this.pivotStates[i].Pivot;
					}
				}
				yield break;
			}

			[SerializeField]
			private List<MapProp.PivotState> pivotStates = new List<MapProp.PivotState>();

			[SerializeField]
			private TextureResource image;

			[SerializeField]
			private int id = -1;

			[SerializeField]
			private string name;

			private Dictionary<GameObject, MapProp.PivotState> gameObjectToPivotStates;
		}
	}
}
