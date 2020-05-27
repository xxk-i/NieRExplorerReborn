using System.Collections.Generic;

namespace NieRExplorer.Explorer
{
	public static class PrefixData
	{
		public static readonly Dictionary<string, string> PrefixCollection = new Dictionary<string, string>
		{
			{
				"pl",
				"Player Files"
			},
			{
				"wd",
				"Scene Files"
			},
			{
				"wd1",
				"Scene Files 1"
			},
			{
				"wd2",
				"Scene Files 2"
			},
			{
				"wd3",
				"Scene Files 3"
			},
			{
				"wd4",
				"Scene Files 4"
			},
			{
				"wd5",
				"Scene Files 4"
			},
			{
				"wda",
				"Scene Files A"
			},
			{
				"wp",
				"Weapon Files"
			},
			{
				"um",
				"NPC Files"
			},
			{
				"em",
				"Enemy Files"
			},
			{
				"et",
				"Item Files"
			},
			{
				"ui",
				"UI Files"
			},
			{
				"quest",
				"Quest Files"
			},
			{
				"subtitle",
				"Subtitle Files"
			},
			{
				"novel",
				"Novel Files"
			},
			{
				"event",
				"Event Files"
			},
			{
				"effect",
				"Effect Files"
			}
		};

		public static string GetPrefixType(string prefix)
		{
			if (PrefixCollection.ContainsKey(prefix))
			{
				return PrefixCollection[prefix];
			}
			return prefix;
		}
	}
}
