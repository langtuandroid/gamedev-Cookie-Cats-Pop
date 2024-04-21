using System;

public interface IHardLevelsProvider
{
	LevelDatabase GetMainLevelDatabase();

	int GetHumanNumber(int index);
}
