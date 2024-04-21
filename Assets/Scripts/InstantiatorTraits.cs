using System;
using UnityEngine;

public class InstantiatorTraits : MonoBehaviour
{
	public virtual void HandleDrawGizmo()
	{
		Vector3 one = Vector3.one;
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			one.x = component.bounds.size.x + 0.1f;
			one.y = component.bounds.size.y + 0.1f;
		}
		Bounds bounds = new Bounds(Vector3.zero, one);
		Gizmos.color = new Color(0f, 0f, 0f, 0f);
		Gizmos.DrawCube(base.transform.position, bounds.size);
	}

	public virtual void HandleDrawGizmoSelected()
	{
	}

	public Instantiator Instantiator
	{
		get
		{
			return this.instantiator;
		}
		set
		{
			this.instantiator = value;
		}
	}

	private Instantiator instantiator;
}
