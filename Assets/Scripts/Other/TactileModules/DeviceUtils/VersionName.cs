using System;

namespace TactileModules.DeviceUtils
{
	public class VersionName
	{
		public VersionName(string version)
		{
			this.version = version;
			this.Parse();
		}

		public int MajorVersion { get; private set; }

		public int MinorVersion { get; private set; }

		public int PatchVersion { get; private set; }

		private void Parse()
		{
			char c = '.';
			string[] array = this.version.Split(new char[]
			{
				c
			});
			int num = array.Length;
			this.MajorVersion = ((num < 1) ? 0 : this.ParseComponent(array[0]));
			this.MinorVersion = ((num < 2) ? 0 : this.ParseComponent(array[1]));
			this.PatchVersion = ((num < 3) ? 0 : this.ParseComponent(array[2]));
		}

		private int ParseComponent(string component)
		{
			int result = 0;
			int.TryParse(component, out result);
			return result;
		}

		public bool Equals(VersionName other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.PatchVersion == other.PatchVersion;
		}

		public bool IsGreaterThan(VersionName other)
		{
			return this.MajorVersion > other.MajorVersion || this.MinorVersion > other.MinorVersion || this.PatchVersion > other.PatchVersion;
		}

		public bool IsGreaterThanOrEquals(VersionName other)
		{
			return this.IsGreaterThan(other) || this.Equals(other);
		}

		public string GetRaw()
		{
			return this.version;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.MajorVersion,
				".",
				this.MinorVersion,
				".",
				this.PatchVersion
			});
		}

		private readonly string version;
	}
}
