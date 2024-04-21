using System;

namespace SQLite4Unity3d
{
	[Flags]
	public enum SQLiteOpenFlags
	{
		ReadOnly = 1,
		ReadWrite = 2,
		Create = 4,
		NoMutex = 32768,
		FullMutex = 65536,
		SharedCache = 131072,
		PrivateCache = 262144,
		ProtectionComplete = 1048576,
		ProtectionCompleteUnlessOpen = 2097152,
		ProtectionCompleteUntilFirstUserAuthentication = 3145728,
		ProtectionNone = 4194304
	}
}
