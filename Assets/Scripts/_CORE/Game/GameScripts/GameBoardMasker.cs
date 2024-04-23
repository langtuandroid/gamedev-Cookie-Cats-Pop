using System;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class GameBoardMasker : MonoBehaviour
{
	public static void Begin(GameBoard board)
	{
		GameView view = UIViewManager.Instance.FindView<GameView>();
		MaskOverlay.Instance.SetDepthAboveView(view);
		MaskOverlay.Instance.Enable(null);
		GameBoardMasker.cam = MaskOverlayExtensions.FindCameraFromGO(board.Root.gameObject);
	}

	public static void HighlightTiles(List<int> tileIndices, GameBoard board)
	{
		foreach (int index in tileIndices)
		{
			Tile tile = board.GetTile(index);
			MaskOverlayCutout maskOverlayCutout = MaskOverlay.Instance.AddCutout(null);
			maskOverlayCutout.SetFromWorldSpace(GameBoardMasker.cam, tile.WorldPosition, Vector2.one * 90f);
			maskOverlayCutout.Oval = true;
		}
	}

	public static void HighlightTarget(Tile t)
	{
		MaskOverlayCutout maskOverlayCutout = MaskOverlay.Instance.AddCutout(null);
		maskOverlayCutout.SetFromWorldSpace(GameBoardMasker.cam, t.WorldPosition, Vector2.one * 250f);
		maskOverlayCutout.Oval = true;
	}

	public static void HighlightShooter()
	{
		GameView gameView = UIViewManager.Instance.FindView<GameView>();
		MaskOverlayCutout cutout = MaskOverlay.Instance.AddCutout(null);
		cutout.SetOvalFromGameObject(gameView.cannon.muzzlePivot.gameObject, Vector2.one * 250f);
	}

	public static void HighlightPos(Vector3 wp, float size)
	{
		MaskOverlayCutout maskOverlayCutout = MaskOverlay.Instance.AddCutout(null);
		maskOverlayCutout.SetFromWorldSpace(GameBoardMasker.cam, wp, Vector2.one * size);
		maskOverlayCutout.Oval = true;
	}

	public static void End()
	{
		MaskOverlay.Instance.Disable(null);
		MaskOverlay.Instance.Clear();
	}

	private static Camera cam;
}
