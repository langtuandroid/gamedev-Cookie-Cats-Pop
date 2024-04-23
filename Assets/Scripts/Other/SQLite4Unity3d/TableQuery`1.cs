using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SQLite4Unity3d
{
	public class TableQuery<T> : BaseTableQuery, IEnumerable<T>, IEnumerable
	{
		private TableQuery(SQLiteConnection conn, TableMapping table)
		{
			this.Connection = conn;
			this.Table = table;
		}

		public TableQuery(SQLiteConnection conn)
		{
			this.Connection = conn;
			this.Table = this.Connection.GetMapping(typeof(T), CreateFlags.None);
		}

		public SQLiteConnection Connection { get; private set; }

		public TableMapping Table { get; private set; }

		public TableQuery<U> Clone<U>()
		{
			TableQuery<U> tableQuery = new TableQuery<U>(this.Connection, this.Table);
			tableQuery._where = this._where;
			tableQuery._deferred = this._deferred;
			if (this._orderBys != null)
			{
				tableQuery._orderBys = new List<BaseTableQuery.Ordering>(this._orderBys);
			}
			tableQuery._limit = this._limit;
			tableQuery._offset = this._offset;
			tableQuery._joinInner = this._joinInner;
			tableQuery._joinInnerKeySelector = this._joinInnerKeySelector;
			tableQuery._joinOuter = this._joinOuter;
			tableQuery._joinOuterKeySelector = this._joinOuterKeySelector;
			tableQuery._joinSelector = this._joinSelector;
			tableQuery._selector = this._selector;
			return tableQuery;
		}

		public TableQuery<T> Where(Expression<Func<T, bool>> predExpr)
		{
			if (predExpr.NodeType == ExpressionType.Lambda)
			{
				Expression body = predExpr.Body;
				TableQuery<T> tableQuery = this.Clone<T>();
				tableQuery.AddWhere(body);
				return tableQuery;
			}
			throw new NotSupportedException("Must be a predicate");
		}

		public TableQuery<T> Take(int n)
		{
			TableQuery<T> tableQuery = this.Clone<T>();
			tableQuery._limit = new int?(n);
			return tableQuery;
		}

		public TableQuery<T> Skip(int n)
		{
			TableQuery<T> tableQuery = this.Clone<T>();
			tableQuery._offset = new int?(n);
			return tableQuery;
		}

		public T ElementAt(int index)
		{
			return this.Skip(index).Take(1).First();
		}

		public TableQuery<T> Deferred()
		{
			TableQuery<T> tableQuery = this.Clone<T>();
			tableQuery._deferred = true;
			return tableQuery;
		}

		public TableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
		{
			return this.AddOrderBy<U>(orderExpr, true);
		}

		public TableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
		{
			return this.AddOrderBy<U>(orderExpr, false);
		}

		public TableQuery<T> ThenBy<U>(Expression<Func<T, U>> orderExpr)
		{
			return this.AddOrderBy<U>(orderExpr, true);
		}

		public TableQuery<T> ThenByDescending<U>(Expression<Func<T, U>> orderExpr)
		{
			return this.AddOrderBy<U>(orderExpr, false);
		}

		private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc)
		{
			if (orderExpr.NodeType != ExpressionType.Lambda)
			{
				throw new NotSupportedException("Must be a predicate");
			}
			UnaryExpression unaryExpression = orderExpr.Body as UnaryExpression;
			MemberExpression memberExpression;
			if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
			{
				memberExpression = (unaryExpression.Operand as MemberExpression);
			}
			else
			{
				memberExpression = (orderExpr.Body as MemberExpression);
			}
			if (memberExpression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
			{
				TableQuery<T> tableQuery = this.Clone<T>();
				if (tableQuery._orderBys == null)
				{
					tableQuery._orderBys = new List<BaseTableQuery.Ordering>();
				}
				tableQuery._orderBys.Add(new BaseTableQuery.Ordering
				{
					ColumnName = this.Table.FindColumnWithPropertyName(memberExpression.Member.Name).Name,
					Ascending = asc
				});
				return tableQuery;
			}
			throw new NotSupportedException("Order By does not support: " + orderExpr);
		}

		private void AddWhere(Expression pred)
		{
			if (this._where == null)
			{
				this._where = pred;
			}
			else
			{
				this._where = Expression.AndAlso(this._where, pred);
			}
		}

		public TableQuery<TResult> Join<TInner, TKey, TResult>(TableQuery<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
		{
			return new TableQuery<TResult>(this.Connection, this.Connection.GetMapping(typeof(TResult), CreateFlags.None))
			{
				_joinOuter = this,
				_joinOuterKeySelector = outerKeySelector,
				_joinInner = inner,
				_joinInnerKeySelector = innerKeySelector,
				_joinSelector = resultSelector
			};
		}

		public TableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
		{
			TableQuery<TResult> tableQuery = this.Clone<TResult>();
			tableQuery._selector = selector;
			return tableQuery;
		}

		private SQLiteCommand GenerateCommand(string selectionList)
		{
			if (this._joinInner != null && this._joinOuter != null)
			{
				throw new NotSupportedException("Joins are not supported.");
			}
			string text = string.Concat(new string[]
			{
				"select ",
				selectionList,
				" from \"",
				this.Table.TableName,
				"\""
			});
			List<object> list = new List<object>();
			if (this._where != null)
			{
				TableQuery<T>.CompileResult compileResult = this.CompileExpr(this._where, list);
				text = text + " where " + compileResult.CommandText;
			}
			if (this._orderBys != null && this._orderBys.Count > 0)
			{
				string str = string.Join(", ", (from o in this._orderBys
				select "\"" + o.ColumnName + "\"" + ((!o.Ascending) ? " desc" : string.Empty)).ToArray<string>());
				text = text + " order by " + str;
			}
			if (this._limit != null)
			{
				text = text + " limit " + this._limit.Value;
			}
			if (this._offset != null)
			{
				if (this._limit == null)
				{
					text += " limit -1 ";
				}
				text = text + " offset " + this._offset.Value;
			}
			return this.Connection.CreateCommand(text, list.ToArray());
		}

		private TableQuery<T>.CompileResult CompileExpr(Expression expr, List<object> queryArgs)
		{
			if (expr == null)
			{
				throw new NotSupportedException("Expression is NULL");
			}
			if (expr is BinaryExpression)
			{
				BinaryExpression binaryExpression = (BinaryExpression)expr;
				TableQuery<T>.CompileResult compileResult = this.CompileExpr(binaryExpression.Left, queryArgs);
				TableQuery<T>.CompileResult compileResult2 = this.CompileExpr(binaryExpression.Right, queryArgs);
				string commandText;
				if (compileResult.CommandText == "?" && compileResult.Value == null)
				{
					commandText = this.CompileNullBinaryExpression(binaryExpression, compileResult2);
				}
				else if (compileResult2.CommandText == "?" && compileResult2.Value == null)
				{
					commandText = this.CompileNullBinaryExpression(binaryExpression, compileResult);
				}
				else
				{
					commandText = string.Concat(new string[]
					{
						"(",
						compileResult.CommandText,
						" ",
						this.GetSqlName(binaryExpression),
						" ",
						compileResult2.CommandText,
						")"
					});
				}
				return new TableQuery<T>.CompileResult
				{
					CommandText = commandText
				};
			}
			if (expr.NodeType == ExpressionType.Call)
			{
				MethodCallExpression methodCallExpression = (MethodCallExpression)expr;
				TableQuery<T>.CompileResult[] array = new TableQuery<T>.CompileResult[methodCallExpression.Arguments.Count];
				TableQuery<T>.CompileResult compileResult3 = (methodCallExpression.Object == null) ? null : this.CompileExpr(methodCallExpression.Object, queryArgs);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.CompileExpr(methodCallExpression.Arguments[i], queryArgs);
				}
				string commandText2 = string.Empty;
				if (methodCallExpression.Method.Name == "Like" && array.Length == 2)
				{
					commandText2 = string.Concat(new string[]
					{
						"(",
						array[0].CommandText,
						" like ",
						array[1].CommandText,
						")"
					});
				}
				else if (methodCallExpression.Method.Name == "Contains" && array.Length == 2)
				{
					commandText2 = string.Concat(new string[]
					{
						"(",
						array[1].CommandText,
						" in ",
						array[0].CommandText,
						")"
					});
				}
				else if (methodCallExpression.Method.Name == "Contains" && array.Length == 1)
				{
					if (methodCallExpression.Object != null && methodCallExpression.Object.Type == typeof(string))
					{
						commandText2 = string.Concat(new string[]
						{
							"(",
							compileResult3.CommandText,
							" like ('%' || ",
							array[0].CommandText,
							" || '%'))"
						});
					}
					else
					{
						commandText2 = string.Concat(new string[]
						{
							"(",
							array[0].CommandText,
							" in ",
							compileResult3.CommandText,
							")"
						});
					}
				}
				else if (methodCallExpression.Method.Name == "StartsWith" && array.Length == 1)
				{
					commandText2 = string.Concat(new string[]
					{
						"(",
						compileResult3.CommandText,
						" like (",
						array[0].CommandText,
						" || '%'))"
					});
				}
				else if (methodCallExpression.Method.Name == "EndsWith" && array.Length == 1)
				{
					commandText2 = string.Concat(new string[]
					{
						"(",
						compileResult3.CommandText,
						" like ('%' || ",
						array[0].CommandText,
						"))"
					});
				}
				else if (methodCallExpression.Method.Name == "Equals" && array.Length == 1)
				{
					commandText2 = string.Concat(new string[]
					{
						"(",
						compileResult3.CommandText,
						" = (",
						array[0].CommandText,
						"))"
					});
				}
				else if (methodCallExpression.Method.Name == "ToLower")
				{
					commandText2 = "(lower(" + compileResult3.CommandText + "))";
				}
				else if (methodCallExpression.Method.Name == "ToUpper")
				{
					commandText2 = "(upper(" + compileResult3.CommandText + "))";
				}
				else
				{
					commandText2 = methodCallExpression.Method.Name.ToLower() + "(" + string.Join(",", (from a in array
					select a.CommandText).ToArray<string>()) + ")";
				}
				return new TableQuery<T>.CompileResult
				{
					CommandText = commandText2
				};
			}
			if (expr.NodeType == ExpressionType.Constant)
			{
				ConstantExpression constantExpression = (ConstantExpression)expr;
				queryArgs.Add(constantExpression.Value);
				return new TableQuery<T>.CompileResult
				{
					CommandText = "?",
					Value = constantExpression.Value
				};
			}
			if (expr.NodeType == ExpressionType.Convert)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expr;
				Type type = unaryExpression.Type;
				TableQuery<T>.CompileResult compileResult4 = this.CompileExpr(unaryExpression.Operand, queryArgs);
				return new TableQuery<T>.CompileResult
				{
					CommandText = compileResult4.CommandText,
					Value = ((compileResult4.Value == null) ? null : TableQuery<T>.ConvertTo(compileResult4.Value, type))
				};
			}
			if (expr.NodeType == ExpressionType.Not)
			{
				UnaryExpression unaryExpression2 = (UnaryExpression)expr;
				Type type2 = unaryExpression2.Type;
				TableQuery<T>.CompileResult compileResult5 = this.CompileExpr(unaryExpression2.Operand, queryArgs);
				return new TableQuery<T>.CompileResult
				{
					CommandText = "NOT " + compileResult5.CommandText,
					Value = ((compileResult5.Value == null) ? null : compileResult5.Value)
				};
			}
			if (expr.NodeType != ExpressionType.MemberAccess)
			{
				throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
			}
			MemberExpression memberExpression = (MemberExpression)expr;
			if (memberExpression.Expression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
			{
				string name = this.Table.FindColumnWithPropertyName(memberExpression.Member.Name).Name;
				return new TableQuery<T>.CompileResult
				{
					CommandText = "\"" + name + "\""
				};
			}
			object obj = null;
			if (memberExpression.Expression != null)
			{
				TableQuery<T>.CompileResult compileResult6 = this.CompileExpr(memberExpression.Expression, queryArgs);
				if (compileResult6.Value == null)
				{
					throw new NotSupportedException("Member access failed to compile expression");
				}
				if (compileResult6.CommandText == "?")
				{
					queryArgs.RemoveAt(queryArgs.Count - 1);
				}
				obj = compileResult6.Value;
			}
			object obj2 = null;
			if (memberExpression.Member.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
				obj2 = propertyInfo.GetGetMethod().Invoke(obj, null);
			}
			else
			{
				if (memberExpression.Member.MemberType != MemberTypes.Field)
				{
					throw new NotSupportedException("MemberExpr: " + memberExpression.Member.MemberType);
				}
				FieldInfo fieldInfo = (FieldInfo)memberExpression.Member;
				obj2 = fieldInfo.GetValue(obj);
			}
			if (obj2 != null && obj2 is IEnumerable && !(obj2 is string) && !(obj2 is IEnumerable<byte>))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("(");
				string value = string.Empty;
				IEnumerator enumerator = ((IEnumerable)obj2).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object item = enumerator.Current;
						queryArgs.Add(item);
						stringBuilder.Append(value);
						stringBuilder.Append("?");
						value = ",";
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
				stringBuilder.Append(")");
				return new TableQuery<T>.CompileResult
				{
					CommandText = stringBuilder.ToString(),
					Value = obj2
				};
			}
			queryArgs.Add(obj2);
			return new TableQuery<T>.CompileResult
			{
				CommandText = "?",
				Value = obj2
			};
		}

		private static object ConvertTo(object obj, Type t)
		{
			Type underlyingType = Nullable.GetUnderlyingType(t);
			if (underlyingType == null)
			{
				return Convert.ChangeType(obj, t);
			}
			if (obj == null)
			{
				return null;
			}
			return Convert.ChangeType(obj, underlyingType);
		}

		private string CompileNullBinaryExpression(BinaryExpression expression, TableQuery<T>.CompileResult parameter)
		{
			if (expression.NodeType == ExpressionType.Equal)
			{
				return "(" + parameter.CommandText + " is ?)";
			}
			if (expression.NodeType == ExpressionType.NotEqual)
			{
				return "(" + parameter.CommandText + " is not ?)";
			}
			throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType.ToString());
		}

		private string GetSqlName(Expression expr)
		{
			ExpressionType nodeType = expr.NodeType;
			if (nodeType == ExpressionType.GreaterThan)
			{
				return ">";
			}
			if (nodeType == ExpressionType.GreaterThanOrEqual)
			{
				return ">=";
			}
			if (nodeType == ExpressionType.LessThan)
			{
				return "<";
			}
			if (nodeType == ExpressionType.LessThanOrEqual)
			{
				return "<=";
			}
			if (nodeType == ExpressionType.And)
			{
				return "&";
			}
			if (nodeType == ExpressionType.AndAlso)
			{
				return "and";
			}
			if (nodeType == ExpressionType.Or)
			{
				return "|";
			}
			if (nodeType == ExpressionType.OrElse)
			{
				return "or";
			}
			if (nodeType == ExpressionType.Equal)
			{
				return "=";
			}
			if (nodeType == ExpressionType.NotEqual)
			{
				return "!=";
			}
			throw new NotSupportedException("Cannot get SQL for: " + nodeType);
		}

		public int Count()
		{
			return this.GenerateCommand("count(*)").ExecuteScalar<int>();
		}

		public int Count(Expression<Func<T, bool>> predExpr)
		{
			return this.Where(predExpr).Count();
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (!this._deferred)
			{
				return this.GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();
			}
			return this.GenerateCommand("*").ExecuteDeferredQuery<T>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T First()
		{
			TableQuery<T> source = this.Take(1);
			return source.ToList<T>().First<T>();
		}

		public T FirstOrDefault()
		{
			TableQuery<T> source = this.Take(1);
			return source.ToList<T>().FirstOrDefault<T>();
		}

		private Expression _where;

		private List<BaseTableQuery.Ordering> _orderBys;

		private int? _limit;

		private int? _offset;

		private BaseTableQuery _joinInner;

		private Expression _joinInnerKeySelector;

		private BaseTableQuery _joinOuter;

		private Expression _joinOuterKeySelector;

		private Expression _joinSelector;

		private Expression _selector;

		private bool _deferred;

		private class CompileResult
		{
			public string CommandText { get; set; }

			public object Value { get; set; }
		}
	}
}
