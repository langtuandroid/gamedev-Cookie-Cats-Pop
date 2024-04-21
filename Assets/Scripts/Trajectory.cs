using System;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory
{
    public static List<Vector2> CalculateTrajectoryHits(GameBoard board, Vector2 originInBoardSpace, Vector2 dir, out Tile hitTile)
    {
        hitTile = Tile.Invalid;
        Vector2 vector = (board.Size - Vector2.one * board.TileSize) * 0.5f;
        List<Vector2> list = new List<Vector2>();
        Vector2 vector2 = originInBoardSpace;
        Vector2 a = originInBoardSpace + dir * 10000f;
        Vector2 b = new Vector2(-vector.x, -vector.y * 2f - 3000f);
        Vector2 b2 = new Vector2(-vector.x, 0f);
        Vector2 b3 = new Vector2(vector.x, -vector.y * 2f - 3000f);
        Vector2 b4 = new Vector2(vector.x, 0f);
        float trimmedTop = board.GetTrimmedTop();
        Vector2 b5 = new Vector2(-vector.x, -trimmedTop);
        Vector2 b6 = new Vector2(vector.x, -trimmedTop);
        float y = originInBoardSpace.y;
        Vector2 zero = Vector2.zero;
        Vector2 zero2 = Vector2.zero;
        int num = 0;
        while (num++ < 25)
        {
            float num2 = float.MaxValue;
            Tile tile = Tile.Invalid;
            Vector2 vector3 = Vector2.zero;
            Vector2 vector4 = Vector2.zero;
            bool flag = false;
            if (Trajectory.FindTileCollision(board, vector2, a, out hitTile, out zero, out zero2))
            {
                float sqrMagnitude = (zero - vector2).sqrMagnitude;
                if (sqrMagnitude < num2)
                {
                    num2 = sqrMagnitude;
                    tile = hitTile;
                    vector3 = zero;
                    vector4 = zero2;
                    flag = (hitTile.Piece is BumperPiece);
                }
            }
            if (Intersect2D.LineSegmentsIntersect(vector2, a, b5, b6, out zero))
            {
                float sqrMagnitude2 = (zero - vector2).sqrMagnitude;
                if (sqrMagnitude2 < num2)
                {
                    num2 = sqrMagnitude2;
                    tile = board.GetClosest(zero, board.TileSize, null);
                    vector3 = zero;
                    vector4 = Vector2.down;
                    flag = false;
                }
            }
            if (Intersect2D.LineSegmentsIntersect(vector2, a, b, b2, out zero))
            {
                float sqrMagnitude3 = (zero - vector2).sqrMagnitude;
                if (sqrMagnitude3 < num2)
                {
                    num2 = sqrMagnitude3;
                    tile = Tile.Invalid;
                    vector3 = zero;
                    vector4 = Vector2.right;
                    flag = true;
                }
            }
            if (Intersect2D.LineSegmentsIntersect(vector2, a, b3, b4, out zero))
            {
                float sqrMagnitude4 = (zero - vector2).sqrMagnitude;
                if (sqrMagnitude4 < num2)
                {
                    tile = Tile.Invalid;
                    vector3 = zero;
                    vector4 = Vector2.left;
                    flag = true;
                }
            }
            if (!(vector4 != Vector2.zero))
            {
                list.Add(Trajectory.ClampToBottom(vector2, dir, y));
                break;
            }
            if (vector3.y < y)
            {
                vector3 = Trajectory.ClampToBottom(vector2, dir, y);
                list.Add(vector3);
                break;
            }
            list.Add(vector3);
            hitTile = tile;
            if (!flag)
            {
                break;
            }
            dir = Vector2.Reflect(dir, vector4);
            vector2 = vector3 + vector4;
            a = vector2 + dir * 10000f;
        }
        return list;
    }

    private static Vector2 ClampToBottom(Vector2 position, Vector2 dir, float bottom)
    {
        float num = (bottom - position.y) / dir.y;
        return new Vector2(position.x + num * dir.x, bottom);
    }

    private static bool IntersectWithPiece(CPPiece piece, Board board, Vector2 a0, Vector2 a1, out Vector2 n, out Vector2 p)
    {
        p = Vector2.zero;
        n = Vector2.zero;
        if (piece is BumperPiece)
        {
            Vector2 b = piece.GetTile().LocalPosition;
            Quaternion localRotation = piece.transform.localRotation;
            Vector2[] array = new Vector2[4];
            float num = board.TileSize * 0.5f;
            float num2 = board.TileSize * 0.5f;
            array[0] = (Vector2)(localRotation * new Vector2(-num, -num2)) + b;
            array[1] = (Vector2)(localRotation * new Vector2(num, -num2)) + b;
            array[2] = (Vector2)(localRotation * new Vector2(num, num2)) + b;
            array[3] = (Vector2)(localRotation * new Vector2(-num, num2)) + b;
            float num3 = float.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                Vector2 vector;
                if (Intersect2D.LineSegmentsIntersect(a0, a1, array[i], array[(i + 1) % 4], out vector))
                {
                    float sqrMagnitude = (vector - a0).sqrMagnitude;
                    if (sqrMagnitude < num3)
                    {
                        n = (array[i] - array[(i + 1) % 4]).GetNormal();
                        p = vector;
                        num3 = sqrMagnitude;
                    }
                }
            }
            if (num3 != 3.40282347E+38f)
            {
                return true;
            }
        }
        else if (Intersect2D.ClosestCircleLineIntersection(a0, a1, piece.GetTile().LocalPosition, board.TileSize * 0.5f + 0.1f, out p))
        {
            n = (p - (Vector2)piece.GetTile().LocalPosition).normalized;
            return true;
        }
        return false;
    }

    private static bool FindTileCollision(Board board, Vector2 a0, Vector2 a1, out Tile hitTile, out Vector2 collidePos, out Vector2 collisionNormal)
    {
        hitTile = Tile.Invalid;
        collidePos = Vector2.zero;
        collisionNormal = Vector2.zero;
        Vector2 zero = Vector2.zero;
        Vector2 zero2 = Vector2.zero;
        float num = float.MaxValue;
        foreach (Tile tile in board.GetOccupiedTiles())
        {
            if (!(tile.Piece == null))
            {
                if (Trajectory.IntersectWithPiece(tile.Piece as CPPiece, board, a0, a1, out zero2, out zero))
                {
                    float sqrMagnitude = (zero - a0).sqrMagnitude;
                    if (sqrMagnitude < num)
                    {
                        collidePos = zero;
                        collisionNormal = zero2;
                        hitTile = tile;
                        num = sqrMagnitude;
                    }
                }
            }
        }
        return hitTile != Tile.Invalid;
    }
}
