using System;
using UnityEngine;

[RequireComponent(typeof(ZSorter))]
public abstract class Piece : MonoBehaviour
{
    protected static Board StaticBoard { get; private set; }

    public int Direction
    {
        get
        {
            return this.direction;
        }
        set
        {
            this.direction = value;
            this.transform.localRotation = Quaternion.Euler(0f, 0f, (float)this.direction);
        }
    }

    public PieceId TypeId
    {
        get
        {
            return new PieceId(base.GetType().Name, this.MatchFlag);
        }
    }

    protected Board Board
    {
        get
        {
            return Piece.StaticBoard;
        }
    }

    public virtual int TileLayer
    {
        get
        {
            return 0;
        }
    }

    public virtual int NumberOfVariants
    {
        get
        {
            return 1;
        }
    }

    public virtual MatchFlag MatchFlag
    {
        get
        {
            return string.Empty;
        }
    }

    public virtual bool IsBasicPiece
    {
        get
        {
            return false;
        }
    }

    public virtual bool IsAttachment
    {
        get
        {
            return false;
        }
    }

    public virtual bool IsRotatable
    {
        get
        {
            return false;
        }
    }

    public virtual void SpawnedByBoard(Board board)
    {
    }

    public virtual void AfterBoardSetup(Board board)
    {
    }

    public new Transform transform
    {
        get
        {
            if (this.cachedTransform == null)
            {
                this.cachedTransform = base.gameObject.transform;
            }
            return this.cachedTransform;
        }
    }

    protected virtual void OnSpawned()
    {
        this.tileIndex = -1;
    }

    public virtual void DetachFromBoard()
    {
        this.TileIndex = -1;
    }

    public int TileIndex
    {
        get
        {
            return this.tileIndex;
        }
        set
        {
            ((IBoardTileAccess)Piece.StaticBoard).SetPieceTileIndex(this, value);
            this.tileIndex = value;
        }
    }

    public static void SetStaticBoard(Board b)
    {
        Piece.StaticBoard = b;
    }

    public virtual void AlignToTile()
    {
        if (this.TileIndex >= 0)
        {
            Tile tile = new Tile(Piece.StaticBoard, this.tileIndex);
            this.transform.localPosition = tile.LocalPosition;
            this.transform.localRotation = Quaternion.Euler(0f, 0f, (float)this.Direction);
            base.gameObject.SetLayerRecursively(Piece.StaticBoard.Root.gameObject.layer);
            this.ZSorter().layer = ZLayer.BoardPiece + this.TileLayer * 50;
        }
    }

    private int tileIndex = -1;

    private Transform cachedTransform;

    private int direction;

    public int prewarmAmount;
}
