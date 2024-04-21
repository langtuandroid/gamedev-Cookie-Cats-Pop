using System;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelCollection
{
	int NumberOfAvailableLevels { get; }

	int GetHumanNumber(LevelProxy levelProxy);

	List<LevelStub> LevelStubs { get; }

	AssetBundle AssetBundle { get; set; }

	bool ValidateLevelIndex(int index);

	string GetAnalyticsDescriptor();
}
