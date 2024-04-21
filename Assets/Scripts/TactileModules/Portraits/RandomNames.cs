using System;
using UnityEngine;

namespace TactileModules.Portraits
{
	public static class RandomNames
	{
		public static string GetRandomName(int seed)
		{
			int value = seed % RandomNames.nameList.Length;
			return RandomNames.nameList[Mathf.Abs(value)];
		}

		private static readonly string[] nameList = new string[]
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
	}
}
