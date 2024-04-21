using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[ExecuteInEditMode]
public class Instantiator : MonoBehaviour
{
	protected virtual void Awake()
	{
		if (this.instance == null)
		{
			this.CreateInstance();
		}
	}

	protected virtual void OnDestroy()
	{
		this.DestroyInstance();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void SetHideFlags(Transform p, HideFlags flags)
	{
		p.gameObject.hideFlags = flags;
		p.hideFlags = flags;
		IEnumerator enumerator = p.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform p2 = (Transform)obj;
				this.SetHideFlags(p2, flags);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private static bool IsAncestorOfEmbeddedPrefabs(Transform t)
	{
		return !(t == null) && (t.GetComponent<EmbeddedPrefabs>() != null || Instantiator.IsAncestorOfEmbeddedPrefabs(t.parent));
	}

	public virtual void CreateInstance()
	{
		if (Application.isPlaying && Instantiator.IsAncestorOfEmbeddedPrefabs(base.transform))
		{
			return;
		}
		this.DestroyInstance();
		if (this.prefab == null)
		{
			return;
		}
		this.instance = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
		this.instance.name = "_temp_" + (uint)this.instance.GetInstanceID();
		this.instanceTraits = this.instance.GetComponent<InstantiatorTraits>();
		if (this.instanceTraits != null)
		{
			this.instanceTraits.Instantiator = this;
		}
		Instantiator.SetLayerRecursively(this.instance, base.gameObject.layer);
		if (!Application.isPlaying)
		{
			this.instance.transform.localScale = base.transform.lossyScale;
			this.instance.transform.position = base.transform.position;
			this.instance.transform.rotation = base.transform.rotation;
			this.SetHideFlags(this.instance.transform, HideFlags.HideAndDontSave);
		}
		else
		{
			this.instance.transform.parent = base.transform;
			this.instance.transform.localScale = Vector3.one;
			this.instance.transform.localPosition = Vector3.zero;
			this.instance.transform.localRotation = Quaternion.identity;
			this.SetHideFlags(this.instance.transform, HideFlags.DontSave);
		}
		this.ApplyParameters(false);
	}

	public virtual void DestroyInstance()
	{
		if (this.instance == null)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			UnityEngine.Object.DestroyImmediate(this.instance.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(this.instance.gameObject);
		}
		this.instance = null;
		this.instanceTraits = null;
	}

	private static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;
		Transform transform = go.transform;
		int i = 0;
		int childCount = transform.childCount;
		while (i < childCount)
		{
			Transform child = transform.GetChild(i);
			Instantiator.SetLayerRecursively(child.gameObject, layer);
			i++;
		}
	}

	public static IEnumerable<Instantiator.ComponentPropertyInfo> GetSerializedProperties(GameObject instance)
	{
		foreach (MonoBehaviour comp in instance.GetComponents<MonoBehaviour>())
		{
			Type type = comp.GetType();
			foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				object[] attr = info.GetCustomAttributes(typeof(Instantiator.SerializeProperty), true);
				if (attr.Length != 0)
				{
					yield return new Instantiator.ComponentPropertyInfo
					{
						propertyInfo = info,
						component = comp,
						attribute = (Instantiator.SerializeProperty)attr[0]
					};
				}
			}
		}
		yield break;
	}

	public static IEnumerable<Instantiator.ComponentPropertyInfo> GetSerializedPropertiesFromComponent(MonoBehaviour component)
	{
		Type type = component.GetType();
		foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
		{
			object[] attr = info.GetCustomAttributes(typeof(Instantiator.SerializeProperty), true);
			if (attr.Length != 0)
			{
				yield return new Instantiator.ComponentPropertyInfo
				{
					propertyInfo = info,
					component = component,
					attribute = (Instantiator.SerializeProperty)attr[0]
				};
			}
		}
		yield break;
	}

