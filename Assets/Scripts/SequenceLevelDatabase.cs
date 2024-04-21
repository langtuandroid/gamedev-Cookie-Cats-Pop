using System;

public abstract class SequenceLevelDatabase : LevelDatabase
{
	public abstract int GetSequenceIndex();

	public int SequenceIndexModulo
	{
		get
		{
			return this.GetSequenceIndex() % base.LevelStubs.Count;
		}
	}

	public sealed override LevelProxy GetLevel(int levelIndex)
	{
		int sequenceIndexModulo = this.SequenceIndexModulo;
		return (sequenceIndexModulo != -1) ? new LevelProxy(this, new int[]
		{
			sequenceIndexModulo
		}).CreateChildProxy(levelIndex) : LevelProxy.Invalid;
	}

	public sealed override int NumberOfAvailableLevels
	{
		get
		{
			return (new LevelProxy(this, new int[1]).LevelAsset as ILevelCollection).NumberOfAvailableLevels;
		}
	}
}
