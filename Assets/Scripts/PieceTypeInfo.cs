using System;
using System.Collections.Generic;

[Serializable]
public class PieceTypeInfo
{
	public bool IsColorValid(MatchFlag flag)
	{
		foreach (PieceInfo pieceInfo in this.infoPerColor)
		{
			if (pieceInfo.id.MatchFlag == flag)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsRotatable
	{
		get
		{
			return this.infoPerColor[0].gamePrefab.IsRotatable;
		}
	}

	public bool IsAttachment
	{
		get
		{
			return this.infoPerColor[0].gamePrefab.IsAttachment;
		}
	}

	public string typeName;

	public bool hasMultipleColors;

	public List<PieceInfo> infoPerColor = new List<PieceInfo>();
}
