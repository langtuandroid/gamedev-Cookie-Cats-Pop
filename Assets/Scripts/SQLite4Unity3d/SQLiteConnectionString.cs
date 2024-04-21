using System;

namespace SQLite4Unity3d
{
	internal class SQLiteConnectionString
	{
		public SQLiteConnectionString(string databasePath, bool storeDateTimeAsTicks)
		{
			this.ConnectionString = databasePath;
			this.StoreDateTimeAsTicks = storeDateTimeAsTicks;
			this.DatabasePath = databasePath;
		}

		public string ConnectionString { get; private set; }

		public string DatabasePath { get; private set; }

		public bool StoreDateTimeAsTicks { get; private set; }
	}
}
