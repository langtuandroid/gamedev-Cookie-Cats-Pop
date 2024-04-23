using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("IAPConfig")]
public class IAPConfig
{
	[JsonSerializable("InAppProducts", typeof(InAppProductTactileInfo))]
	public List<InAppProductTactileInfo> InAppProducts { get; set; }
}
