using System;

namespace SQLite4Unity3d
{
    public class PreparedSqlLiteInsertCommand : IDisposable
    {
        internal PreparedSqlLiteInsertCommand(SQLiteConnection conn)
        {
            this.Connection = conn;
        }

        public bool Initialized { get; set; }

        protected SQLiteConnection Connection { get; set; }

        public string CommandText { get; set; }

        protected IntPtr Statement { get; set; }

        public int ExecuteNonQuery(object[] source)
        {
            if (this.Connection.Trace)
            {
                this.Connection.InvokeTrace("Executing: " + this.CommandText);
            }
            if (!this.Initialized)
            {
                this.Statement = this.Prepare();
                this.Initialized = true;
            }
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    SQLiteCommand.BindParameter(this.Statement, i + 1, source[i], this.Connection.StoreDateTimeAsTicks);
                }
            }
            SQLite3.Result result = SQLite3.Step(this.Statement);
            if (result == SQLite3.Result.Done)
            {
                int result2 = SQLite3.Changes(this.Connection.Handle);
                SQLite3.Reset(this.Statement);
                return result2;
            }
            if (result == SQLite3.Result.Error)
            {
                string errmsg = SQLite3.GetErrmsg(this.Connection.Handle);
                SQLite3.Reset(this.Statement);
                throw SQLiteException.New(result, errmsg);
            }
            if (result == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(this.Connection.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
            {
                SQLite3.Reset(this.Statement);
                throw NotNullConstraintViolationException.New(result, SQLite3.GetErrmsg(this.Connection.Handle));
            }
            SQLite3.Reset(this.Statement);
            throw SQLiteException.New(result, result.ToString());
        }

        protected virtual IntPtr Prepare()
        {
            return SQLite3.Prepare2(this.Connection.Handle, this.CommandText);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.Statement != PreparedSqlLiteInsertCommand.NullStatement)
            {
                try
                {
                    SQLite3.Finalize(this.Statement);
                }
                finally
                {
                    this.Statement = PreparedSqlLiteInsertCommand.NullStatement;
                    this.Connection = null;
                }
            }
        }

        ~PreparedSqlLiteInsertCommand()
        {
            this.Dispose(false);
        }

        internal static readonly IntPtr NullStatement = (IntPtr)0;
    }
}
