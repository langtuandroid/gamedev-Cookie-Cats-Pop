using System;

public class SuperAimPieceHighlighter
{
	public SuperAimPieceHighlighter(LevelSession levelSession)
	{
		this.levelSession = levelSession;
		this.levelSession.Cannon.AimingStarted += this.AimingStarted;
	}

	private GameBoard Board
	{
		get
		{
			return this.levelSession.TurnLogic.Board;
		}
	}

	public void StopSuperAim()
	{
		this.levelSession.Cannon.AimingStarted -= this.AimingStarted;
	}

	private void AimingStarted(AimingState aimState)
	{
		MatchFlag matchFlag = this.levelSession.GetNextPieceToShoot().MatchFlag;
		if (matchFlag == string.Empty)
		{
			return;
		}
		foreach (Tile tile in this.Board.GetOccupiedTiles())
		{
			foreach (Tile.PieceWithComponent<PieceVisuals> pieceWithComponent in tile.PiecesWithComponent<PieceVisuals>())
			{
				pieceWithComponent.component.Highlighted = (pieceWithComponent.piece.MatchFlag == matchFlag);
			}
		}
		this.levelSession.Cannon.AimingEnded -= this.AimingEnded;
		this.levelSession.Cannon.AimingEnded += this.AimingEnded;
	}

	private void AimingEnded(AimingState aimState)
	{
		foreach (Tile tile in this.Board.GetOccupiedTiles())
		{
			foreach (Tile.PieceWithComponent<PieceVisuals> pieceWithComponent in tile.PiecesWithComponent<PieceVisuals>())
			{
				pieceWithComponent.component.Highlighted = true;
			}
		}
		this.levelSession.Cannon.AimingEnded -= this.AimingEnded;
	}

	private LevelSession levelSession;
}
