using System;

public interface ILevelDatabase : ILevelCollection
{
	string GetPersistedKey(LevelProxy levelProxy);

	void Save();

	ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy);

	void RemoveLevelData(LevelProxy levelProxy);

	LevelProxy GetLevel(int levelIndex);
}
