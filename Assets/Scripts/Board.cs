using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Board : IBoardTileAccess
{
    protected SpawnPool SpawnPool { get; private set; }

    public Transform Root { get; private set; }

    public float TileSize { get; protected set; }

    public abstract ITopology Topology { get; }

    protected abstract Vector3 TileOffset { get; }

    Piece IBoardTileAccess.GetPieceAtTileIndex(int index)
    {
        if (!this.elementsAtIndex.ContainsKey(index))
        {
            return null;
        }
        if (this.elementsAtIndex[index].Count == 0)
        {
            return null;
        }
        return this.elementsAtIndex[index][0];
    }

    public bool IsTileOccupied(int index)
    {
        return this.elementsAtIndex.ContainsKey(index) && this.elementsAtIndex[index].Count != 0;
    }

    public virtual bool TileIndexDisabled(int index)
    {
        return false;
    }

    public Vector3 GetTilePosition(int index)
    {
        return ((Vector3)this.Topology.GetPositionFromTile(index) + this.TileOffset) * this.TileSize;
    }

    public int GetTileAt(Vector2 pos)
    {
        Vector2 pos2 = pos / this.TileSize - (Vector2)this.TileOffset;
        return ((BubbleTopology)this.Topology).GetTileFromPosition(pos2);
    }

    List<Piece> IBoardTileAccess.GetPiecesAtTileIndex(int index)
    {
        if (!this.elementsAtIndex.ContainsKey(index))
        {
            return null;
        }
        return this.elementsAtIndex[index];
    }

    void IBoardTileAccess.SetPieceTileIndex(Piece piece, int newIndex)
    {
        if (piece.TileIndex >= 0)
        {
            this.DetachFromTile(piece, piece.TileIndex);
        }
        if (newIndex >= 0)
        {
            this.AttachToTile(piece, newIndex);
        }
    }

    public void InitializePool(Transform parent)
    {
        this.Root = new GameObject("Board").transform;
        this.Root.parent = parent;
        this.Root.localPosition = Vector3.zero;
        this.Root.gameObject.layer = parent.gameObject.layer;
        GameObject gameObject = new GameObject("PiecesPool");
        gameObject.transform.parent = this.Root;
        gameObject.transform.localPosition = Vector3.back;
        this.SpawnPool = PoolManager.Pools.Create("pieces", gameObject);
        foreach (PieceInfo pieceInfo in SingletonAsset<PieceDatabase>.Instance.AllPieces)
        {
            Piece gamePrefab = pieceInfo.gamePrefab;
            if (!(gamePrefab == null) && gamePrefab.prewarmAmount != 0)
            {
                PrefabPool prefabPool = new PrefabPool(pieceInfo.gamePrefab.transform);
                prefabPool.preloadAmount = gamePrefab.prewarmAmount;
                this.SpawnPool.CreatePrefabPool(prefabPool);
            }
        }
        Piece.SetStaticBoard(this);
    }

    public Tile GetTile(int index)
    {
        return new Tile(this, index);
    }

    public IEnumerable<Tile> FindAllWithPieceClass<T>()
    {
        foreach (Tile tile in this.GetOccupiedTiles())
        {
            if (tile.Piece != null && tile.Piece is T)
            {
                yield return tile;
            }
        }
        yield break;
    }

    public int CountAllPiecesWithClass<T>()
    {
        int num = 0;
        foreach (int index in this.GetOccupiedTilesIndexes())
        {
            Tile tile = this.GetTile(index);
            if (tile.Piece != null && tile.Piece is T)
            {
                num++;
            }
        }
        return num;
    }

    public abstract void BuildLevel();

    public Piece SpawnPiece(PieceId type)
    {
        if (type.IsEmpty)
        {
            return null;
        }
        PieceInfo piece = SingletonAsset<PieceDatabase>.Instance.GetPiece(type);
        return this.SpawnPiece(piece);
    }

    private Piece SpawnPiece(PieceInfo pieceInfo)
    {
        Piece component = this.SpawnPool.Spawn(pieceInfo.gamePrefab.transform).GetComponent<Piece>();
        component.gameObject.SetLayerRecursively(this.Root.gameObject.layer);
        component.transform.localScale = Vector3.one;
        component.SpawnedByBoard(this);
        component.name = string.Format("type={0}", pieceInfo.id);
        return component;
    }

    private void AttachToTile(Piece e, int tileIndex)
    {
        if (!this.elementsAtIndex.ContainsKey(tileIndex))
        {
            this.elementsAtIndex[tileIndex] = new List<Piece>();
        }
        if (this.elementsAtIndex[tileIndex].Contains(e))
        {
        }
        this.elementsAtIndex[tileIndex].Add(e);
        List<Piece> list = this.elementsAtIndex[tileIndex];
      
        list.Sort(new Comparison<Piece>(Board.SortByLayer));
    }

    private void DetachFromTile(Piece e, int tileIndex)
    {
        this.elementsAtIndex[tileIndex].Remove(e);
        List<Piece> list = this.elementsAtIndex[tileIndex];
       
        list.Sort(new Comparison<Piece>(Board.SortByLayer));
        if (this.elementsAtIndex[tileIndex].Count == 0)
        {
            this.elementsAtIndex.Remove(tileIndex);
        }
    }

    public Piece SpawnPieceAt(int atTileIndex, PieceId type)
    {
        if (atTileIndex < 0)
        {
            return null;
        }
        Tile tile = new Tile(this, atTileIndex);
        if (tile.Disabled)
        {
            return null;
        }
        PieceInfo piece = SingletonAsset<PieceDatabase>.Instance.GetPiece(type);
        foreach (Piece piece2 in tile.Pieces)
        {
            if (piece2.TileLayer == piece.gamePrefab.TileLayer)
            {
                return null;
            }
        }
        Piece piece3 = this.SpawnPiece(piece);
        piece3.TileIndex = atTileIndex;
        piece3.AlignToTile();
        Coord coord = tile.Coord;
        piece3.name = string.Format("type={2} @ [{0},{1}]", coord.x, coord.y, type);
        return piece3;
    }

    public void DespawnPieceAt(Tile tile)
    {
        Piece piece = tile.Piece;
        if (piece == null)
        {
            return;
        }
        this.DetachFromTile(piece, tile.Index);
        this.SpawnPool.Despawn(piece.transform);
    }

    public void DespawnPiece(Piece piece)
    {
        this.RemovePieceWithoutDespawn(piece);
        this.SpawnPool.Despawn(piece.transform);
    }

    public void RemovePieceWithoutDespawn(Piece piece)
    {
        piece.TileIndex = -1;
    }

    public IEnumerable<Tile> GetOccupiedTiles()
    {
        foreach (KeyValuePair<int, List<Piece>> pair in this.elementsAtIndex)
        {
            Tile t = new Tile(this, pair.Key);
            yield return t;
        }
        yield break;
    }

    public IEnumerable<int> GetOccupiedTilesIndexes()
    {
        foreach (KeyValuePair<int, List<Piece>> pair in this.elementsAtIndex)
        {
            yield return pair.Key;
        }
        yield break;
    }

    public Tile GetClosest(Vector2 localPos, float radius, Func<Tile, bool> filter)
    {
        Tile result = Tile.Invalid;
        float num = float.MaxValue;
        for (int i = 0; i < this.Topology.Count; i++)
        {
            Tile tile = new Tile(this, i);
            if (!tile.Disabled && (filter == null || filter(tile)))
            {
                float magnitude = (tile.LocalPosition - (Vector3)localPos).magnitude;
                if (magnitude < radius && magnitude < num)
                {
                    num = magnitude;
                    result = tile;
                }
            }
        }
        return result;
    }

    private static int SortByLayer(Piece a, Piece b)
    {
        if (a.TileLayer > b.TileLayer)
        {
            return -1;
        }
        if (a.TileLayer < b.TileLayer)
        {
            return 1;
        }
        return 0;
    }

    protected readonly Dictionary<int, List<Piece>> elementsAtIndex = new Dictionary<int, List<Piece>>();

    
}
