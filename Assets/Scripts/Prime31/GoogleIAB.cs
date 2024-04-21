using System;
using UnityEngine;

namespace Prime31
{
	public class GoogleIAB
	{
		static GoogleIAB()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.GoogleIABPlugin"))
			//{
			//	GoogleIAB._plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			//}
		}

		public static void enableLogging(bool shouldEnable)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			if (shouldEnable)
			{
			}
			//GoogleIAB._plugin.Call("enableLogging", new object[]
			//{
			//	shouldEnable
			//});
		}

		public static void setAutoVerifySignatures(bool shouldVerify)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("setAutoVerifySignatures", new object[]
			//{
			//	shouldVerify
			//});
		}

		public static void init(string publicKey)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("init", new object[]
			//{
			//	publicKey
			//});
		}

		public static void unbindService()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("unbindService", new object[0]);
		}

		public static bool areSubscriptionsSupported()
		{
			return Application.platform == RuntimePlatform.Android;
		}

		public static void queryInventory(string[] skus)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("queryInventory", new object[]
			//{
			//	skus
			//});
		}

		public static void purchaseProduct(string sku)
		{
			GoogleIAB.purchaseProduct(sku, string.Empty);
		}

		public static void purchaseProduct(string sku, string developerPayload)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("purchaseProduct", new object[]
			//{
			//	sku,
			//	developerPayload
			//});
		}

		public static void consumeProduct(string sku)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("consumeProduct", new object[]
			//{
			//	sku
			//});
		}

		public static void consumeProducts(string[] skus)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//GoogleIAB._plugin.Call("consumeProducts", new object[]
			//{
			//	skus
			//});
		}

		private static AndroidJavaObject _plugin;
	}
}
