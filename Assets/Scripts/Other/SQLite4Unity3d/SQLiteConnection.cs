using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SQLite4Unity3d
{
    public class SQLiteConnection : IDisposable, ISQLiteConnection
    {
        public SQLiteConnection(string databasePath, bool storeDateTimeAsTicks = false) : this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
        {
        }

        public SQLiteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = false)
        {
            if (string.IsNullOrEmpty(databasePath))
            {
                throw new ArgumentException("Must be specified", "databasePath");
            }
            this.DatabasePath = databasePath;
            this.mayCreateSyncObject(databasePath);
            byte[] nullTerminatedUtf = SQLiteConnection.GetNullTerminatedUtf8(this.DatabasePath);
            IntPtr handle;
            SQLite3.Result result = SQLite3.Open(nullTerminatedUtf, out handle, (int)openFlags, IntPtr.Zero);
            this.Handle = handle;
            if (result != SQLite3.Result.OK)
            {
                throw SQLiteException.New(result, string.Format("Could not open database file: {0} ({1})", this.DatabasePath, result));
            }
            this._open = true;
            this.StoreDateTimeAsTicks = storeDateTimeAsTicks;
            this.BusyTimeout = TimeSpan.FromSeconds(0.1);
        }

        static SQLiteConnection()
        {
            if (SQLiteConnection._preserveDuringLinkMagic)
            {
                SQLiteConnection.ColumnInfo columnInfo = new SQLiteConnection.ColumnInfo();
                columnInfo.Name = "magic";
            }
        }

        public IntPtr Handle { get; private set; }

        public string DatabasePath { get; private set; }

        public bool Trace { get; set; }

        public bool TimeExecution { get; set; }

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event SQLiteConnection.TraceHandler TraceEvent;

        internal void InvokeTrace(string message)
        {
            if (this.TraceEvent != null)
            {
                this.TraceEvent(message);
            }
        }

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event SQLiteConnection.TimeExecutionHandler TimeExecutionEvent;

        internal void InvokeTimeExecution(TimeSpan executionTime, TimeSpan totalExecutionTime)
        {
            if (this.TimeExecutionEvent != null)
            {
                this.TimeExecutionEvent(executionTime, totalExecutionTime);
            }
        }

        public bool StoreDateTimeAsTicks { get; private set; }

        private void mayCreateSyncObject(string databasePath)
        {
            if (!SQLiteConnection.syncObjects.ContainsKey(databasePath))
            {
                SQLiteConnection.syncObjects[databasePath] = new object();
            }
        }

        public object SyncObject
        {
            get
            {
                return SQLiteConnection.syncObjects[this.DatabasePath];
            }
        }

        public void EnableLoadExtension(int onoff)
        {
            SQLite3.Result result = SQLite3.EnableLoadExtension(this.Handle, onoff);
            if (result != SQLite3.Result.OK)
            {
                string errmsg = SQLite3.GetErrmsg(this.Handle);
                throw SQLiteException.New(result, errmsg);
            }
        }

        private static byte[] GetNullTerminatedUtf8(string s)
        {
            int num = Encoding.UTF8.GetByteCount(s);
            byte[] array = new byte[num + 1];
            num = Encoding.UTF8.GetBytes(s, 0, s.Length, array, 0);
            return array;
        }

        public TimeSpan BusyTimeout
        {
            get
            {
                return this._busyTimeout;
            }
            set
            {
                this._busyTimeout = value;
                if (this.Handle != SQLiteConnection.NullHandle)
                {
                    SQLite3.BusyTimeout(this.Handle, (int)this._busyTimeout.TotalMilliseconds);
                }
            }
        }

        public IEnumerable<TableMapping> TableMappings
        {
            get
            {
                return (this._tables == null) ? Enumerable.Empty<TableMapping>() : this._tables.Values;
            }
        }

        public TableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None)
        {
            if (this._mappings == null)
            {
                this._mappings = new Dictionary<string, TableMapping>();
            }
            TableMapping tableMapping;
            if (!this._mappings.TryGetValue(type.FullName, out tableMapping))
            {
                tableMapping = new TableMapping(type, createFlags);
                this._mappings[type.FullName] = tableMapping;
            }
            return tableMapping;
        }

        public TableMapping GetMapping<T>()
        {
            return this.GetMapping(typeof(T), CreateFlags.None);
        }

        public int DropTable<T>()
        {
            TableMapping mapping = this.GetMapping(typeof(T), CreateFlags.None);
            string query = string.Format("drop table if exists \"{0}\"", mapping.TableName);
            return this.Execute(query, new object[0]);
        }

        public int CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
        {
            return this.CreateTable(typeof(T), createFlags);
        }

        public int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None)
        {
            if (this._tables == null)
            {
                this._tables = new Dictionary<string, TableMapping>();
            }
            TableMapping mapping;
            if (!this._tables.TryGetValue(ty.FullName, out mapping))
            {
                mapping = this.GetMapping(ty, createFlags);
                this._tables.Add(ty.FullName, mapping);
            }
            string text = "create table if not exists \"" + mapping.TableName + "\"(\n";
            IEnumerable<string> source = from p in mapping.Columns
                                         select Orm.SqlDecl(p, this.StoreDateTimeAsTicks);
            string str = string.Join(",\n", source.ToArray<string>());
            text += str;
            text += ")";
            int num = this.Execute(text, new object[0]);
            if (num == 0)
            {
                this.MigrateTable(mapping);
            }
            Dictionary<string, SQLiteConnection.IndexInfo> dictionary = new Dictionary<string, SQLiteConnection.IndexInfo>();
            foreach (TableMapping.Column column in mapping.Columns)
            {
                foreach (IndexedAttribute indexedAttribute in column.Indices)
                {
                    string text2 = indexedAttribute.Name ?? (mapping.TableName + "_" + column.Name);
                    SQLiteConnection.IndexInfo value;
                    if (!dictionary.TryGetValue(text2, out value))
                    {
                        value = new SQLiteConnection.IndexInfo
                        {
                            IndexName = text2,
                            TableName = mapping.TableName,
                            Unique = indexedAttribute.Unique,
                            Columns = new List<SQLiteConnection.IndexedColumn>()
                        };
                        dictionary.Add(text2, value);
                    }
                    if (indexedAttribute.Unique != value.Unique)
                    {
                        throw new Exception("All the columns in an index must have the same value for their Unique property");
                    }
                    value.Columns.Add(new SQLiteConnection.IndexedColumn
                    {
                        Order = indexedAttribute.Order,
                        ColumnName = column.Name
                    });
                }
            }
            foreach (string text3 in dictionary.Keys)
            {
                SQLiteConnection.IndexInfo indexInfo = dictionary[text3];
                string[] array = new string[indexInfo.Columns.Count];
                if (indexInfo.Columns.Count == 1)
                {
                    array[0] = indexInfo.Columns[0].ColumnName;
                }
                else
                {
                    indexInfo.Columns.Sort((SQLiteConnection.IndexedColumn lhs, SQLiteConnection.IndexedColumn rhs) => lhs.Order - rhs.Order);
                    int j = 0;
                    int count = indexInfo.Columns.Count;
                    while (j < count)
                    {
                        array[j] = indexInfo.Columns[j].ColumnName;
                        j++;
                    }
                }
                num += this.CreateIndex(text3, indexInfo.TableName, array, indexInfo.Unique);
            }
            return num;
        }

        public int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false)
        {
            string query = string.Format("create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")", new object[]
            {
                tableName,
                string.Join("\", \"", columnNames),
                (!unique) ? string.Empty : "unique",
                indexName
            });
            return this.Execute(query, new object[0]);
        }

        public int CreateIndex(string indexName, string tableName, string columnName, bool unique = false)
        {
            return this.CreateIndex(indexName, tableName, new string[]
            {
                columnName
            }, unique);
        }

        public int CreateIndex(string tableName, string columnName, bool unique = false)
        {
            return this.CreateIndex(tableName + "_" + columnName, tableName, columnName, unique);
        }

        public int CreateIndex(string tableName, string[] columnNames, bool unique = false)
        {
            return this.CreateIndex(tableName + "_" + string.Join("_", columnNames), tableName, columnNames, unique);
        }

        public void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false)
        {
            MemberExpression memberExpression;
            if (property.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = (((UnaryExpression)property.Body).Operand as MemberExpression);
            }
            else
            {
                memberExpression = (property.Body as MemberExpression);
            }
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }
            string name = propertyInfo.Name;
            TableMapping mapping = this.GetMapping<T>();
            string name2 = mapping.FindColumnWithPropertyName(name).Name;
            this.CreateIndex(mapping.TableName, name2, unique);
        }

        public List<SQLiteConnection.ColumnInfo> GetTableInfo(string tableName)
        {
            string query = "pragma table_info(\"" + tableName + "\")";
            return this.Query<SQLiteConnection.ColumnInfo>(query, new object[0]);
        }

        private void MigrateTable(TableMapping map)
        {
            List<SQLiteConnection.ColumnInfo> tableInfo = this.GetTableInfo(map.TableName);
            List<TableMapping.Column> list = new List<TableMapping.Column>();
            foreach (TableMapping.Column column in map.Columns)
            {
                bool flag = false;
                foreach (SQLiteConnection.ColumnInfo columnInfo in tableInfo)
                {
                    flag = (string.Compare(column.Name, columnInfo.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    list.Add(column);
                }
            }
            foreach (TableMapping.Column p in list)
            {
                string query = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p, this.StoreDateTimeAsTicks);
                this.Execute(query, new object[0]);
            }
        }

        protected virtual SQLiteCommand NewCommand()
        {
            return new SQLiteCommand(this);
        }

        public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
        {
            if (!this._open)
            {
                throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");
            }
            SQLiteCommand sqliteCommand = this.NewCommand();
            sqliteCommand.CommandText = cmdText;
            foreach (object val in ps)
            {
                sqliteCommand.Bind(val);
            }
            return sqliteCommand;
        }

        public int Execute(string query, params object[] args)
        {
            SQLiteCommand sqliteCommand = this.CreateCommand(query, args);
            if (this.TimeExecution)
            {
                if (this._sw == null)
                {
                    this._sw = new Stopwatch();
                }
                this._sw.Reset();
                this._sw.Start();
            }
            int result = sqliteCommand.ExecuteNonQuery();
            if (this.TimeExecution)
            {
                this._sw.Stop();
                this._elapsed += this._sw.Elapsed;
                this.InvokeTimeExecution(this._sw.Elapsed, this._elapsed);
            }
            return result;
        }

        public T ExecuteScalar<T>(string query, params object[] args)
        {
            SQLiteCommand sqliteCommand = this.CreateCommand(query, args);
            if (this.TimeExecution)
            {
                if (this._sw == null)
                {
                    this._sw = new Stopwatch();
                }
                this._sw.Reset();
                this._sw.Start();
            }
            T result = sqliteCommand.ExecuteScalar<T>();
            if (this.TimeExecution)
            {
                this._sw.Stop();
                this._elapsed += this._sw.Elapsed;
                this.InvokeTimeExecution(this._sw.Elapsed, this._elapsed);
            }
            return result;
        }

        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            SQLiteCommand sqliteCommand = this.CreateCommand(query, args);
            return sqliteCommand.ExecuteQuery<T>();
        }

        public IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new()
        {
            SQLiteCommand sqliteCommand = this.CreateCommand(query, args);
            return sqliteCommand.ExecuteDeferredQuery<T>();
        }

        public List<object> Query(TableMapping map, string query, params object[] args)
        {
            SQLiteCommand sqliteCommand = this.CreateCommand(query, args);
            return sqliteCommand.ExecuteQuery<object>(map);
        }

        public IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args)
        {
            SQLiteCommand sqliteCommand = this.CreateCommand(query, args);
            return sqliteCommand.ExecuteDeferredQuery<object>(map);
        }

        public TableQuery<T> Table<T>() where T : new()
        {
            return new TableQuery<T>(this);
        }

        public T Get<T>(object pk) where T : new()
        {
            TableMapping mapping = this.GetMapping(typeof(T), CreateFlags.None);
            return this.Query<T>(mapping.GetByPrimaryKeySql, new object[]
            {
                pk
            }).First<T>();
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return this.Table<T>().Where(predicate).First();
        }

        public T Find<T>(object pk) where T : new()
        {
            TableMapping mapping = this.GetMapping(typeof(T), CreateFlags.None);
            return this.Query<T>(mapping.GetByPrimaryKeySql, new object[]
            {
                pk
            }).FirstOrDefault<T>();
        }

        public object Find(object pk, TableMapping map)
        {
            return this.Query(map, map.GetByPrimaryKeySql, new object[]
            {
                pk
            }).FirstOrDefault<object>();
        }

        public T Find<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return this.Table<T>().Where(predicate).FirstOrDefault();
        }

        public bool IsInTransaction
        {
            get
            {
                return this._transactionDepth > 0;
            }
        }

        public void BeginTransaction()
        {
            if (Interlocked.CompareExchange(ref this._transactionDepth, 1, 0) == 0)
            {
                try
                {
                    this.Execute("begin transaction", new object[0]);
                }
                catch (Exception ex)
                {
                    SQLiteException ex2 = ex as SQLiteException;
                    if (ex2 != null)
                    {
                        switch (ex2.Result)
                        {
                            case SQLite3.Result.Busy:
                            case SQLite3.Result.NoMem:
                            case SQLite3.Result.Interrupt:
                            case SQLite3.Result.IOError:
                            case SQLite3.Result.Full:
                                this.RollbackTo(null, true);
                                break;
                        }
                    }
                    else
                    {
                        Interlocked.Decrement(ref this._transactionDepth);
                    }
                    throw;
                }
                return;
            }
            throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
        }

        public string SaveTransactionPoint()
        {
            int num = Interlocked.Increment(ref this._transactionDepth) - 1;
            string text = string.Concat(new object[]
            {
                "S",
                this._rand.Next(32767),
                "D",
                num
            });
            try
            {
                this.Execute("savepoint " + text, new object[0]);
            }
            catch (Exception ex)
            {
                SQLiteException ex2 = ex as SQLiteException;
                if (ex2 != null)
                {
                    switch (ex2.Result)
                    {
                        case SQLite3.Result.Busy:
                        case SQLite3.Result.NoMem:
                        case SQLite3.Result.Interrupt:
                        case SQLite3.Result.IOError:
                        case SQLite3.Result.Full:
                            this.RollbackTo(null, true);
                            break;
                    }
                }
                else
                {
                    Interlocked.Decrement(ref this._transactionDepth);
                }
                throw;
            }
            return text;
        }

        public void Rollback()
        {
            this.RollbackTo(null, false);
        }

        public void RollbackTo(string savepoint)
        {
            this.RollbackTo(savepoint, false);
        }

        private void RollbackTo(string savepoint, bool noThrow)
        {
            try
            {
                if (string.IsNullOrEmpty(savepoint))
                {
                    if (Interlocked.Exchange(ref this._transactionDepth, 0) > 0)
                    {
                        this.Execute("rollback", new object[0]);
                    }
                }
                else
                {
                    this.DoSavePointExecute(savepoint, "rollback to ");
                }
            }
            catch (SQLiteException)
            {
                if (!noThrow)
                {
                    throw;
                }
            }
        }

        public void Release(string savepoint)
        {
            this.DoSavePointExecute(savepoint, "release ");
        }

        private void DoSavePointExecute(string savepoint, string cmd)
        {
            int num = savepoint.IndexOf('D');
            int num2;
            if (num >= 2 && savepoint.Length > num + 1 && int.TryParse(savepoint.Substring(num + 1), out num2) && 0 <= num2 && num2 < this._transactionDepth)
            {
                Thread.VolatileWrite(ref this._transactionDepth, num2);
                this.Execute(cmd + savepoint, new object[0]);
                return;
            }
            throw new ArgumentException("savePoint is not valid, and should be the result of a call to SaveTransactionPoint.", "savePoint");
        }

        public void Commit()
        {
            if (Interlocked.Exchange(ref this._transactionDepth, 0) != 0)
            {
                this.Execute("commit", new object[0]);
            }
        }

        public void RunInTransaction(Action action)
        {
            try
            {
                object obj = SQLiteConnection.syncObjects[this.DatabasePath];
                lock (obj)
                {
                    string savepoint = this.SaveTransactionPoint();
                    action();
                    this.Release(savepoint);
                }
            }
            catch (Exception)
            {
                this.Rollback();
                throw;
            }
        }

        public void RunInDatabaseLock(Action action)
        {
            object obj = SQLiteConnection.syncObjects[this.DatabasePath];
            lock (obj)
            {
                action();
            }
        }

        public int InsertAll(IEnumerable objects)
        {
            int c = 0;
            this.RunInTransaction(delegate
            {
                IEnumerator enumerator = objects.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        c += this.Insert(obj);
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }
            });
            return c;
        }

        public int InsertAll(IEnumerable objects, string extra)
        {
            int c = 0;
            this.RunInTransaction(delegate
            {
                IEnumerator enumerator = objects.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        c += this.Insert(obj, extra);
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }
            });
            return c;
        }

        public int InsertAll(IEnumerable objects, Type objType)
        {
            int c = 0;
            this.RunInTransaction(delegate
            {
                IEnumerator enumerator = objects.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        c += this.Insert(obj, objType);
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }
            });
            return c;
        }

        public int Insert(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Insert(obj, string.Empty, obj.GetType());
        }

        public int InsertOrReplace(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Insert(obj, "OR REPLACE", obj.GetType());
        }

        public int Insert(object obj, Type objType)
        {
            return this.Insert(obj, string.Empty, objType);
        }

        public int InsertOrReplace(object obj, Type objType)
        {
            return this.Insert(obj, "OR REPLACE", objType);
        }

        public int Insert(object obj, string extra)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Insert(obj, extra, obj.GetType());
        }

        public int Insert(object obj, string extra, Type objType)
        {
            if (obj == null || objType == null)
            {
                return 0;
            }
            TableMapping mapping = this.GetMapping(objType, CreateFlags.None);
            if (mapping.PK != null && mapping.PK.IsAutoGuid)
            {
                PropertyInfo property = objType.GetProperty(mapping.PK.PropertyName);
                if (property != null && property.GetGetMethod().Invoke(obj, null).Equals(Guid.Empty))
                {
                    property.SetValue(obj, Guid.NewGuid(), null);
                }
            }
            bool flag = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;
            TableMapping.Column[] array = (!flag) ? mapping.InsertColumns : mapping.InsertOrReplaceColumns;
            object[] array2 = new object[array.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i] = array[i].GetValue(obj);
            }
            PreparedSqlLiteInsertCommand insertCommand = mapping.GetInsertCommand(this, extra);
            int result;
            try
            {
                result = insertCommand.ExecuteNonQuery(array2);
            }
            catch (SQLiteException ex)
            {
                if (SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(ex.Result, ex.Message, mapping, obj);
                }
                throw;
            }
            if (mapping.HasAutoIncPK)
            {
                long id = SQLite3.LastInsertRowid(this.Handle);
                mapping.SetAutoIncPK(obj, id);
            }
            return result;
        }

        public int Update(object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return this.Update(obj, obj.GetType());
        }

        public int Update(object obj, Type objType)
        {
            int result = 0;
            if (obj == null || objType == null)
            {
                return 0;
            }
            TableMapping mapping = this.GetMapping(objType, CreateFlags.None);
            TableMapping.Column pk = mapping.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot update " + mapping.TableName + ": it has no PK");
            }
            IEnumerable<TableMapping.Column> source = from p in mapping.Columns
                                                      where p != pk
                                                      select p;
            IEnumerable<object> collection = from c in source
                                             select c.GetValue(obj);
            List<object> list = new List<object>(collection);
            list.Add(pk.GetValue(obj));
            string query = string.Format("update \"{0}\" set {1} where {2} = ? ", mapping.TableName, string.Join(",", (from c in source
                                                                                                                       select "\"" + c.Name + "\" = ? ").ToArray<string>()), pk.Name);
            try
            {
                result = this.Execute(query, list.ToArray());
            }
            catch (SQLiteException ex)
            {
                if (ex.Result == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(this.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
                {
                    throw NotNullConstraintViolationException.New(ex, mapping, obj);
                }
                throw ex;
            }
            return result;
        }

        public int UpdateAll(IEnumerable objects)
        {
            int c = 0;
            this.RunInTransaction(delegate
            {
                IEnumerator enumerator = objects.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        c += this.Update(obj);
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }
            });
            return c;
        }

        public int Delete(object objectToDelete)
        {
            TableMapping mapping = this.GetMapping(objectToDelete.GetType(), CreateFlags.None);
            TableMapping.Column pk = mapping.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
            }
            string query = string.Format("delete from \"{0}\" where \"{1}\" = ?", mapping.TableName, pk.Name);
            return this.Execute(query, new object[]
            {
                pk.GetValue(objectToDelete)
            });
        }

        public int Delete<T>(object primaryKey)
        {
            TableMapping mapping = this.GetMapping(typeof(T), CreateFlags.None);
            TableMapping.Column pk = mapping.PK;
            if (pk == null)
            {
                throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
            }
            string query = string.Format("delete from \"{0}\" where \"{1}\" = ?", mapping.TableName, pk.Name);
            return this.Execute(query, new object[]
            {
                primaryKey
            });
        }

        public int DeleteAll<T>()
        {
            TableMapping mapping = this.GetMapping(typeof(T), CreateFlags.None);
            string query = string.Format("delete from \"{0}\"", mapping.TableName);
            return this.Execute(query, new object[0]);
        }

        ~SQLiteConnection()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Close();
        }

        public void Close()
        {
            if (this._open && this.Handle != SQLiteConnection.NullHandle)
            {
                try
                {
                    if (this._mappings != null)
                    {
                        foreach (TableMapping tableMapping in this._mappings.Values)
                        {
                            tableMapping.Dispose();
                        }
                    }
                    SQLite3.Result result = SQLite3.Close(this.Handle);
                    if (result != SQLite3.Result.OK)
                    {
                        string errmsg = SQLite3.GetErrmsg(this.Handle);
                        throw SQLiteException.New(result, errmsg);
                    }
                }
                finally
                {
                    this.Handle = SQLiteConnection.NullHandle;
                    this._open = false;
                }
            }
        }

        private bool _open;

        private TimeSpan _busyTimeout;

        private Dictionary<string, TableMapping> _mappings;

        private Dictionary<string, TableMapping> _tables;

        private Stopwatch _sw;

        private TimeSpan _elapsed = default(TimeSpan);

        private int _transactionDepth;

        private Random _rand = new Random();

        internal static readonly IntPtr NullHandle = (IntPtr)0;

        private static Dictionary<string, object> syncObjects = new Dictionary<string, object>();

        private static bool _preserveDuringLinkMagic;

        public delegate void TraceHandler(string message);

        public delegate void TimeExecutionHandler(TimeSpan executionTime, TimeSpan totalExecutionTime);

        private struct IndexedColumn
        {
            public int Order;

            public string ColumnName;
        }

        private struct IndexInfo
        {
            public string IndexName;

            public string TableName;

            public bool Unique;

            public List<SQLiteConnection.IndexedColumn> Columns;
        }

        public class ColumnInfo
        {
            [Column("name")]
            public string Name { get; set; }

            public int notnull { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
