using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public struct PieceId
{
	public PieceId(string pieceTypeName, string matchFlag)
	{
		this.typeName = pieceTypeName;
		this.matchFlag = matchFlag;
	}

	public static PieceId Create<T>(MatchFlag matchFlag)
	{
		return new PieceId(typeof(T).Name, matchFlag);
	}

	public static bool operator ==(PieceId a, PieceId b)
	{
		return a.typeName == b.typeName && a.matchFlag == b.matchFlag;
	}

	public static bool operator !=(PieceId a, PieceId b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj is PieceId)
		{
			PieceId pieceId = (PieceId)obj;
			return this.typeName == pieceId.typeName && this.matchFlag == pieceId.matchFlag;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return this.typeName.GetHashCode() ^ this.matchFlag.GetHashCode();
	}

	public override string ToString()
	{
		if (!this.IsEmpty)
		{
			string str = (!(this.matchFlag != string.Empty)) ? string.Empty : (":" + this.matchFlag.ToString());
			return this.typeName + str;
		}
		return string.Empty;
	}

	public static PieceId FromString(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return PieceId.Empty;
		}
		string[] array = s.Split(new char[]
		{
			':'
		});
		MatchFlag t = string.Empty;
		if (array.Length > 1)
		{
			t = array[1];
		}
		string pieceTypeName = array[0];
		return new PieceId(pieceTypeName, t);
	}

	public Type SystemType
	{
		get
		{
			return PieceId.GetTypeInAssembly(this.typeName);
		}
	}

	public string TypeName
	{
		get
		{
			return this.typeName;
		}
	}

	public MatchFlag MatchFlag
	{
		get
		{
			return this.matchFlag;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return string.IsNullOrEmpty(this.TypeName);
		}
	}

	private static Type GetTypeInAssembly(string typeName)
	{
		if (PieceId.cachedAssembly == null)
		{
			PieceId.cachedAssembly = Assembly.GetAssembly(typeof(Piece));
		}
		if (string.IsNullOrEmpty(typeName))
		{
			return null;
		}
		return PieceId.cachedAssembly.GetType(typeName);
	}

	private static Assembly cachedAssembly;

	[SerializeField]
	[Hashable(null)]
	private string typeName;

	[SerializeField]
	[Hashable(null)]
	private MatchFlag matchFlag;

	public static readonly PieceId Empty = default(PieceId);
}
