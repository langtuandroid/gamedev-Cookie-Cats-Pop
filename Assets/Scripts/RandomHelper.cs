using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomHelper
{
	public static List<Texture2D> PortraitList
	{
		set
		{
			RandomHelper.PortraitPool = value;
		}
	}

	public static int MainSeed
	{
		set
		{
			RandomHelper.rand = new System.Random(value);
		}
	}

	public static string RandomString(int minLength = 5, int maxLength = 10, string characterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890")
	{
		int num = RandomHelper.rand.Next(minLength, maxLength);
		string text = string.Empty;
		for (int i = 0; i < num; i++)
		{
			text += characterPool[RandomHelper.rand.Next(characterPool.Length)];
		}
		return text;
	}

	public static string RandomName(int seed = -1)
	{
		if (seed == -1)
		{
			seed = RandomHelper.RandomInteger(0, 1000);
		}
		int value = seed % RandomHelper.NamePool.Length;
		return RandomHelper.NamePool[Mathf.Abs(value)];
	}

	public static Texture2D RandomPortrait(int seed = -1)
	{
		if (RandomHelper.PortraitPool == null || RandomHelper.PortraitPool.Count == 0)
		{
			return null;
		}
		if (seed == -1)
		{
			seed = RandomHelper.RandomInteger(0, 1000);
		}
		int value = seed % RandomHelper.PortraitPool.Count;
		return RandomHelper.PortraitPool[Mathf.Abs(value)];
	}

	public static int RandomInteger(int min = 0, int max = 10)
	{
		return RandomHelper.rand.Next(min, max);
	}

	public static List<int> RandomIntegerList(int count, int min, int max)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < count; i++)
		{
			list.Add(RandomHelper.RandomInteger(min, max));
		}
		return list;
	}

	public static float RandomFloat()
	{
		return (float)RandomHelper.rand.NextDouble();
	}

	private static System.Random rand = new System.Random();

	private const string DefaultCharacterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

	private static readonly string[] NamePool = new string[]
	{
		"Sarah",
		"Jacob",
		"Jennifer",
		"Keith",
		"Sally",
		"Eric",
		"Connor",
		"Madeleine",
		"Sam",
		"Una",
		"Steven",
		"Lillian",
		"Claire",
		"Luke",
		"Heather",
		"Ruth",
		"Harry",
		"Austin",
		"Steven",
		"Joan",
		"Luke",
		"Diana",
		"Carl",
		"Adam",
		"Brian",
		"Richard",
		"Cameron",
		"Jane",
		"Christian",
		"Trevor",
		"Sebastian",
		"Frank",
		"Liam",
		"Diana",
		"Arya",
		"Sally",
		"Julian",
		"Natalie",
		"Molly",
		"Ruth",
		"Jason",
		"Grace",
		"Faith",
		"Max",
		"Fiona",
		"Vladimir",
		"Diana",
		"Stephen",
		"Colin",
		"Keith",
		"Emma",
		"David",
		"Sally",
		"Colin",
		"Warren",
		"Alexander",
		"Lucas",
		"Wendy",
		"Nathan",
		"Lillian",
		"Anne",
		"Neil",
		"Sally",
		"Brandon",
		"Caroline",
		"Amelia",
		"Ruth",
		"Ryan",
		"Wanda",
		"Leah",
		"Jason",
		"Alexandra",
		"Jasmine",
		"Bella",
		"Nathan",
		"Sam",
		"Anna",
		"Matt",
		"Brandon",
		"Warren",
		"Oliver",
		"Sue",
		"Karen",
		"Dharma",
		"Rebecca",
		"Rachel",
		"Sean",
		"Suzy",
		"Amelia",
		"Oliver",
		"Benjamin",
		"Elizabeth",
		"Diana",
		"Eleanor",
		"Madonna",
		"Lisa",
		"Dylan",
		"John",
		"Gordon",
		"Jason",
		"Dorothy",
		"Amanda",
		"Dorothy",
		"Bella",
		"Christian",
		"Wanda",
		"Sarah",
		"Wanda",
		"Pippa",
		"Karen",
		"Matt",
		"Angela",
		"Amelia",
		"Michelle",
		"Karen",
		"Pippa",
		"Penelope",
		"Max",
		"Benjamin",
		"Joseph",
		"Line",
		"Alexander",
		"Michael",
		"Heather",
		"Stewart",
		"Michael",
		"Boris",
		"Dorothy",
		"Sam",
		"Wen",
		"Faith",
		"Jennifer",
		"Henrik",
		"Sofie",
		"Tracey",
		"Lauren",
		"Yvonne",
		"Chloe",
		"Alison",
		"Lucas",
		"Chloe",
		"Stewart",
		"Nicola",
		"Jane",
		"John",
		"Thomas",
		"Cameron",
		"Claire",
		"Joan",
		"Claire",
		"Jan",
		"Hannah",
		"Connor",
		"Abigail",
		"Heather",
		"Owen",
		"Emma",
		"Joshua",
		"Wanda",
		"Rachel",
		"Amanda",
		"Alexander",
		"Eric",
		"Dylan",
		"Andrew",
		"Trevor",
		"Robert",
		"Eric",
		"Brian",
		"Jan",
		"Pippa",
		"Brandon",
		"Dan",
		"Amelia",
		"Lily",
		"Nathan",
		"Rebecca",
		"Sam",
		"Hannah",
		"Carol",
		"Bernadette",
		"Jake",
		"Jennifer",
		"Stephen",
		"Olivia",
		"Nathan",
		"David",
		"Jack",
		"Colin",
		"Dan",
		"Mary",
		"Sue",
		"Dan",
		"Michael",
		"Chloe",
		"Lillian",
		"Maria",
		"Leonard",
		"William"
	};

	private static List<Texture2D> PortraitPool = new List<Texture2D>();
}
