using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.Ads.Analytics
{
	[Serializable]
	public struct AdGroupContext
	{
		[UsedImplicitly]
		public static List<string> GetIdentifiers()
		{
			List<string> list;
			List<string> result;
			CollectionExtensions.GetConstNamesAndValues<AdGroupContext, string>(out list, out result);
			return result;
		}

		public static implicit operator AdGroupContext(string val)
		{
			return new AdGroupContext
			{
				value = val
			};
		}

		public static implicit operator string(AdGroupContext t)
		{
			return t.value;
		}

		public override string ToString()
		{
			return this.value;
		}

		public static bool operator ==(AdGroupContext a, AdGroupContext b)
		{
			return object.ReferenceEquals(a, b) || ((object)a != (object)null && (object)b != (object)null && a.value == b.value);
		}

		public static bool operator !=(AdGroupContext a, AdGroupContext b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return obj is AdGroupContext && this.value == ((AdGroupContext)obj).value;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public const string POPUP = "Popup";

		public const string MAIN_MAP_BUTTON = "MainMapButton";

		public const string TACTILE_HUB = "TactileHub";

		public const string OUT_OF_LIVES = "OutOfLives";

		public const string SHOP_VIEW = "Shop";

		public const string LEVEL_OBJECTIVE = "LevelObjective";

		public const string OUT_OF_MOVES = "OutOfMoves";

		public const string DEBUG = "Debug";

		[SerializeField]
		private string value;
	}
}
