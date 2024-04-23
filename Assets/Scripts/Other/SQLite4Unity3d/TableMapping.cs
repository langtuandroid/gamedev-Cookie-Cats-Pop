using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLite4Unity3d
{
	public class TableMapping
	{
		public TableMapping(Type type, CreateFlags createFlags = CreateFlags.None)
		{
			this.MappedType = type;
			TableAttribute tableAttribute = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault<object>();
			this.TableName = ((tableAttribute == null) ? this.MappedType.Name : tableAttribute.Name);
			PropertyInfo[] properties = this.MappedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			List<TableMapping.Column> list = new List<TableMapping.Column>();
			foreach (PropertyInfo propertyInfo in properties)
			{
				bool flag = propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0;
				if (propertyInfo.CanWrite && !flag)
				{
					list.Add(new TableMapping.Column(propertyInfo, createFlags));
				}
			}
			this.Columns = list.ToArray();
			foreach (TableMapping.Column column in this.Columns)
			{
				if (column.IsAutoInc && column.IsPK)
				{
					this._autoPk = column;
				}
				if (column.IsPK)
				{
					this.PK = column;
				}
			}
			this.HasAutoIncPK = (this._autoPk != null);
			if (this.PK != null)
			{
				this.GetByPrimaryKeySql = string.Format("select * from \"{0}\" where \"{1}\" = ?", this.TableName, this.PK.Name);
			}
			else
			{
				this.GetByPrimaryKeySql = string.Format("select * from \"{0}\" limit 1", this.TableName);
			}
		}

		public Type MappedType { get; private set; }

		public string TableName { get; private set; }

		public TableMapping.Column[] Columns { get; private set; }

		public TableMapping.Column PK { get; private set; }

		public string GetByPrimaryKeySql { get; private set; }

		public bool HasAutoIncPK { get; private set; }

		public void SetAutoIncPK(object obj, long id)
		{
			if (this._autoPk != null)
			{
				this._autoPk.SetValue(obj, Convert.ChangeType(id, this._autoPk.ColumnType, null));
			}
		}

		public TableMapping.Column[] InsertColumns
		{
			get
			{
				if (this._insertColumns == null)
				{
					this._insertColumns = (from c in this.Columns
					where !c.IsAutoInc
					select c).ToArray<TableMapping.Column>();
				}
				return this._insertColumns;
			}
		}

		public TableMapping.Column[] InsertOrReplaceColumns
		{
			get
			{
				if (this._insertOrReplaceColumns == null)
				{
					this._insertOrReplaceColumns = this.Columns.ToArray<TableMapping.Column>();
				}
				return this._insertOrReplaceColumns;
			}
		}

		public TableMapping.Column FindColumnWithPropertyName(string propertyName)
		{
			return this.Columns.FirstOrDefault((TableMapping.Column c) => c.PropertyName == propertyName);
		}

		public TableMapping.Column FindColumn(string columnName)
		{
			return this.Columns.FirstOrDefault((TableMapping.Column c) => c.Name == columnName);
		}

		public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
		{
			if (this._insertCommand == null)
			{
				this._insertCommand = this.CreateInsertCommand(conn, extra);
				this._insertCommandExtra = extra;
			}
			else if (this._insertCommandExtra != extra)
			{
				this._insertCommand.Dispose();
				this._insertCommand = this.CreateInsertCommand(conn, extra);
				this._insertCommandExtra = extra;
			}
			return this._insertCommand;
		}

		private PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
		{
			TableMapping.Column[] source = this.InsertColumns;
			string commandText;
			if (!source.Any<TableMapping.Column>() && this.Columns.Count<TableMapping.Column>() == 1 && this.Columns[0].IsAutoInc)
			{
				commandText = string.Format("insert {1} into \"{0}\" default values", this.TableName, extra);
			}
			else
			{
				bool flag = string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0;
				if (flag)
				{
					source = this.InsertOrReplaceColumns;
				}
				string format = "insert {3} into \"{0}\"({1}) values ({2})";
				object[] array = new object[4];
				array[0] = this.TableName;
				array[1] = string.Join(",", (from c in source
				select "\"" + c.Name + "\"").ToArray<string>());
				array[2] = string.Join(",", (from c in source
				select "?").ToArray<string>());
				array[3] = extra;
				commandText = string.Format(format, array);
			}
			return new PreparedSqlLiteInsertCommand(conn)
			{
				CommandText = commandText
			};
		}

		protected internal void Dispose()
		{
			if (this._insertCommand != null)
			{
				this._insertCommand.Dispose();
				this._insertCommand = null;
			}
		}

		private TableMapping.Column _autoPk;

		private TableMapping.Column[] _insertColumns;

		private TableMapping.Column[] _insertOrReplaceColumns;

		private PreparedSqlLiteInsertCommand _insertCommand;

		private string _insertCommandExtra;

		public class Column
		{
			public Column(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None)
			{
				ColumnAttribute columnAttribute = (ColumnAttribute)prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault<object>();
				this._prop = prop;
				this.Name = ((columnAttribute != null) ? columnAttribute.Name : prop.Name);
				this.ColumnType = (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
				this.Collation = Orm.Collation(prop);
				this.IsPK = (Orm.IsPK(prop) || ((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK && string.Compare(prop.Name, "Id", StringComparison.OrdinalIgnoreCase) == 0));
				bool flag = Orm.IsAutoInc(prop) || (this.IsPK && (createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK);
				this.IsAutoGuid = (flag && this.ColumnType == typeof(Guid));
				this.IsAutoInc = (flag && !this.IsAutoGuid);
				this.Indices = Orm.GetIndices(prop);
				if (!this.Indices.Any<IndexedAttribute>() && !this.IsPK && (createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex && this.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
				{
					this.Indices = new IndexedAttribute[]
					{
						new IndexedAttribute()
					};
				}
				this.IsNullable = (!this.IsPK && !Orm.IsMarkedNotNull(prop));
				this.MaxStringLength = Orm.MaxStringLength(prop);
			}

			public string Name { get; private set; }

			public string PropertyName
			{
				get
				{
					return this._prop.Name;
				}
			}

			public Type ColumnType { get; private set; }

			public string Collation { get; private set; }

			public bool IsAutoInc { get; private set; }

			public bool IsAutoGuid { get; private set; }

			public bool IsPK { get; private set; }

			public IEnumerable<IndexedAttribute> Indices { get; set; }

			public bool IsNullable { get; private set; }

			public int? MaxStringLength { get; private set; }

			public void SetValue(object obj, object val)
			{
				this._prop.SetValue(obj, val, null);
			}

			public object GetValue(object obj)
			{
				return this._prop.GetGetMethod().Invoke(obj, null);
			}

			private PropertyInfo _prop;
		}
	}
}
