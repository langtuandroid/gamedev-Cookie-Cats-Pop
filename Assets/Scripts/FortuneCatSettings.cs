using System;
using System.Collections.Generic;

[SingletonAssetPath("Assets/[Database]/Resources/FortuneCatSettings.asset")]
public class FortuneCatSettings : SingletonAsset<FortuneCatSettings>
{
	private string GetRandomMessage()
	{
		if (this.currentPool.Count == 0)
		{
			this.currentPool = new List<string>(this.messages);
			this.currentPool.Shuffle<string>();
		}
		string result = this.currentPool[this.currentPool.Count - 1];
		this.currentPool.RemoveAt(this.currentPool.Count - 1);
		return result;
	}

	public string GetRandomMessageLocalized()
	{
		return L.Get(this.GetRandomMessage());
	}

	public List<string> messages;

	private List<string> currentPool = new List<string>();
}
