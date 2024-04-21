using System;
using System.Collections;

namespace SQLite4Unity3d
{
	public interface ISQLiteConnection
	{
		TableQuery<T> Table<T>() where T : new();

		int Insert(object obj, Type objType);

		int InsertAll(IEnumerable objects, Type objType);

		int InsertOrReplace(object obj, Type objType);

		int DropTable<T>();

		int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None);

		void Close();

		int Update(object obj, Type objType);

		int UpdateAll(IEnumerable objects);

		int Delete(object objectToDelete);

		int DeleteAll<T>();
	}
}
