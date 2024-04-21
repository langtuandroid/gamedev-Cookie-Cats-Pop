using System;
using UnityEngine;

public static class AdjustUtils
{
	public static void Initialize(string appToken, string appSecretId, string appSecret1, string appSecret2, string appSecret3, string appSecret4, string userRegisteredToken, string userCheatedToken, string iapToken, string userIsPayingToken)
	{
		global::AdjustUtils.userRegisteredToken = userRegisteredToken;
		global::AdjustUtils.userCheatedToken = userCheatedToken;
		global::AdjustUtils.iapToken = iapToken;
		global::AdjustUtils.userIsPayingToken = userIsPayingToken;
		
	}

	public static void LogUserRegistered()
	{
		
	}

	public static void LogUserCheated()
	{
		
	}

	public static void LogValidatedPurchase(InAppProduct p)
	{
		
	}

	private static string userRegisteredToken;

	private static string userCheatedToken;

	private static string iapToken;

	private static string userIsPayingToken;
}
