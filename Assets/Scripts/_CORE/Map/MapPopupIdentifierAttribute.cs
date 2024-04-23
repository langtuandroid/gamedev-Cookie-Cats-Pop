using System;

public class MapPopupIdentifierAttribute : Attribute
{
	public MapPopupIdentifierAttribute(string identifier)
	{
		this.Identifier = identifier;
	}

	public string Identifier { get; private set; }
}
