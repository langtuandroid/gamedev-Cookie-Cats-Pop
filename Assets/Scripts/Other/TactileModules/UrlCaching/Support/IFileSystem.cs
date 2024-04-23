using System;
using System.IO;

namespace TactileModules.UrlCaching.Support
{
	public interface IFileSystem
	{
		bool DirectoryExists(string path);

		DirectoryInfo CreateDirectory(string path);

		bool FileExists(string path);

		void DeleteFile(string path);

		void WriteAllBytes(string path, byte[] bytes);

		byte[] ReadAllBytes(string path);
	}
}
