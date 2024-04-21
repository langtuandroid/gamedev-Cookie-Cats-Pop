using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
public class ObsoleteJsonNameAttribute : Attribute
{
	public ObsoleteJsonNameAttribute(string jsonName)
	{
		this.jsonName = new string[]
		{
			jsonName
		};
	}

	public ObsoleteJsonNameAttribute(List<string> jsonNames)
	{
		this.jsonName = jsonNames.ToArray();
	}

	public ObsoleteJsonNameAttribute(params string[] jsonNames)
	{
		this.jsonName = jsonNames;
	}

	public string[] jsonName { get; private set; }
}
