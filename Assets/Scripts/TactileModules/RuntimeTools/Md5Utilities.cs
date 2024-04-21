using System;
using System.Security.Cryptography;
using System.Text;

namespace TactileModules.RuntimeTools
{
	public static class Md5Utilities
	{
		public static string CreateMd5(string input)
		{
			string result;
			using (MD5 md = MD5.Create())
			{
				byte[] bytes = Encoding.ASCII.GetBytes(input);
				byte[] array = md.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("X2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
