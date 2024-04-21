using System;

public class SingletonAssetPath : Attribute
{
	public SingletonAssetPath(string path)
	{
		this.path = path;
	}

	public string path;
}
