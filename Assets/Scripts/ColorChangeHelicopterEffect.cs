using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class ColorChangeHelicopterEffect : SpawnedEffect
{
    protected override IEnumerator AnimationLogic(object[] parameters)
    {
        this.session = (LevelSession)parameters[0];
        int numberOfRows = ConfigurationManager.Get<BoosterConfig>().CustomLimitedBoosterConfig.PaintbrushRows;
        Dictionary<MatchFlag, List<Piece>> colorPieceDict = this.ConstructColorPieceDictionary(numberOfRows);
        List<List<Piece>> colorPieceListList = this.ConvertDictionaryToListAndSort(colorPieceDict);
        List<MatchFlag> fromColors = this.FindPotentialColorsToChooseFrom();
        List<Piece> piecesToColor = this.FindPiecesToColor(fromColors, colorPieceListList);
        MatchFlag fromColor = piecesToColor[0].MatchFlag;
        MatchFlag toColor = this.FindToColor(fromColor);
        yield return this.AnimateHelicopter(piecesToColor, toColor);
        yield break;
    }

    private Dictionary<MatchFlag, List<Piece>> ConstructColorPieceDictionary(int numberOfRows)
    {
        List<Piece> allPiecesOnBoard = this.session.TurnLogic.Board.GetAllPiecesOnBoard();
        Dictionary<MatchFlag, List<Piece>> dictionary = new Dictionary<MatchFlag, List<Piece>>();
        foreach (Piece piece in allPiecesOnBoard)
        {
            if (piece.MatchFlag != string.Empty && this.session.TurnLogic.Board.IsPieceBelowRow(piece, numberOfRows))
            {
                List<Piece> list;
                if (!dictionary.ContainsKey(piece.MatchFlag))
                {
                    list = new List<Piece>();
                    dictionary.Add(piece.MatchFlag, list);
                }
                else
                {
                    list = dictionary[piece.MatchFlag];
                }
                list.Add(piece);
            }
        }
        return dictionary;
    }

    private List<List<Piece>> ConvertDictionaryToListAndSort(Dictionary<MatchFlag, List<Piece>> colorPieceDict)
    {
        List<List<Piece>> list = new List<List<Piece>>();
        foreach (KeyValuePair<MatchFlag, List<Piece>> keyValuePair in colorPieceDict)
        {
            list.Add(keyValuePair.Value);
        }
        list.Sort((List<Piece> x, List<Piece> y) => x.Count.CompareTo(y.Count));
        list.Reverse();
        return list;
    }

    private List<MatchFlag> FindPotentialColorsToChooseFrom()
    {
        LevelAsset levelAsset = this.session.Level.LevelAsset as LevelAsset;
        List<MatchFlag> list = new List<MatchFlag>();
        foreach (PuzzleLevel.SpawnInfo spawnInfo in levelAsset.spawnInfos)
        {
            list.Add(spawnInfo.id.MatchFlag);
        }
        List<MatchFlag> list2 = new List<MatchFlag>(list.ToArray());
        foreach (MatchFlag item in this.session.AdjustedEnabledPowerupColors)
        {
            list2.Remove(item);
        }
        return list2;
    }

    private List<Piece> FindPiecesToColor(List<MatchFlag> fromColors, List<List<Piece>> colorPieceListList)
    {
        List<Piece> result = colorPieceListList[0];
        foreach (List<Piece> list in colorPieceListList)
        {
            MatchFlag matchFlag = list[0].MatchFlag;
            if (fromColors.Contains(matchFlag))
            {
                result = list;
            }
        }
        return result;
    }

    private MatchFlag FindToColor(MatchFlag fromColor)
    {
        List<MatchFlag> list = new List<MatchFlag>(this.session.AdjustedEnabledPowerupColors);
        list.Remove(fromColor);
        MatchFlag random = list.GetRandom<MatchFlag>();
        if (list.Count == 0)
        {
            List<MatchFlag> list2 = this.FindPotentialColorsToChooseFrom();
            list2.Remove(fromColor);
            return list2[0];
        }
        return random;
    }

    private IEnumerator AnimateHelicopter(List<Piece> colorPieces, MatchFlag toColor)
    {
        this.SetColorOfBrushEffect(toColor);
        Vector3 initialPos = base.transform.position;
        base.GetComponentInChildren<SkeletonAnimation>().AnimationName = toColor.ToString();
        base.transform.parent = this.session.TurnLogic.Board.Root;
        if (colorPieces.Count > 0)
        {
            colorPieces.Sort((Piece a, Piece b) => a.transform.localPosition.y.CompareTo(b.transform.localPosition.y));
            List<Vector2> points = new List<Vector2>();
            foreach (Piece piece in colorPieces)
            {
                points.Add(piece.transform.localPosition);
            }
            Vector3 initialLocalPos = base.transform.localPosition;
            yield return FiberAnimation.MoveLocalTransform(base.transform, initialLocalPos, new Vector3(points[0].x, points[0].y, initialLocalPos.z), this.helicopterCatMoveInOutCurve, 1f);
            Spline2 spline = new Spline2();
            spline.Nodes = points;
            spline.CalculateSpline();
            int currentCurveIndex = -1;
            float time = Mathf.Clamp((float)points.Count * 0.2f, 2.5f, 5f);
            yield return FiberAnimation.Animate(time, this.helicopterCatPaintCurve, delegate (float t)
            {
                Spline2.SplineInfo splineInfo = spline.EvaluateWithInfo(t * spline.FullLength);
                Piece piece2 = colorPieces[splineInfo.curveIndex];
                this.transform.localPosition = new Vector3(splineInfo.position.x, splineInfo.position.y, piece2.transform.localPosition.z - 10f);
                if (splineInfo.curveIndex != currentCurveIndex)
                {
                    for (int i = currentCurveIndex; i < splineInfo.curveIndex; i++)
                    {
                        piece2 = colorPieces[i + 1];
                        this.PaintTile(piece2, toColor);
                    }
                    currentCurveIndex = splineInfo.curveIndex;
                }
            }, false);
            this.PaintTile(colorPieces[colorPieces.Count - 1], toColor);
            this.drops.Stop();
            Vector3 srcPosEnd = base.transform.position;
            yield return FiberAnimation.MoveTransform(base.transform, srcPosEnd, new Vector3(initialPos.x, initialPos.y, srcPosEnd.z), this.helicopterCatMoveInOutCurve, 1f);
            base.transform.parent = base.transform;
        }
        yield break;
    }

    private void SetColorOfBrushEffect(MatchFlag toColor)
    {
        foreach (ColorChangeHelicopterEffect.ColorData colorData in this.dropColors)
        {
            if (colorData.colorName == toColor)
            {
                var temp = this.drops.main;

                temp.startColor = colorData.color;
            }
        }
    }

    private void PaintTile(Piece piece, MatchFlag toColor)
    {
        PieceId type = new PieceId(piece.GetType().Name, toColor);
        int tileIndex = piece.TileIndex;
        this.session.TurnLogic.Board.DespawnPiece(piece);
        Piece newPiece = this.session.TurnLogic.Board.SpawnPieceAt(tileIndex, type);
        FiberCtrl.Pool.Run(this.PaintTileEffect(newPiece, toColor), false);
    }

    private IEnumerator PaintTileEffect(Piece newPiece, MatchFlag color)
    {
        ParticleEffect particle = ManagerRepository.Get<EffectPool>().SpawnEffect("ColorChangePaintEffect", newPiece.transform.position, newPiece.gameObject.layer, new object[0]) as ParticleEffect;
        ParticleSystem[] particleSystems = particle.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            foreach (ColorChangeHelicopterEffect.ColorData colorData in this.dropColors)
            {
                if (colorData.colorName == color)
                {
                    main.startColor = colorData.color;
                }
            }
        }
        yield return FiberAnimation.ScaleTransform(newPiece.transform, Vector3.zero, Vector3.one, this.helicopterCatMoveInOutCurve, 0.1f);
        yield break;
    }

    [SerializeField]
    private AnimationCurve helicopterCatMoveInOutCurve;

    [SerializeField]
    private AnimationCurve helicopterCatPaintCurve;

    [SerializeField]
    private ColorChangeHelicopterEffect.ColorData[] dropColors;

    [SerializeField]
    private ParticleSystem drops;

    private LevelSession session;

    [Serializable]
    public class ColorData
    {
        public MatchFlag colorName;

        public Color color;
    }
}
