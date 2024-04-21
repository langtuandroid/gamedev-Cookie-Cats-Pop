using System;

namespace TactileModules.RuntimeTools
{
	public class TactileDateTime : ITactileDateTime
	{
		public DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow;
			}
		}
	}
}
