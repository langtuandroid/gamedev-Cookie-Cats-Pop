using System;
using System.Collections;
using UnityEngine;

public class XperiaGamesClubManager
{
	public XperiaGamesClubManager()
	{
		this.IsXperiaGamesClubBuild = false;
		FiberCtrl.Pool.Run(this.UpdateIsXperiaGamesClubBuild(), false);
	}

	private IEnumerator UpdateIsXperiaGamesClubBuild()
	{
		using (WWW www = new WWW("jar:file://" + Application.dataPath + "!/assets/adjust_click"))
		{
			yield return www;
			if (www.error == null && !string.IsNullOrEmpty(www.text))
			{
				this.IsXperiaGamesClubBuild = true;
			}
		}
		yield break;
	}

	public bool IsXperiaGamesClubBuild { get; private set; }

	public bool IsSonyDevice
	{
		get
		{
			return SystemInfoHelper.Manufacturer != null && SystemInfoHelper.Manufacturer.IndexOf("sony", StringComparison.InvariantCultureIgnoreCase) != -1;
		}
	}
}
