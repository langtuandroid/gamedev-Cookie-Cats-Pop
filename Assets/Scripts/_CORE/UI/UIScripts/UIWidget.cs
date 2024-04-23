using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class UIWidget : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<MeshData> OnPostFill;

	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			if (this.color != value)
			{
				this.color = value;
				this.changeFlags |= UIChangeFlags.Colors;
			}
		}
	}

	public float Alpha
	{
		get
		{
			return this.color.a;
		}
		set
		{
			Color color = this.color;
			color.a = value;
			this.Color = color;
		}
	}

	public UIPivot Pivot
	{
		get
		{
			return this.pivot;
		}
		set
		{
			if (this.pivot != value)
			{
				this.pivot = value;
				this.changeFlags = UIChangeFlags.All;
			}
		}
	}

	protected abstract Material GetMaterial();

	protected virtual bool GetMaterials(int index, out Material m)
	{
		if (index == 0)
		{
			m = this.GetMaterial();
			return true;
		}
		m = null;
		return false;
	}

	public Renderer renderer
	{
		get
		{
			if (this.cachedRendererComponent == null)
			{
				this.cachedRendererComponent = base.GetComponent<Renderer>();
			}
			return this.cachedRendererComponent;
		}
	}

	public Texture MainTexture
	{
		get
		{
			Material material = this.GetMaterial();
			if (material != null)
			{
				return material.mainTexture;
			}
			return null;
		}
	}

	protected virtual void Initialize()
	{
	}

	protected abstract void OnFill(MeshData meshData);

	public void CorrectAspect()
	{
		this.CorrectAspect(AspectCorrection.Fit);
	}

	public virtual void CorrectAspect(AspectCorrection correction)
	{
	}

	public virtual bool RendererInspectorsHidden
	{
		get
		{
			return true;
		}
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	private void Awake()
	{
		this.EnsureMesh();
		this.Initialize();
		this.MarkAsChanged();
		this.UpdateGeometry();
	}

	protected virtual void OnDestroy()
	{
		UIWidget.DestroyUnityObject(this.mesh);
		this.mesh = null;
	}

	public static void DestroyUnityObject(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return;
		}
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(obj);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	private void EnsureMesh()
	{
		if (this.mesh != null)
		{
			return;
		}
		this.mesh = new Mesh();
		MeshFilter component = base.GetComponent<MeshFilter>();
		component.mesh = this.mesh;
		base.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
		base.GetComponent<Renderer>().receiveShadows = false;
		if (this.Element != null)
		{
			Vector3 localScale = base.transform.localScale;
			if (localScale.z != 0.987f)
			{
				localScale.z = 0.987f;
				base.transform.localScale = localScale;
			}
		}
	}

	protected static void RemoveFromMarkAsChangedInLateUpdate(UIWidget widget)
	{
		UIWidget.dirtyWidgetsInLateUpdate.Remove(widget);
	}

	protected static void MarkAsChangedInLateUpdate(UIWidget widget)
	{
		UIWidget.dirtyWidgetsInLateUpdate.Add(widget);
	}

	protected virtual void LateUpdate()
	{
		this.TryUpdateGeometry();
		int num = 10000;
		while (--num > 0)
		{
			if (UIWidget.dirtyWidgetsInLateUpdate.Count == 0)
			{
				break;
			}
			UIWidget.dirtyWidgetsInLateUpdateIterator.Clear();
			foreach (UIWidget item in UIWidget.dirtyWidgetsInLateUpdate)
			{
				UIWidget.dirtyWidgetsInLateUpdateIterator.Add(item);
			}
			UIWidget.dirtyWidgetsInLateUpdate.Clear();
			foreach (UIWidget uiwidget in UIWidget.dirtyWidgetsInLateUpdateIterator)
			{
				uiwidget.MarkAsChanged();
				uiwidget.TryUpdateGeometry();
			}
		}
	}

	private void TryUpdateGeometry()
	{
		if (this.changeFlags != UIChangeFlags.None)
		{
			this.UpdateGeometry();
			this.changeFlags = UIChangeFlags.None;
		}
	}

	public virtual void MarkAsChanged()
	{
		this.changeFlags = UIChangeFlags.All;
	}

	public bool IsMarkedForChange
	{
		get
		{
			return this.changeFlags != UIChangeFlags.None;
		}
	}

	public virtual bool UpdateGeometryColorsOnly(Mesh m, List<Color> buffer)
	{
		return false;
	}

	public bool UpdateGeometry()
	{
		if (this.mesh == null)
		{
			return false;
		}
		if (this.changeFlags == UIChangeFlags.None)
		{
			return false;
		}
		UIWidget.reusableMeshData.Clear();
		if (this.changeFlags == UIChangeFlags.Colors && this.UpdateGeometryColorsOnly(this.mesh, UIWidget.reusableMeshData.colors))
		{
			return true;
		}
		this.mesh.Clear();
		this.OnFill(UIWidget.reusableMeshData);
		if (this.OnPostFill != null)
		{
			this.OnPostFill(UIWidget.reusableMeshData);
		}
		this.changeFlags = UIChangeFlags.None;
		if (!UIWidget.reusableMeshData.IsEmpty())
		{
			Vector3 offset = new Vector2(-0.5f, 0.5f);
			Vector2 size = this.Size;
			offset.x *= size.x;
			offset.y *= size.y;
			UIWidget.reusableMeshData.ApplyToMesh(this.mesh, offset);
			if (UIWidget.reusableMeshData.numSubmeshes != this.prevAmountOfMaterials)
			{
				this.reusableMaterialList.Clear();
				int num = 0;
				Material item;
				while (this.GetMaterials(num++, out item))
				{
					if (this.reusableMaterialList.size >= UIWidget.reusableMeshData.numSubmeshes)
					{
						break;
					}
					this.reusableMaterialList.Add(item);
				}
				this.prevAmountOfMaterials = this.reusableMaterialList.size;
				this.renderer.sharedMaterials = this.reusableMaterialList.ToArray();
			}
		}
		return true;
	}

	public virtual Vector2 PivotOffset
	{
		get
		{
			Vector2 zero = Vector2.zero;
			if (this.pivot == UIPivot.Top || this.pivot == UIPivot.Center || this.pivot == UIPivot.Bottom)
			{
				zero.x = -0.5f;
			}
			else if (this.pivot == UIPivot.TopRight || this.pivot == UIPivot.Right || this.pivot == UIPivot.BottomRight)
			{
				zero.x = -1f;
			}
			if (this.pivot == UIPivot.Left || this.pivot == UIPivot.Center || this.pivot == UIPivot.Right)
			{
				zero.y = 0.5f;
			}
			else if (this.pivot == UIPivot.BottomLeft || this.pivot == UIPivot.Bottom || this.pivot == UIPivot.BottomRight)
			{
				zero.y = 1f;
			}
			return zero;
		}
	}

	protected void MarkMaterialsDirty()
	{
		this.prevAmountOfMaterials = 0;
	}

	public UIElement Element
	{
		get
		{
			return base.GetComponent<UIElement>();
		}
	}

	public Vector2 Size
	{
		get
		{
			if (this.Element == null)
			{
				return Vector2.one;
			}
			return this.Element.Size;
		}
		set
		{
			if (this.Element != null)
			{
				this.Element.Size = value;
				Vector3 localScale = base.transform.localScale;
				if (localScale.z != 0.987f)
				{
					localScale.z = 0.987f;
					base.transform.localScale = localScale;
				}
				this.MarkAsChanged();
			}
		}
	}

	private void UIElementNeedDefaultSize()
	{
		this.Size = base.GetComponent<Renderer>().bounds.size;
	}

	public const float DEFAULT_SCALE_Z = 0.987f;

	public static readonly Vector3 DEFAULT_VECTOR3_ONE = new Vector3(1f, 1f, 0.987f);

	private static readonly MeshData reusableMeshData = new MeshData();

	private readonly BetterList<Material> reusableMaterialList = new BetterList<Material>();

	private int prevAmountOfMaterials;

	private static readonly HashSet<UIWidget> dirtyWidgetsInLateUpdate = new HashSet<UIWidget>();

	private static readonly HashSet<UIWidget> dirtyWidgetsInLateUpdateIterator = new HashSet<UIWidget>();

	[HideInInspector]
	[SerializeField]
	private Color color = Color.white;

	[HideInInspector]
	[SerializeField]
	private UIPivot pivot = UIPivot.Center;

	protected UIChangeFlags changeFlags = UIChangeFlags.All;

	protected Mesh mesh;

	private Renderer cachedRendererComponent;
}
