using System;

namespace TactileModules.GameCore.StreamingAssets
{
	[AttributeUsage(AttributeTargets.Field)]
	public class StreamingAssetGroup : Attribute
	{
		public StreamingAssetGroup(string name)
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}
}