	public static IEnumerable<Instantiator.ComponentPropertyInfo> GetCachedSerializedProperties(GameObject instance)
	{
		if (Instantiator.propertyInfosCache == null)
		{
			Instantiator.propertyInfosCache = new Dictionary<int, List<Instantiator.ComponentPropertyInfo>>();
		}
		List<Instantiator.ComponentPropertyInfo> listInfos;
		if (!Instantiator.propertyInfosCache.TryGetValue(instance.GetInstanceID(), out listInfos))
		{
			listInfos = new List<Instantiator.ComponentPropertyInfo>();
			Instantiator.propertyInfosCache.Add(instance.GetInstanceID(), listInfos);
			foreach (MonoBehaviour monoBehaviour in instance.GetComponents<MonoBehaviour>())
			{
				Type type = monoBehaviour.GetType();
				foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
				{
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(Instantiator.SerializeProperty), true);
					if (customAttributes.Length != 0)
					{
						listInfos.Add(new Instantiator.ComponentPropertyInfo
						{
							propertyInfo = propertyInfo,
							component = monoBehaviour,
							attribute = (Instantiator.SerializeProperty)customAttributes[0]
						});
					}
				}
			}
		}
		for (int i = 0; i < listInfos.Count; i++)
		{
			yield return listInfos[i];
		}
		yield break;
	}

	public void ApplyParametersAsHashtable(Hashtable parameters, bool useCachedPropertyInfos = false)
	{
		this.ApplyParameters(parameters, useCachedPropertyInfos);
	}

	protected void ApplyParameters(bool useCachedPropertyInfos = false)
	{
		Hashtable jsonTable = MiniJSON.jsonDecode(this.jsonString) as Hashtable;
		this.ApplyParameters(jsonTable, useCachedPropertyInfos);
	}

	protected void ApplyParameters(Hashtable jsonTable, bool useCachedPropertyInfos = false)
	{
		foreach (Instantiator.ComponentPropertyInfo componentPropertyInfo in ((!useCachedPropertyInfos) ? Instantiator.GetSerializedProperties(this.instance) : Instantiator.GetCachedSerializedProperties(this.instance)))
		{
			if (componentPropertyInfo.propertyInfo.PropertyType == typeof(Instantiator.MethodReference))
			{
				string message=string.Empty;
				if (jsonTable != null && jsonTable.ContainsKey(componentPropertyInfo.propertyInfo.Name) && jsonTable[componentPropertyInfo.propertyInfo.Name] != null)
				{
					message = (jsonTable[componentPropertyInfo.propertyInfo.Name] as string);
				}
				else
				{
					Instantiator.SerializeProperty attribute = componentPropertyInfo.attribute;
					message = (attribute.DefaultValue as string);
				}
                UnityEngine.Debug.Log("message " + message);
                Instantiator.MethodReference methodReference = new Instantiator.MethodReference
				{
					receiver = (GameObject)this.GetReference(componentPropertyInfo.propertyInfo.Name),
					message = message
				};
                //UnityEngine.Debug.Log("methodReference " + methodReference);
				componentPropertyInfo.propertyInfo.SetValue(componentPropertyInfo.component, methodReference, null);
			}
			else if (componentPropertyInfo.propertyInfo.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				UnityEngine.Object reference = this.GetReference(componentPropertyInfo.propertyInfo.Name);
				if (componentPropertyInfo.propertyInfo.PropertyType.IsSubclassOf(typeof(InstantiatorTraits)))
				{
					Instantiator instantiator = reference as Instantiator;
					if (instantiator != null && instantiator.GetInstance() == null)
					{
						instantiator.CreateInstance();
					}
					InstantiatorTraits value = (!(instantiator != null)) ? null : instantiator.GetInstance<InstantiatorTraits>();
					componentPropertyInfo.propertyInfo.SetValue(componentPropertyInfo.component, value, null);
				}
				else if (reference != null && (reference.GetType() == componentPropertyInfo.propertyInfo.PropertyType || reference.GetType().IsSubclassOf(componentPropertyInfo.propertyInfo.PropertyType)))
				{
					componentPropertyInfo.propertyInfo.SetValue(componentPropertyInfo.component, reference, null);
				}
				else
				{
					componentPropertyInfo.propertyInfo.SetValue(componentPropertyInfo.component, null, null);
				}
			}
			else if (jsonTable != null && jsonTable.ContainsKey(componentPropertyInfo.propertyInfo.Name) && jsonTable[componentPropertyInfo.propertyInfo.Name] != null)
			{
				if (componentPropertyInfo.propertyInfo.PropertyType == typeof(string) && componentPropertyInfo.attribute is Instantiator.SerializeLocalizableProperty)
				{
					string data = L.Get(jsonTable[componentPropertyInfo.propertyInfo.Name].ToString());
					JsonSerializer.SetPropertyFromTableEntry(componentPropertyInfo.component, componentPropertyInfo.propertyInfo, data);
				}
				else if (!JsonSerializer.SetPropertyFromTableEntry(componentPropertyInfo.component, componentPropertyInfo.propertyInfo, jsonTable[componentPropertyInfo.propertyInfo.Name].ToString()))
				{
					componentPropertyInfo.propertyInfo.SetValue(componentPropertyInfo.component, JsonSerializer.HashtableToObject(componentPropertyInfo.propertyInfo.PropertyType, (Hashtable)jsonTable[componentPropertyInfo.propertyInfo.Name], JsonSerializer.SerializationType.WithPreAndPostCallbacks), null);
				}
			}
			else
			{
				Instantiator.SerializeProperty attribute2 = componentPropertyInfo.attribute;
				componentPropertyInfo.propertyInfo.SetValue(componentPropertyInfo.component, attribute2.DefaultValue, null);
			}
		}
	}

	protected virtual void Update()
	{
	}

	public GameObject GetInstance()
	{
		return this.instance;
	}

	public T GetInstance<T>() where T : Component
	{
		GameObject gameObject = this.GetInstance();
		return (!(gameObject != null)) ? ((T)((object)null)) : gameObject.GetComponent<T>();
	}

	public InstantiatorTraits InstanceTraits
	{
		get
		{
			return this.instanceTraits;
		}
	}

	private void OnDrawGizmos()
	{
		if (this.instanceTraits != null)
		{
			this.instanceTraits.HandleDrawGizmo();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (this.instanceTraits != null)
		{
			this.instanceTraits.HandleDrawGizmoSelected();
		}
	}

	public GameObject Prefab
	{
		get
		{
			return this.prefab;
		}
		set
		{
			this.prefab = value;
			this.CreateInstance();
		}
	}

	public UnityEngine.Object GetReference(string name)
	{
		if (this.referenceNames == null)
		{
			return null;
		}
		int num = this.referenceNames.IndexOf(name);
		if (num >= 0)
		{
			return this.references[num];
		}
		return null;
	}

	private static Dictionary<int, List<Instantiator.ComponentPropertyInfo>> propertyInfosCache;

	protected GameObject instance;

	protected InstantiatorTraits instanceTraits;

	[SerializeField]
	[HideInInspector]
	protected GameObject prefab;

	[SerializeField]
	[HideInInspector]
	protected List<UnityEngine.Object> references;

	[SerializeField]
	[HideInInspector]
	protected List<string> referenceNames;

	[SerializeField]
	//[HideInInspector]
	protected string jsonString;

	[SerializeField]
	[HideInInspector]
	protected List<string> doNotLocalizeProperties = new List<string>();

	public class SerializeProperty : Attribute
	{
		public SerializeProperty()
		{
			this.DefaultValue = null;
		}

		public SerializeProperty(object defaultValue)
		{
			this.DefaultValue = defaultValue;
		}

		public object DefaultValue { get; private set; }
	}

	public class SerializeLocalizableProperty : Instantiator.SerializeProperty
	{
		public SerializeLocalizableProperty()
		{
		}

		public SerializeLocalizableProperty(object defaultValue) : base(defaultValue)
		{
		}
	}

	public class ReadOnlyProperty : Instantiator.SerializeProperty
	{
	}

	public class ComponentPropertyInfo
	{
		public PropertyInfo propertyInfo;

		public MonoBehaviour component;

		public Instantiator.SerializeProperty attribute;
	}

	public struct MethodReference
	{
		public static bool operator ==(Instantiator.MethodReference a, Instantiator.MethodReference b)
		{
			return a.receiver == b.receiver && a.message == b.message;
		}

		public static bool operator !=(Instantiator.MethodReference a, Instantiator.MethodReference b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public GameObject receiver;

		public string message;
	}
}
