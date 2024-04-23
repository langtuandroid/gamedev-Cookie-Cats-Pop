using System;
using System.Collections.Generic;
using UnityEngine;

[SingletonAssetPath("Assets/[Database]/Resources/PieceDatabase.asset")]
public class PieceDatabase : SingletonAsset<PieceDatabase>
{
	public IEnumerable<PieceInfo> AllPieces
	{
		get
		{
			foreach (PieceTypeInfo t in this.pieceTypeInfos)
			{
				foreach (PieceInfo i in t.infoPerColor)
				{
					yield return i;
				}
			}
			yield break;
		}
	}

	public IEnumerable<PieceTypeInfo> AllPieceTypes
	{
		get
		{
			return this.pieceTypeInfos;
		}
	}

	public PieceInfo GetPiece(PieceId id)
	{
		return this.GetPiece(id.TypeName, id.MatchFlag);
	}

	public PieceInfo GetPiece(string typeName, MatchFlag matchFlag)
	{
		foreach (PieceTypeInfo pieceTypeInfo in this.pieceTypeInfos)
		{
			if (!(typeName != pieceTypeInfo.typeName))
			{
				foreach (PieceInfo pieceInfo in pieceTypeInfo.infoPerColor)
				{
					if (pieceInfo.id.MatchFlag == matchFlag)
					{
						return pieceInfo;
					}
				}
			}
		}
		return null;
	}

	public PieceInfo GetPiece<T>(MatchFlag matchFlag) where T : Piece
	{
		return this.GetPiece(typeof(T).Name, matchFlag);
	}

	public PieceTypeInfo GetPieceType(string typeName)
	{
		foreach (PieceTypeInfo pieceTypeInfo in this.pieceTypeInfos)
		{
			if (pieceTypeInfo.typeName == typeName)
			{
				return pieceTypeInfo;
			}
		}
		return null;
	}

	public void RebuildFromAssets(List<Piece> assets)
	{
		this.pieceTypeInfos.Clear();
		foreach (Piece piece in assets)
		{
			Type type = piece.GetType();
			PieceTypeInfo pieceTypeInfo = this.GetPieceType(type.Name);
			if (pieceTypeInfo == null)
			{
				pieceTypeInfo = new PieceTypeInfo();
				pieceTypeInfo.typeName = type.Name;
				this.pieceTypeInfos.Add(pieceTypeInfo);
			}
			PieceInfo pieceInfo = new PieceInfo();
			pieceInfo.gamePrefab = piece.gameObject.GetComponent<Piece>();
			pieceInfo.id = new PieceId(type.Name, pieceInfo.gamePrefab.MatchFlag);
			pieceTypeInfo.hasMultipleColors = (pieceInfo.id.MatchFlag != string.Empty);
			pieceTypeInfo.infoPerColor.Add(pieceInfo);
		}
		foreach (PieceTypeInfo pieceTypeInfo2 in this.pieceTypeInfos)
		{
			if (pieceTypeInfo2.hasMultipleColors)
			{
				foreach (string matchFlag in this.specialMatchIds)
				{
					pieceTypeInfo2.infoPerColor.Add(new PieceInfo
					{
						gamePrefab = null,
						id = new PieceId(pieceTypeInfo2.typeName, matchFlag)
					});
				}
			}
		}
	}

	public bool MatchFlagIsTokenOnly(MatchFlag matchFlag)
	{
		return this.specialMatchIds.Contains(matchFlag);
	}

	[HideInInspector]
	[SerializeField]
	private List<PieceTypeInfo> pieceTypeInfos = new List<PieceTypeInfo>();

	[HideInInspector]
	public string piecesFolder = "Assets/";

	[SerializeField]
	public List<string> specialMatchIds = new List<string>();
}
