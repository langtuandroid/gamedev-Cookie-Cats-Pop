using System;

public class AchievementAssetPiece : AchievementAsset
{
	public override bool AllowProgress(GameEvent e)
	{
		Piece piece = e.context as Piece;
		return !(piece == null) && piece.TypeId.TypeName == this.pieceType.TypeName;
	}

	public PieceId pieceType;
}
