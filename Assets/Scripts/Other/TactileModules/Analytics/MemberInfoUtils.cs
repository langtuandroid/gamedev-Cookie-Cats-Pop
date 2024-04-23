using System;
using System.Collections.Generic;
using System.Reflection;

namespace TactileModules.Analytics
{
	public static class MemberInfoUtils
	{
		public static bool IsGenericType(MemberInfo info)
		{
			return (info.MemberType != MemberTypes.Property) ? ((FieldInfo)info).FieldType.IsGenericType : ((PropertyInfo)info).PropertyType.IsGenericType;
		}

		public static Type GetGenericTypeDefinition(MemberInfo info)
		{
			return (info.MemberType != MemberTypes.Property) ? ((FieldInfo)info).FieldType.GetGenericTypeDefinition() : ((PropertyInfo)info).PropertyType.GetGenericTypeDefinition();
		}

		public static Type[] GetGenericArguments(MemberInfo info)
		{
			return (info.MemberType != MemberTypes.Property) ? ((FieldInfo)info).FieldType.GetGenericArguments() : ((PropertyInfo)info).PropertyType.GetGenericArguments();
		}

		public static object GetValue(MemberInfo info, object obj)
		{
			return (info.MemberType != MemberTypes.Property) ? ((FieldInfo)info).GetValue(obj) : ((PropertyInfo)info).GetValue(obj, null);
		}

		public static IEnumerable<MemberInfo> GetAllProperties(Type t)
		{
			foreach (PropertyInfo p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				yield return p;
			}
			if (t.BaseType != null)
			{
				foreach (MemberInfo i in MemberInfoUtils.GetAllProperties(t.BaseType))
				{
					yield return i;
				}
			}
			yield break;
		}
	}
}
