using System;

[Serializable]
public class PieceInfo
{
	public bool RequirePrefab
	{
		get
		{
			return !SingletonAsset<PieceDatabase>.Instance.MatchFlagIsTokenOnly(this.id.MatchFlag);
		}
	}

	public PieceId id;

	public Piece gamePrefab;
}
