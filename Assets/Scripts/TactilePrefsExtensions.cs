using System;
using System.Security.Cryptography;
using System.Text;

public static class TactilePrefsExtensions
{
	public static string TactilePlayerPrefsHash(this string value, string salt)
	{
		HMACSHA256 hmacsha = new HMACSHA256(Encoding.UTF8.GetBytes(Constants.SHARED_GAME_SECRET));
		byte[] array = hmacsha.ComputeHash(Encoding.UTF8.GetBytes(value + salt));
		StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static byte[] TactilePlayerPrefsCryptKeyHash(this string value)
	{
		return null;
	}
}
