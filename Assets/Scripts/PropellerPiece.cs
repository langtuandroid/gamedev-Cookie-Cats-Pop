using System;

public class PropellerPiece : CPPiece
{
	public override bool IsAttachment
	{
		get
		{
			return true;
		}
	}

	public override int TileLayer
	{
		get
		{
			return 1;
		}
	}
}
