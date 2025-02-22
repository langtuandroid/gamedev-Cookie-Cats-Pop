using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLite4Unity3d
{
	public class NotNullConstraintViolationException : SQLiteException
	{
		protected NotNullConstraintViolationException(SQLite3.Result r, string message) : this(r, message, null, null)
		{
		}

		protected NotNullConstraintViolationException(SQLite3.Result r, string message, TableMapping mapping, object obj) : base(r, message)
		{
			if (mapping != null && obj != null)
			{
				this.Columns = from c in mapping.Columns
				where !c.IsNullable && c.GetValue(obj) == null
				select c;
			}
		}

		public IEnumerable<TableMapping.Column> Columns { get; protected set; }

		public new static NotNullConstraintViolationException New(SQLite3.Result r, string message)
		{
			return new NotNullConstraintViolationException(r, message);
		}

		public static NotNullConstraintViolationException New(SQLite3.Result r, string message, TableMapping mapping, object obj)
		{
			return new NotNullConstraintViolationException(r, message, mapping, obj);
		}

		public static NotNullConstraintViolationException New(SQLiteException exception, TableMapping mapping, object obj)
		{
			return new NotNullConstraintViolationException(exception.Result, exception.Message, mapping, obj);
		}
	}
}
