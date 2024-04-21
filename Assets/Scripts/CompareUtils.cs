using System;
using System.Collections;
using System.Reflection;

public static class CompareUtils
{
	public static bool AreObjectsEqual(object obj1, object obj2)
	{
		if (obj1 == null || obj2 == null)
		{
			return false;
		}
		if (!obj1.GetType().Equals(obj2.GetType()))
		{
			return false;
		}
		Type type = obj1.GetType();
		if (type.IsPrimitive)
		{
			return obj1.Equals(obj2);
		}
		if (typeof(string).Equals(type))
		{
			string text = (string)obj1;
			string value = (string)obj2;
			return text.Equals(value, StringComparison.OrdinalIgnoreCase);
		}
		if (type.IsArray)
		{
			Array array = obj1 as Array;
			Array array2 = obj2 as Array;
			IEnumerator enumerator = array.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				if (!CompareUtils.AreObjectsEqual(enumerator.Current, array2.GetValue(num)))
				{
					return false;
				}
				num++;
			}
		}
		else
		{
			foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object value2 = propertyInfo.GetValue(obj1, null);
				object value3 = propertyInfo.GetValue(obj2, null);
				if (!CompareUtils.AreObjectsEqual(value2, value3))
				{
					return false;
				}
			}
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object value4 = fieldInfo.GetValue(obj1);
				object value5 = fieldInfo.GetValue(obj2);
				if (!CompareUtils.AreObjectsEqual(value4, value5))
				{
					return false;
				}
			}
		}
		return true;
	}
}
