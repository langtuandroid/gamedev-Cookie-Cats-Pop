using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SQLite4Unity3d
{
	public static class SQLiteConnector
	{
		static SQLiteConnector()
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			if (SQLiteConnector._003C_003Ef__mg_0024cache0 == null)
			{
				SQLiteConnector._003C_003Ef__mg_0024cache0 = new EventHandler(SQLiteConnector.Destructor);
			}
			currentDomain.ProcessExit += SQLiteConnector._003C_003Ef__mg_0024cache0;
		}

		private static void Destructor(object sender, EventArgs e)
		{
			foreach (SQLiteConnector.ConnectionData connectionData in SQLiteConnector.Connections)
			{
				connectionData.connection.Close();
			}
		}

		public static ISQLiteConnection GetConnection(string databasePath)
		{
			SQLiteConnector.ConnectionData connectionData = SQLiteConnector.Connections.Find((SQLiteConnector.ConnectionData connection) => connection.databasePath == databasePath);
			if (connectionData != null)
			{
				return connectionData.connection;
			}
			SQLiteConnection sqliteConnection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, false);
			SQLiteConnector.ConnectionData item = new SQLiteConnector.ConnectionData(sqliteConnection, databasePath);
			SQLiteConnector.Connections.Add(item);
			return sqliteConnection;
		}

		private static readonly List<SQLiteConnector.ConnectionData> Connections = new List<SQLiteConnector.ConnectionData>();

		[CompilerGenerated]
		private static EventHandler _003C_003Ef__mg_0024cache0;

		private class ConnectionData
		{
			public ConnectionData(ISQLiteConnection connection, string databasePath)
			{
				this.connection = connection;
				this.databasePath = databasePath;
			}

			public readonly ISQLiteConnection connection;

			public readonly string databasePath;
		}
	}
}
