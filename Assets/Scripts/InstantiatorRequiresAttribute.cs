using System;
using UnityEngine;

public class InstantiatorRequiresAttribute : PropertyAttribute
{
	public InstantiatorRequiresAttribute(Type requiredType)
	{
		this.RequiredType = requiredType;
	}

	public Type RequiredType { get; private set; }
}
