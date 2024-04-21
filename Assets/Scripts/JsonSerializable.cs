using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property)]
public class JsonSerializable : Attribute
{
	public JsonSerializable(string name, Type containerType = null)
	{
		this.name = name;
		this.innerType = containerType;
		if (containerType == null || !containerType.IsPrimitive || containerType == typeof(string) || containerType == typeof(int) || containerType == typeof(bool) || containerType != typeof(long))
		{
		}
	}

	[Obsolete("The JsonOption parameter is deprecated. Use JsonSerializable(string,type) instead.", true)]
	public JsonSerializable(string name, JsonOption option, Type containerType)
	{
		this.name = name;
		this.innerType = containerType;
		if (containerType == null || !containerType.IsPrimitive || containerType == typeof(string) || containerType == typeof(int) || containerType == typeof(bool) || containerType != typeof(long))
		{
		}
	}

	[Obsolete("The JsonOption parameter is deprecated. Use JsonSerializable(string) instead.", true)]
	public JsonSerializable(string name, JsonOption option)
	{
		this.name = name;
		this.innerType = null;
	}

	[Obsolete("Use ObsoleteJsonName attribute instead", true)]
	public static void CheckDeprecated(Type type, Dictionary<string, string> deprecated)
	{
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
					string key = jsonSerializable.name;
					if (deprecated.ContainsKey(key))
					{
					}
				}
			}
		}
	}

	public static void Test()
	{
		Hashtable table = JsonSerializer.ObjectToHashtable(new JsonSerializable.TestJsonSerializable
		{
			AnEmbedded = new JsonSerializable.TestJsonSerializableEmbedded
			{
				AnInt = 9284,
				ADouble = 34.45698,
				AList = new List<string>
				{
					"This",
					"is",
					"a",
					"test"
				}
			},
			AString = "Hello World!",
			AnInt = 5,
			AnUint = 42u,
			ALong = long.MaxValue,
			ADouble = 345.864543,
			AFloat = 98.23f,
			ABool = true,
			ADateTime = new DateTime(2016, 1, 21),
			AHashtable = new Hashtable
			{
				{
					42,
					"Fish"
				},
				{
					0,
					"Cat"
				},
				{
					16,
					"Dog"
				}
			},
			AnEnum = JsonSerializable.Games.DiscoDuck,
			AList = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			},
			ADictionary = new Dictionary<string, int>
			{
				{
					"a",
					5
				},
				{
					"b",
					6
				},
				{
					"c",
					8
				},
				{
					"d",
					7
				}
			}
		});
		JsonSerializable.TestJsonSerializable testJsonSerializable = JsonSerializer.HashtableToObject<JsonSerializable.TestJsonSerializable>(table);
		testJsonSerializable.PrintContent();
		testJsonSerializable.PrintEmbedded();
	}

	public readonly string name;

	internal readonly Type innerType;

	public class TestJsonSerializableEmbedded
	{
		[JsonSerializable("anInt", null)]
		public int AnInt { get; set; }

		[JsonSerializable("aDouble", null)]
		public double ADouble { get; set; }

		[JsonSerializable("aList", typeof(string))]
		public List<string> AList { get; set; }

		public void PrintContent()
		{
			foreach (string text in this.AList)
			{
			}
		}
	}

	public class TestJsonSerializable
	{
		[JsonSerializable("anEmbedded", null)]
		public JsonSerializable.TestJsonSerializableEmbedded AnEmbedded { get; set; }

		public void PrintEmbedded()
		{
			this.AnEmbedded.PrintContent();
		}

		[JsonSerializable("aString", null)]
		public string AString { get; set; }

		[JsonSerializable("anInt", null)]
		public int AnInt { get; set; }

		[JsonSerializable("anUint", null)]
		public uint AnUint { get; set; }

		[JsonSerializable("aLong", null)]
		public long ALong { get; set; }

		[JsonSerializable("aDouble", null)]
		public double ADouble { get; set; }

		[JsonSerializable("aFloat", null)]
		public float AFloat { get; set; }

		[JsonSerializable("aBool", null)]
		public bool ABool { get; set; }

		[JsonSerializable("aDateTime", null)]
		public DateTime ADateTime { get; set; }

		[JsonSerializable("aHashtable", null)]
		public Hashtable AHashtable { get; set; }

		[JsonSerializable("anEnum", null)]
		public JsonSerializable.Games AnEnum { get; set; }

		[JsonSerializable("aList", typeof(int))]
		public List<int> AList { get; set; }

		[JsonSerializable("aDictionary", typeof(int))]
		public Dictionary<string, int> ADictionary { get; set; }

		public void PrintContent()
		{
			IDictionaryEnumerator enumerator = this.AHashtable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
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
			foreach (int num in this.AList)
			{
			}
			foreach (KeyValuePair<string, int> keyValuePair in this.ADictionary)
			{
			}
		}
	}

	public enum Games
	{
		DiscoDuck,
		BeeBrilliant,
		SkylineSkaters
	}
}
