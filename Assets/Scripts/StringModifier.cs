using System;
using System.Runtime.InteropServices;
using System.Text;

public static class StringModifier
{
	public unsafe static void CopyFromStringBuilder(ref string str, StringBuilder stringBuilder)
	{
		GCHandle gchandle = default(GCHandle);
		try
		{
			int num = Math.Min(stringBuilder.Length, 2048);
			gchandle = GCHandle.Alloc(str, GCHandleType.Pinned);
			char* ptr = (char*)((void*)gchandle.AddrOfPinnedObject());
			for (int i = 0; i < num; i++)
			{
				ptr[i] = stringBuilder[i];
			}
			ptr[num] = '\0';
		}
		finally
		{
			try
			{
				gchandle.Free();
			}
			catch (InvalidOperationException)
			{
			}
		}
	}

	public const int STRING_MAX_SIZE = 2048;
}
