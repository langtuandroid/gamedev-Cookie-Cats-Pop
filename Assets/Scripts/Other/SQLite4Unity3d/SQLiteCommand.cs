using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLite4Unity3d
{
	public class SQLiteCommand
	{
		internal SQLiteCommand(SQLiteConnection conn)
		{
			this._conn = conn;
			this._bindings = new List<SQLiteCommand.Binding>();
			this.CommandText = string.Empty;
		}

		public string CommandText { get; set; }

		public int ExecuteNonQuery()
		{
			if (this._conn.Trace)
			{
				this._conn.InvokeTrace("Executing: " + this);
			}
			SQLite3.Result result = SQLite3.Result.OK;
			object syncObject = this._conn.SyncObject;
			lock (syncObject)
			{
				IntPtr stmt = this.Prepare();
				result = SQLite3.Step(stmt);
				this.Finalize(stmt);
			}
			if (result == SQLite3.Result.Done)
			{
				return SQLite3.Changes(this._conn.Handle);
			}
			if (result == SQLite3.Result.Error)
			{
				string errmsg = SQLite3.GetErrmsg(this._conn.Handle);
				throw SQLiteException.New(result, errmsg);
			}
			if (result == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(this._conn.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
			{
				throw NotNullConstraintViolationException.New(result, SQLite3.GetErrmsg(this._conn.Handle));
			}
			throw SQLiteException.New(result, result.ToString());
		}

		public IEnumerable<T> ExecuteDeferredQuery<T>()
		{
			return this.ExecuteDeferredQuery<T>(this._conn.GetMapping(typeof(T), CreateFlags.None));
		}

		public List<T> ExecuteQuery<T>()
		{
			return this.ExecuteDeferredQuery<T>(this._conn.GetMapping(typeof(T), CreateFlags.None)).ToList<T>();
		}

		public List<T> ExecuteQuery<T>(TableMapping map)
		{
			return this.ExecuteDeferredQuery<T>(map).ToList<T>();
		}

		protected virtual void OnInstanceCreated(object obj)
		{
		}

		public IEnumerable<T> ExecuteDeferredQuery<T>(TableMapping map)
		{
			if (this._conn.Trace)
			{
				this._conn.InvokeTrace("Executing Query: " + this);
			}
			object syncObject = this._conn.SyncObject;
			lock (syncObject)
			{
				IntPtr stmt = this.Prepare();
				try
				{
					TableMapping.Column[] cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];
					for (int i = 0; i < cols.Length; i++)
					{
						string columnName = SQLite3.ColumnName16(stmt, i);
						cols[i] = map.FindColumn(columnName);
					}
					while (SQLite3.Step(stmt) == SQLite3.Result.Row)
					{
						object obj = Activator.CreateInstance(map.MappedType);
						for (int j = 0; j < cols.Length; j++)
						{
							if (cols[j] != null)
							{
								SQLite3.ColType type = SQLite3.ColumnType(stmt, j);
								object val = this.ReadCol(stmt, j, type, cols[j].ColumnType);
								cols[j].SetValue(obj, val);
							}
						}
						this.OnInstanceCreated(obj);
						yield return (T)((object)obj);
					}
				}
				finally
				{
					SQLite3.Finalize(stmt);
				}
			}
			yield break;
		}

		public T ExecuteScalar<T>()
		{
			if (this._conn.Trace)
			{
				this._conn.InvokeTrace("Executing Query: " + this);
			}
			T result = default(T);
			object syncObject = this._conn.SyncObject;
			lock (syncObject)
			{
				IntPtr stmt = this.Prepare();
				try
				{
					SQLite3.Result result2 = SQLite3.Step(stmt);
					if (result2 == SQLite3.Result.Row)
					{
						SQLite3.ColType type = SQLite3.ColumnType(stmt, 0);
						result = (T)((object)this.ReadCol(stmt, 0, type, typeof(T)));
					}
					else if (result2 != SQLite3.Result.Done)
					{
						throw SQLiteException.New(result2, SQLite3.GetErrmsg(this._conn.Handle));
					}
				}
				finally
				{
					this.Finalize(stmt);
				}
			}
			return result;
		}

		public void Bind(string name, object val)
		{
			this._bindings.Add(new SQLiteCommand.Binding
			{
				Name = name,
				Value = val
			});
		}

		public void Bind(object val)
		{
			this.Bind(null, val);
		}

		public override string ToString()
		{
			string[] array = new string[1 + this._bindings.Count];
			array[0] = this.CommandText;
			int num = 1;
			foreach (SQLiteCommand.Binding binding in this._bindings)
			{
				array[num] = string.Format("  {0}: {1}", num - 1, binding.Value);
				num++;
			}
			return string.Join(Environment.NewLine, array);
		}

		private IntPtr Prepare()
		{
			IntPtr intPtr = SQLite3.Prepare2(this._conn.Handle, this.CommandText);
			this.BindAll(intPtr);
			return intPtr;
		}

		private void Finalize(IntPtr stmt)
		{
			SQLite3.Finalize(stmt);
		}

		private void BindAll(IntPtr stmt)
		{
			int num = 1;
			foreach (SQLiteCommand.Binding binding in this._bindings)
			{
				if (binding.Name != null)
				{
					binding.Index = SQLite3.BindParameterIndex(stmt, binding.Name);
				}
				else
				{
					binding.Index = num++;
				}
				SQLiteCommand.BindParameter(stmt, binding.Index, binding.Value, this._conn.StoreDateTimeAsTicks);
			}
		}

		internal static void BindParameter(IntPtr stmt, int index, object value, bool storeDateTimeAsTicks)
		{
			if (value == null)
			{
				SQLite3.BindNull(stmt, index);
			}
			else if (value is int)
			{
				SQLite3.BindInt(stmt, index, (int)value);
			}
			else if (value is string)
			{
				SQLite3.BindText(stmt, index, (string)value, -1, SQLiteCommand.NegativePointer);
			}
			else if (value is byte || value is ushort || value is sbyte || value is short)
			{
				SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
			}
			else if (value is bool)
			{
				SQLite3.BindInt(stmt, index, (!(bool)value) ? 0 : 1);
			}
			else if (value is uint || value is long)
			{
				SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
			}
			else if (value is float || value is double || value is decimal)
			{
				SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
			}
			else if (value is TimeSpan)
			{
				SQLite3.BindInt64(stmt, index, ((TimeSpan)value).Ticks);
			}
			else if (value is DateTime)
			{
				if (storeDateTimeAsTicks)
				{
					SQLite3.BindInt64(stmt, index, ((DateTime)value).Ticks);
				}
				else
				{
					SQLite3.BindText(stmt, index, ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"), -1, SQLiteCommand.NegativePointer);
				}
			}
			else if (value is DateTimeOffset)
			{
				SQLite3.BindInt64(stmt, index, ((DateTimeOffset)value).UtcTicks);
			}
			else if (value.GetType().IsEnum)
			{
				SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
			}
			else if (value is byte[])
			{
				SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, SQLiteCommand.NegativePointer);
			}
			else
			{
				if (!(value is Guid))
				{
					throw new NotSupportedException("Cannot store type: " + value.GetType());
				}
				SQLite3.BindText(stmt, index, ((Guid)value).ToString(), 72, SQLiteCommand.NegativePointer);
			}
		}

		private object ReadCol(IntPtr stmt, int index, SQLite3.ColType type, Type clrType)
		{
			if (type == SQLite3.ColType.Null)
			{
				return null;
			}
			if (clrType == typeof(string))
			{
				return SQLite3.ColumnString(stmt, index);
			}
			if (clrType == typeof(int))
			{
				return SQLite3.ColumnInt(stmt, index);
			}
			if (clrType == typeof(bool))
			{
				return SQLite3.ColumnInt(stmt, index) == 1;
			}
			if (clrType == typeof(double))
			{
				return SQLite3.ColumnDouble(stmt, index);
			}
			if (clrType == typeof(float))
			{
				return (float)SQLite3.ColumnDouble(stmt, index);
			}
			if (clrType == typeof(TimeSpan))
			{
				return new TimeSpan(SQLite3.ColumnInt64(stmt, index));
			}
			if (clrType == typeof(DateTime))
			{
				if (this._conn.StoreDateTimeAsTicks)
				{
					return new DateTime(SQLite3.ColumnInt64(stmt, index));
				}
				string s = SQLite3.ColumnString(stmt, index);
				return DateTime.Parse(s);
			}
			else
			{
				if (clrType == typeof(DateTimeOffset))
				{
					return new DateTimeOffset(SQLite3.ColumnInt64(stmt, index), TimeSpan.Zero);
				}
				if (clrType.IsEnum)
				{
					return SQLite3.ColumnInt(stmt, index);
				}
				if (clrType == typeof(long))
				{
					return SQLite3.ColumnInt64(stmt, index);
				}
				if (clrType == typeof(uint))
				{
					return (uint)SQLite3.ColumnInt64(stmt, index);
				}
				if (clrType == typeof(decimal))
				{
					return (decimal)SQLite3.ColumnDouble(stmt, index);
				}
				if (clrType == typeof(byte))
				{
					return (byte)SQLite3.ColumnInt(stmt, index);
				}
				if (clrType == typeof(ushort))
				{
					return (ushort)SQLite3.ColumnInt(stmt, index);
				}
				if (clrType == typeof(short))
				{
					return (short)SQLite3.ColumnInt(stmt, index);
				}
				if (clrType == typeof(sbyte))
				{
					return (sbyte)SQLite3.ColumnInt(stmt, index);
				}
				if (clrType == typeof(byte[]))
				{
					return SQLite3.ColumnByteArray(stmt, index);
				}
				if (clrType == typeof(Guid))
				{
					string g = SQLite3.ColumnString(stmt, index);
					return new Guid(g);
				}
				throw new NotSupportedException("Don't know how to read " + clrType);
			}
		}

		private SQLiteConnection _conn;

		private List<SQLiteCommand.Binding> _bindings;

		internal static IntPtr NegativePointer = new IntPtr(-1);

		private class Binding
		{
			public string Name { get; set; }

			public object Value { get; set; }

			public int Index { get; set; }
		}
	}
}
