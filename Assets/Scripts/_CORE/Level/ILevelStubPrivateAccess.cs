using System;

public interface ILevelStubPrivateAccess
{
	LevelMetaData MetaData { get; set; }

	string MetaClassType { get; set; }

	string MetaDataAsJson { get; set; }

	string AssetResourcePath { get; set; }
}
