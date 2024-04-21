using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public class JsonSerializer
{
	public static string Encode(object obj)
	{
		Hashtable json = JsonSerializer.ObjectToHashtable(obj);
		return MiniJSON.jsonEncode(json, false, 0);
	}

	public static ArrayList GenericListToArrayList<T>(List<T> list)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle.IsPrimitive || typeFromHandle == typeof(string))
		{
			return new ArrayList(list);
		}
		ArrayList arrayList = new ArrayList(list.Count);
		foreach (T t in list)
		{
			arrayList.Add(JsonSerializer.ObjectToHashtable(t));
		}
		return arrayList;
	}

	public static Hashtable ObjectToHashtable(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj.GetType().IsPrimitive)
		{
		}
		if (obj.GetType() == typeof(Hashtable))
		{
			throw new Exception("You are trying to serialize a hashtable into a hashtable, this won't work");
		}
		JsonSerializer.InvokePreSerialize(obj);
		Hashtable hashtable = new Hashtable();
		Type type = obj.GetType();
		foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (propertyInfo.MemberType == MemberTypes.Property)
			{
				JsonSerializable jsonSerializable = null;
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(JsonSerializable), false);
				int num = 0;
				if (num < customAttributes.Length)
				{
					JsonSerializable jsonSerializable2 = (JsonSerializable)customAttributes[num];
					jsonSerializable = jsonSerializable2;
				}
				if (jsonSerializable != null)
				{
					string name = jsonSerializable.name;
					if (propertyInfo.PropertyType == typeof(string))
					{
						hashtable[name] = (propertyInfo.GetValue(obj, null) as string);
					}
					else if (propertyInfo.PropertyType == typeof(int))
					{
						hashtable[name] = (int)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType == typeof(uint))
					{
						hashtable[name] = (uint)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType == typeof(long))
					{
						hashtable[name] = (long)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType == typeof(double))
					{
						hashtable[name] = (double)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType == typeof(float))
					{
						hashtable[name] = (float)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType == typeof(bool))
					{
						hashtable[name] = (bool)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType == typeof(Type))
					{
						hashtable[name] = ((Type)propertyInfo.GetValue(obj, null)).AssemblyQualifiedName;
					}
					else if (propertyInfo.PropertyType == typeof(DateTime))
					{
						hashtable[name] = ((DateTime)propertyInfo.GetGetMethod(true).Invoke(obj, null)).ToString(CultureInfo.InvariantCulture);
					}
					else if (propertyInfo.PropertyType == typeof(Hashtable))
					{
						hashtable[name] = (propertyInfo.GetValue(obj, null) as Hashtable);
					}
					else if (propertyInfo.PropertyType == typeof(Color))
					{
						hashtable[name] = JsonSerializer.ColorToString((Color)propertyInfo.GetValue(obj, null));
					}
					else if (propertyInfo.PropertyType.IsEnum)
					{
						hashtable[name] = (int)propertyInfo.GetGetMethod(true).Invoke(obj, null);
					}
					else if (propertyInfo.PropertyType.GetInterface(typeof(IList).Name) != null)
					{
						IList list = (IList)propertyInfo.GetValue(obj, null);
						Type typeOfGenericList = JsonSerializer.GetTypeOfGenericList(propertyInfo);
						if (typeOfGenericList != null)
						{
							ArrayList arrayList = new ArrayList();
							if (list != null)
							{
								IEnumerator enumerator = list.GetEnumerator();
								try
								{
									while (enumerator.MoveNext())
									{
										object obj2 = enumerator.Current;
										if (typeOfGenericList == typeof(string))
										{
											arrayList.Add(obj2 as string);
										}
										else if (typeOfGenericList == typeof(int))
										{
											arrayList.Add(obj2);
										}
										else if (typeOfGenericList == typeof(long))
										{
											arrayList.Add(obj2);
										}
										else if (typeOfGenericList == typeof(bool))
										{
											arrayList.Add(obj2);
										}
										else if (typeOfGenericList == typeof(DateTime))
										{
											arrayList.Add(((DateTime)obj2).ToString(CultureInfo.InvariantCulture));
										}
										else
										{
											if (typeOfGenericList.IsPrimitive)
											{
											}
											arrayList.Add(JsonSerializer.ObjectToHashtable(obj2));
										}
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
							}
							hashtable[name] = arrayList;
						}
					}
					else if (propertyInfo.PropertyType.GetInterface(typeof(IDictionary).Name) != null)
					{
						IDictionary dictionary = (IDictionary)propertyInfo.GetValue(obj, null);
						Type typeOfGenericList2 = JsonSerializer.GetTypeOfGenericList(propertyInfo);
						if (typeOfGenericList2 != null)
						{
							Hashtable hashtable2 = new Hashtable();
							if (dictionary != null)
							{
								IDictionaryEnumerator enumerator2 = dictionary.GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										object obj3 = enumerator2.Current;
										DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
										if (typeOfGenericList2 == typeof(string))
										{
											hashtable2[dictionaryEntry.Key.ToString()] = (dictionaryEntry.Value as string);
										}
										else if (typeOfGenericList2 == typeof(int))
										{
											hashtable2[dictionaryEntry.Key.ToString()] = dictionaryEntry.Value;
										}
										else if (typeOfGenericList2 == typeof(long))
										{
											hashtable2[dictionaryEntry.Key.ToString()] = dictionaryEntry.Value;
										}
										else if (typeOfGenericList2 == typeof(bool))
										{
											hashtable2[dictionaryEntry.Key.ToString()] = dictionaryEntry.Value;
										}
										else if (typeOfGenericList2 == typeof(DateTime))
										{
											DateTime dateTime = (DateTime)dictionaryEntry.Value;
											hashtable2[dictionaryEntry.Key.ToString()] = dateTime.ToString(CultureInfo.InvariantCulture);
										}
										else
										{
											if (typeOfGenericList2.IsPrimitive)
											{
											}
											hashtable2[dictionaryEntry.Key.ToString()] = JsonSerializer.ObjectToHashtable(dictionaryEntry.Value);
										}
									}
								}
								finally
								{
									IDisposable disposable2;
									if ((disposable2 = (enumerator2 as IDisposable)) != null)
									{
										disposable2.Dispose();
									}
								}
							}
							hashtable[name] = hashtable2;
						}
					}
					else
					{
						hashtable[name] = JsonSerializer.ObjectToHashtable(propertyInfo.GetValue(obj, null));
					}
				}
			}
		}
		return hashtable;
	}

	public static object Decode(Type objType, string json)
	{
		Hashtable table = (Hashtable)MiniJSON.jsonDecode(json);
		return JsonSerializer.HashtableToObject(objType, table, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
	}

	public static T HashtableToObject<T>(Hashtable table)
	{
		return (T)((object)JsonSerializer.HashtableToObject(typeof(T), table, JsonSerializer.SerializationType.WithPreAndPostCallbacks));
	}

	public static List<T> ArrayListToGenericList<T>(ArrayList array)
	{
		List<T> list = new List<T>(array.Count);
		Type typeFromHandle = typeof(T);
		if (typeFromHandle.IsPrimitive || typeFromHandle == typeof(string))
		{
			IEnumerator enumerator = array.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object value = enumerator.Current;
					list.Add((T)((object)Convert.ChangeType(value, typeFromHandle)));
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
		}
		else
		{
			IEnumerator enumerator2 = array.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj = enumerator2.Current;
					list.Add(JsonSerializer.HashtableToObject<T>((Hashtable)obj));
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}
		return list;
	}

	private static string ColorToString(Color c)
	{
		Color32 color = c;
		return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
		{
			color.r,
			color.g,
			color.b,
			color.a
		});
	}

	private static Color StringToColor(string s)
	{
		Color result;
		try
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				int startIndex = i * 2;
				string value = s.Substring(startIndex, 2);
				array[i] = Convert.ToByte(value, 16);
			}
			result = new Color32(array[0], array[1], array[2], array[3]);
		}
		catch (Exception)
		{
			result = default(Color);
		}
		return result;
	}

	public static bool GetPropertyToTableEntry(object obj, PropertyInfo info, out object result)
	{
		if (info.PropertyType == typeof(string))
		{
			result = (info.GetValue(obj, null) as string);
		}
		else if (info.PropertyType == typeof(int))
		{
			result = info.GetValue(obj, null);
		}
		else if (info.PropertyType == typeof(uint))
		{
			result = info.GetValue(obj, null);
		}
		else if (info.PropertyType == typeof(long))
		{
			result = info.GetValue(obj, null);
		}
		else if (info.PropertyType == typeof(float))
		{
			result = (float)info.GetValue(obj, null);
		}
		else if (info.PropertyType == typeof(double))
		{
			result = (double)info.GetValue(obj, null);
		}
		else if (info.PropertyType == typeof(bool))
		{
			result = (bool)info.GetValue(obj, null);
		}
		else if (info.PropertyType == typeof(DateTime))
		{
			result = ((DateTime)info.GetValue(obj, null)).ToString(CultureInfo.InvariantCulture);
		}
		else if (info.PropertyType == typeof(Color))
		{
			Color c = (Color)info.GetValue(obj, null);
			result = JsonSerializer.ColorToString(c);
		}
		else if (info.PropertyType == typeof(Hashtable))
		{
			result = (info.GetValue(obj, null) as Hashtable);
		}
		else
		{
			if (!info.PropertyType.IsEnum)
			{
				result = null;
				return false;
			}
			result = (int)info.GetValue(obj, null);
		}
		return true;
	}

	public static bool SetPropertyFromTableEntry(object result, PropertyInfo info, string data)
	{
		if (info.PropertyType == typeof(string))
		{
			info.SetValue(result, data, null);
		}
		else if (info.PropertyType == typeof(int))
		{
			int num;
			if (int.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
			{
				info.SetValue(result, num, null);
			}
		}
		else if (info.PropertyType == typeof(uint))
		{
			uint num2;
			if (uint.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out num2))
			{
				info.SetValue(result, num2, null);
			}
		}
		else if (info.PropertyType == typeof(long))
		{
			long num3;
			if (long.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out num3))
			{
				info.SetValue(result, num3, null);
			}
		}
		else if (info.PropertyType == typeof(double))
		{
			double num4;
			if (double.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out num4))
			{
				info.SetValue(result, num4, null);
			}
		}
		else if (info.PropertyType == typeof(float))
		{
			float num5;
			if (float.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out num5))
			{
				info.SetValue(result, num5, null);
			}
		}
		else if (info.PropertyType == typeof(bool))
		{
			bool flag;
			if (bool.TryParse(data, out flag))
			{
				info.SetValue(result, flag, null);
			}
		}
		else if (info.PropertyType == typeof(DateTime))
		{
			DateTime dateTime;
			if (DateTime.TryParse(data, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				info.SetValue(result, dateTime, null);
			}
		}
		else if (info.PropertyType == typeof(Color))
		{
			Color color = JsonSerializer.StringToColor(data);
			info.SetValue(result, color, null);
		}
		else
		{
			if (!info.PropertyType.IsEnum)
			{
				return false;
			}
			Enum value = (Enum)Enum.Parse(info.PropertyType, data);
			info.SetValue(result, value, null);
		}
		return true;
	}

	public static object HashtableToObject(Type objType, Hashtable table, JsonSerializer.SerializationType serializationType = JsonSerializer.SerializationType.WithPreAndPostCallbacks)
	{
		return JsonSerializer.HashtableToObjectInternal(objType, table, serializationType != JsonSerializer.SerializationType.WithoutPreAndPostCallbacks);
	}

	private static object HashtableToObjectInternal(Type objType, Hashtable table, bool callPostDeserialized)
	{
		if (table == null)
		{
			return null;
		}
		object result = Activator.CreateInstance(objType, true);
		return JsonSerializer.HashtableToObject(result, table, callPostDeserialized);
	}

	public static object HashtableToObject(object result, Hashtable table, bool callPostDeserialize)
	{
		if (table == null)
		{
			return result;
		}
		Type type = result.GetType();
		foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (propertyInfo.MemberType == MemberTypes.Property)
			{
				JsonSerializable jsonSerializable = null;
				object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(JsonSerializable), false);
				int num = 0;
				if (num < customAttributes.Length)
				{
					JsonSerializable jsonSerializable2 = (JsonSerializable)customAttributes[num];
					jsonSerializable = jsonSerializable2;
				}
				if (jsonSerializable != null)
				{
					string name = jsonSerializable.name;
					if (table.ContainsKey(name))
					{
						if (propertyInfo.PropertyType == typeof(string))
						{
							propertyInfo.SetValue(result, JsonSerializer.AsString(table[name]), null);
						}
						else if (propertyInfo.PropertyType == typeof(int))
						{
							int num2;
							if (int.TryParse(JsonSerializer.AsString(table[name]), NumberStyles.Any, CultureInfo.InvariantCulture, out num2))
							{
								propertyInfo.SetValue(result, num2, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(uint))
						{
							uint num3;
							if (uint.TryParse(JsonSerializer.AsString(table[name]), NumberStyles.Any, CultureInfo.InvariantCulture, out num3))
							{
								propertyInfo.SetValue(result, num3, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(double))
						{
							double num4;
							if (double.TryParse(JsonSerializer.AsString(table[name]), NumberStyles.Any, CultureInfo.InvariantCulture, out num4))
							{
								propertyInfo.SetValue(result, num4, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(long))
						{
							long num5;
							if (table[name].GetType() == typeof(double))
							{
								propertyInfo.SetValue(result, (long)((double)table[name]), null);
							}
							else if (table[name].GetType() == typeof(long))
							{
								propertyInfo.SetValue(result, (long)table[name], null);
							}
							else if (long.TryParse(JsonSerializer.AsString(table[name]), NumberStyles.Any, CultureInfo.InvariantCulture, out num5))
							{
								propertyInfo.SetValue(result, num5, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(float))
						{
							float num6;
							if (float.TryParse(JsonSerializer.AsString(table[name]), NumberStyles.Any, CultureInfo.InvariantCulture, out num6))
							{
								propertyInfo.SetValue(result, num6, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(bool))
						{
							bool flag;
							if (bool.TryParse(JsonSerializer.AsString(table[name]), out flag))
							{
								propertyInfo.SetValue(result, flag, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(Type))
						{
							string text = JsonSerializer.AsString(table[name]);
							if (!string.IsNullOrEmpty(text))
							{
								Type type2 = Type.GetType(text);
								if (type2 != null)
								{
									propertyInfo.SetValue(result, type2, null);
								}
							}
						}
						else if (propertyInfo.PropertyType == typeof(DateTime))
						{
							DateTime dateTime;
							if (DateTime.TryParse(JsonSerializer.AsString(table[name]), CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
							{
								propertyInfo.SetValue(result, dateTime, null);
							}
						}
						else if (propertyInfo.PropertyType == typeof(Color))
						{
							Color color = JsonSerializer.StringToColor(JsonSerializer.AsString(table[name]));
							propertyInfo.SetValue(result, color, null);
						}
						else if (propertyInfo.PropertyType == typeof(Hashtable))
						{
							propertyInfo.SetValue(result, table[name] as Hashtable, null);
						}
						else if (propertyInfo.PropertyType.IsEnum)
						{
							object obj = table[name];
							string text2 = JsonSerializer.AsString(obj);
							int num7;
							if (int.TryParse(text2, out num7))
							{
								Type underlyingType = Enum.GetUnderlyingType(propertyInfo.PropertyType);
								object value = Convert.ChangeType(text2, underlyingType);
								if (Enum.IsDefined(propertyInfo.PropertyType, value))
								{
									object value2 = Enum.Parse(propertyInfo.PropertyType, text2);
									propertyInfo.SetValue(result, value2, null);
								}
							}
							else if (Enum.IsDefined(propertyInfo.PropertyType, text2))
							{
								object value3 = Enum.Parse(propertyInfo.PropertyType, text2);
								propertyInfo.SetValue(result, value3, null);
							}
							else if (Enum.IsDefined(propertyInfo.PropertyType, "Unknown"))
							{
								object value4 = Enum.Parse(propertyInfo.PropertyType, "Unknown");
								propertyInfo.SetValue(result, value4, null);
							}
						}
						else if (propertyInfo.PropertyType.GetInterface(typeof(IList).Name) != null)
						{
							IList list = Activator.CreateInstance(propertyInfo.PropertyType) as IList;
							propertyInfo.SetValue(result, list, null);
							Type typeOfGenericList = JsonSerializer.GetTypeOfGenericList(propertyInfo);
							if (typeOfGenericList != null)
							{
								object obj2 = table[name];
								if (obj2 != null && obj2 is ArrayList)
								{
									ArrayList arrayList = obj2 as ArrayList;
									IEnumerator enumerator = arrayList.GetEnumerator();
									try
									{
										while (enumerator.MoveNext())
										{
											object obj3 = enumerator.Current;
											if (typeOfGenericList == typeof(string))
											{
												list.Add(obj3 as string);
											}
											else if (typeOfGenericList == typeof(int))
											{
												list.Add(int.Parse(obj3.ToString()));
											}
											else if (typeOfGenericList == typeof(long))
											{
												long num8;
												if (obj3 is double)
												{
													list.Add((long)((double)obj3));
												}
												else if (obj3 is long)
												{
													list.Add((long)obj3);
												}
												else if (long.TryParse(JsonSerializer.AsString(obj3), NumberStyles.Any, CultureInfo.InvariantCulture, out num8))
												{
													list.Add(num8);
												}
											}
											else if (typeOfGenericList == typeof(float))
											{
												list.Add(float.Parse(obj3.ToString()));
											}
											else if (typeOfGenericList == typeof(DateTime))
											{
												DateTime minValue;
												if (!DateTime.TryParse(obj3.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue))
												{
													minValue = DateTime.MinValue;
												}
												list.Add(minValue);
											}
											else
											{
												list.Add(JsonSerializer.HashtableToObjectInternal(typeOfGenericList, (Hashtable)obj3, callPostDeserialize));
											}
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
								}
							}
						}
						else if (propertyInfo.PropertyType.GetInterface(typeof(IDictionary).Name) != null)
						{
							IDictionary dictionary = Activator.CreateInstance(propertyInfo.PropertyType) as IDictionary;
							propertyInfo.SetValue(result, dictionary, null);
							Type typeOfGenericList2 = JsonSerializer.GetTypeOfGenericList(propertyInfo);
							if (typeOfGenericList2 != null)
							{
								object obj4 = table[name];
								if (obj4 != null && obj4 is Hashtable)
								{
									Hashtable hashtable = obj4 as Hashtable;
									IDictionaryEnumerator enumerator2 = hashtable.GetEnumerator();
									try
									{
										while (enumerator2.MoveNext())
										{
											object obj5 = enumerator2.Current;
											DictionaryEntry dictionaryEntry = (DictionaryEntry)obj5;
											if (typeOfGenericList2 == typeof(string))
											{
												dictionary[dictionaryEntry.Key.ToString()] = (dictionaryEntry.Value as string);
											}
											else if (typeOfGenericList2 == typeof(int))
											{
												dictionary[dictionaryEntry.Key.ToString()] = int.Parse(dictionaryEntry.Value.ToString());
											}
											else if (typeOfGenericList2 == typeof(long))
											{
												long num9;
												if (dictionaryEntry.Value is double)
												{
													dictionary[dictionaryEntry.Key.ToString()] = (long)((double)dictionaryEntry.Value);
												}
												else if (dictionaryEntry.Value is long)
												{
													dictionary[dictionaryEntry.Key.ToString()] = (long)dictionaryEntry.Value;
												}
												else if (long.TryParse(JsonSerializer.AsString(dictionaryEntry.Value), NumberStyles.Any, CultureInfo.InvariantCulture, out num9))
												{
													dictionary[dictionaryEntry.Key.ToString()] = num9;
												}
											}
											else if (typeOfGenericList2 == typeof(bool))
											{
												dictionary[dictionaryEntry.Key.ToString()] = bool.Parse(dictionaryEntry.Value.ToString());
											}
											else if (typeOfGenericList2 == typeof(DateTime))
											{
												DateTime minValue2;
												if (!DateTime.TryParse(dictionaryEntry.Value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue2))
												{
													minValue2 = DateTime.MinValue;
												}
												dictionary[dictionaryEntry.Key.ToString()] = minValue2;
											}
											else
											{
												dictionary[dictionaryEntry.Key.ToString()] = JsonSerializer.HashtableToObject(typeOfGenericList2, (Hashtable)dictionaryEntry.Value, JsonSerializer.SerializationType.WithPreAndPostCallbacks);
											}
										}
									}
									finally
									{
										IDisposable disposable2;
										if ((disposable2 = (enumerator2 as IDisposable)) != null)
										{
											disposable2.Dispose();
										}
									}
								}
							}
						}
						else if (propertyInfo.PropertyType == typeof(object))
						{
							object value5 = table[name];
							propertyInfo.SetValue(result, value5, null);
						}
						else
						{
							Hashtable hashtable2 = table[name] as Hashtable;
							if (hashtable2 != null)
							{
								object value6 = JsonSerializer.HashtableToObjectInternal(propertyInfo.PropertyType, hashtable2, callPostDeserialize);
								propertyInfo.SetValue(result, value6, null);
							}
							else
							{
								propertyInfo.SetValue(result, null, null);
							}
						}
					}
				}
			}
		}
		if (callPostDeserialize)
		{
			JsonSerializer.InvokePostDeserialize(result);
		}
		return result;
	}

	private static string AsString(object obj)
	{
		return (obj == null) ? null : obj.ToString();
	}

	private static bool IsSupportedContainer(PropertyInfo info)
	{
		return info.PropertyType.GetInterface(typeof(IList).Name) != null || info.PropertyType.GetInterface(typeof(IDictionary).Name) != null;
	}

	private static Type GetTypeOfGenericList(PropertyInfo info)
	{
		object[] customAttributes = info.GetCustomAttributes(typeof(JsonSerializable), false);
		int num = 0;
		if (num >= customAttributes.Length)
		{
			return null;
		}
		JsonSerializable jsonSerializable = (JsonSerializable)customAttributes[num];
		return jsonSerializable.innerType;
	}

	private static void InvokePostDeserialize(object obj)
	{
		foreach (MethodInfo methodInfo in obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (Attribute.GetCustomAttribute(methodInfo, typeof(JsonPostDeserializeAttribute)) != null)
			{
				try
				{
					methodInfo.Invoke(obj, null);
				}
				catch (Exception ex)
				{
				}
			}
		}
	}

	private static void InvokePreSerialize(object obj)
	{
		foreach (MethodInfo methodInfo in obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (Attribute.GetCustomAttribute(methodInfo, typeof(JsonPreSerializeAttribute)) != null)
			{
				try
				{
					methodInfo.Invoke(obj, null);
				}
				catch (Exception ex)
				{
				}
			}
		}
	}

	public static T Decode<T>(string json)
	{
		return (T)((object)JsonSerializer.Decode(typeof(T), json));
	}

	public enum SerializationType
	{
		WithoutPreAndPostCallbacks,
		WithPreAndPostCallbacks
	}
}
