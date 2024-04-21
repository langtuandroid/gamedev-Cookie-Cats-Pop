using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTestBoot : Boot
{
	protected override void BootCompleted()
	{
		FiberCtrl.Pool.Run(this.StartCr(), false);
	}

	private IEnumerator StartCr()
	{
		yield return new WaitForSeconds(0.1f);
		this.band.ConfigureSingers(false, this.lineup);
		if (this.startWithTrack)
		{
			this.band.SetMultiTrack(this.startWithTrack);
			this.band.StartSinging(true);
		}
		yield break;
	}

	public SingingBand band;

	public MultiTrack startWithTrack;

	public List<string> lineup = new List<string>
	{
		"red",
		"green",
		"blue",
		"yellow"
	};
}
