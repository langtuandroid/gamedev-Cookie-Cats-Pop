using System;
using System.IO;

namespace TactileModules.UrlCaching.Support
{
	public class FileSystem : IFileSystem
	{
		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public DirectoryInfo CreateDirectory(string path)
		{
			return Directory.CreateDirectory(path);
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public void DeleteFile(string path)
		{
			File.Delete(path);
		}

		public void WriteAllBytes(string path, byte[] bytes)
		{
			File.WriteAllBytes(path, bytes);
		}

		public byte[] ReadAllBytes(string path)
		{
			return File.ReadAllBytes(path);
		}
	}
}
