using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SQLite4Unity3d
{
	public static class SQLite3
	{
		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open")]
		public static extern SQLite3.Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open_v2")]
		public static extern SQLite3.Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, IntPtr zvfs);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open_v2")]
		public static extern SQLite3.Result Open(byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open16")]
		public static extern SQLite3.Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_enable_load_extension")]
		public static extern SQLite3.Result EnableLoadExtension(IntPtr db, int onoff);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_close")]
		public static extern SQLite3.Result Close(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_initialize")]
		public static extern SQLite3.Result Initialize();

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_shutdown")]
		public static extern SQLite3.Result Shutdown();

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_config")]
		public static extern SQLite3.Result Config(SQLite3.ConfigOption option);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "sqlite3_win32_set_directory")]
		public static extern int SetDirectory(uint directoryType, string directoryPath);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_busy_timeout")]
		public static extern SQLite3.Result BusyTimeout(IntPtr db, int milliseconds);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_changes")]
		public static extern int Changes(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_prepare_v2")]
		public static extern SQLite3.Result Prepare2(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

		public static IntPtr Prepare2(IntPtr db, string query)
		{
			IntPtr result2;
			SQLite3.Result result = SQLite3.Prepare2(db, query, Encoding.UTF8.GetByteCount(query), out result2, IntPtr.Zero);
			if (result != SQLite3.Result.OK)
			{
				throw SQLiteException.New(result, SQLite3.GetErrmsg(db));
			}
			return result2;
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_step")]
		public static extern SQLite3.Result Step(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_reset")]
		public static extern SQLite3.Result Reset(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_finalize")]
		public static extern SQLite3.Result Finalize(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_last_insert_rowid")]
		public static extern long LastInsertRowid(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_errmsg16")]
		public static extern IntPtr Errmsg(IntPtr db);

		public static string GetErrmsg(IntPtr db)
		{
			return Marshal.PtrToStringUni(SQLite3.Errmsg(db));
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_parameter_index")]
		public static extern int BindParameterIndex(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_null")]
		public static extern int BindNull(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_int")]
		public static extern int BindInt(IntPtr stmt, int index, int val);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_int64")]
		public static extern int BindInt64(IntPtr stmt, int index, long val);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_double")]
		public static extern int BindDouble(IntPtr stmt, int index, double val);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "sqlite3_bind_text16")]
		public static extern int BindText(IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_blob")]
		public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_count")]
		public static extern int ColumnCount(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_name")]
		public static extern IntPtr ColumnName(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_name16")]
		private static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);

		public static string ColumnName16(IntPtr stmt, int index)
		{
			return Marshal.PtrToStringUni(SQLite3.ColumnName16Internal(stmt, index));
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_type")]
		public static extern SQLite3.ColType ColumnType(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_int")]
		public static extern int ColumnInt(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_int64")]
		public static extern long ColumnInt64(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_double")]
		public static extern double ColumnDouble(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_text")]
		public static extern IntPtr ColumnText(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_text16")]
		public static extern IntPtr ColumnText16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_blob")]
		public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_bytes")]
		public static extern int ColumnBytes(IntPtr stmt, int index);

		public static string ColumnString(IntPtr stmt, int index)
		{
			return Marshal.PtrToStringUni(SQLite3.ColumnText16(stmt, index));
		}

		public static byte[] ColumnByteArray(IntPtr stmt, int index)
		{
			int num = SQLite3.ColumnBytes(stmt, index);
			byte[] array = new byte[num];
			if (num > 0)
			{
				Marshal.Copy(SQLite3.ColumnBlob(stmt, index), array, 0, num);
			}
			return array;
		}

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_extended_errcode")]
		public static extern SQLite3.ExtendedResult ExtendedErrCode(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_libversion_number")]
		public static extern int LibVersionNumber();

		public enum Result
		{
			OK,
			Error,
			Internal,
			Perm,
			Abort,
			Busy,
			Locked,
			NoMem,
			ReadOnly,
			Interrupt,
			IOError,
			Corrupt,
			NotFound,
			Full,
			CannotOpen,
			LockErr,
			Empty,
			SchemaChngd,
			TooBig,
			Constraint,
			Mismatch,
			Misuse,
			NotImplementedLFS,
			AccessDenied,
			Format,
			Range,
			NonDBFile,
			Notice,
			Warning,
			Row = 100,
			Done
		}

		public enum ExtendedResult
		{
			IOErrorRead = 266,
			IOErrorShortRead = 522,
			IOErrorWrite = 778,
			IOErrorFsync = 1034,
			IOErrorDirFSync = 1290,
			IOErrorTruncate = 1546,
			IOErrorFStat = 1802,
			IOErrorUnlock = 2058,
			IOErrorRdlock = 2314,
			IOErrorDelete = 2570,
			IOErrorBlocked = 2826,
			IOErrorNoMem = 3082,
			IOErrorAccess = 3338,
			IOErrorCheckReservedLock = 3594,
			IOErrorLock = 3850,
			IOErrorClose = 4106,
			IOErrorDirClose = 4362,
			IOErrorSHMOpen = 4618,
			IOErrorSHMSize = 4874,
			IOErrorSHMLock = 5130,
			IOErrorSHMMap = 5386,
			IOErrorSeek = 5642,
			IOErrorDeleteNoEnt = 5898,
			IOErrorMMap = 6154,
			LockedSharedcache = 262,
			BusyRecovery = 261,
			CannottOpenNoTempDir = 270,
			CannotOpenIsDir = 526,
			CannotOpenFullPath = 782,
			CorruptVTab = 267,
			ReadonlyRecovery = 264,
			ReadonlyCannotLock = 520,
			ReadonlyRollback = 776,
			AbortRollback = 516,
			ConstraintCheck = 275,
			ConstraintCommitHook = 531,
			ConstraintForeignKey = 787,
			ConstraintFunction = 1043,
			ConstraintNotNull = 1299,
			ConstraintPrimaryKey = 1555,
			ConstraintTrigger = 1811,
			ConstraintUnique = 2067,
			ConstraintVTab = 2323,
			NoticeRecoverWAL = 283,
			NoticeRecoverRollback = 539
		}

		public enum ConfigOption
		{
			SingleThread = 1,
			MultiThread,
			Serialized
		}

		public enum ColType
		{
			Integer = 1,
			Float,
			Text,
			Blob,
			Null
		}
	}
}
