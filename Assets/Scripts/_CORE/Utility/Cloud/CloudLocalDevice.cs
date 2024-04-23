using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CloudLocalDevice
{
	public static CloudLocalDevice Create(bool pushEnabled, string oneSignalPlayerId)
	{
		CloudLocalDevice cloudLocalDevice = new CloudLocalDevice();
		cloudLocalDevice.UID = SystemInfoHelper.DeviceID;
		cloudLocalDevice.IFA = SystemInfoHelper.IFA;
		cloudLocalDevice.AID = SystemInfoHelper.AID;
		cloudLocalDevice.type = SystemInfoHelper.DeviceType;
		cloudLocalDevice.language = Application.systemLanguage.ToString();
		cloudLocalDevice.gameThriveId = oneSignalPlayerId;
		cloudLocalDevice.pushEnabled = pushEnabled;
		cloudLocalDevice.UpdateHash();
		return cloudLocalDevice;
	}

	private void UpdateHash()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.UID);
		stringBuilder.Append(this.IFA);
		stringBuilder.Append(this.AID);
		stringBuilder.Append(this.type);
		stringBuilder.Append(this.language);
		stringBuilder.Append(this.gameThriveId);
		stringBuilder.Append(this.pushEnabled);
		stringBuilder.Append(1);
		SHA1 sha = new SHA1CryptoServiceProvider();
		byte[] array = sha.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
		StringBuilder stringBuilder2 = new StringBuilder(array.Length * 2);
		foreach (byte b in array)
		{
			stringBuilder2.Append(b.ToString("x2"));
		}
		this.hash = stringBuilder2.ToString();
	}

	public string UID;

	public string IFA;

	public string AID;

	public string type;

	public string language;

	public string gameThriveId;

	public bool pushEnabled;

	public string hash;

	private const int version = 1;
}
