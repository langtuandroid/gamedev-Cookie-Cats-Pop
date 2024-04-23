using System;

public class BossCheckpointPiece : CPPiece, IComparable
{
	public int CheckpointIdx { get; set; }

	public int CompareTo(object obj)
	{
		BossCheckpointPiece bossCheckpointPiece = obj as BossCheckpointPiece;
		return this.CheckpointIdx.CompareTo(bossCheckpointPiece.CheckpointIdx);
	}
}
