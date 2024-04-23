using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using com.amazon.device.iap.cpt.json;
using UnityEngine;

namespace com.amazon.device.iap.cpt
{
	public abstract class AmazonIapV2Impl : MonoBehaviour, IAmazonIapV2
	{
		private AmazonIapV2Impl()
		{
		}

		public static IAmazonIapV2 Instance
		{
			get
			{
				return AmazonIapV2Impl.Builder.instance;
			}
		}

		public static void callback(string jsonMessage)
		{
			try
			{
				AmazonIapV2Impl.logger.Debug("Executing callback");
				Dictionary<string, object> dictionary = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				string callerId = dictionary["callerId"] as string;
				Dictionary<string, object> response = dictionary["response"] as Dictionary<string, object>;
				AmazonIapV2Impl.callbackCaller(response, callerId);
			}
			catch (KeyNotFoundException inner)
			{
				AmazonIapV2Impl.logger.Debug("callerId not found in callback");
				throw new AmazonException("Internal Error: Unknown callback id", inner);
			}
			catch (AmazonException ex)
			{
				AmazonIapV2Impl.logger.Debug("Async call threw exception: " + ex.ToString());
			}
		}

		private static void callbackCaller(Dictionary<string, object> response, string callerId)
		{
			IDelegator delegator = null;
			try
			{
				Jsonable.CheckForErrors(response);
				object obj = AmazonIapV2Impl.callbackLock;
				lock (obj)
				{
					delegator = AmazonIapV2Impl.callbackDictionary[callerId];
					AmazonIapV2Impl.callbackDictionary.Remove(callerId);
					delegator.ExecuteSuccess(response);
				}
			}
			catch (AmazonException e)
			{
				object obj2 = AmazonIapV2Impl.callbackLock;
				lock (obj2)
				{
					if (delegator == null)
					{
						delegator = AmazonIapV2Impl.callbackDictionary[callerId];
					}
					AmazonIapV2Impl.callbackDictionary.Remove(callerId);
					delegator.ExecuteError(e);
				}
			}
		}

		public static void FireEvent(string jsonMessage)
		{
			try
			{
				Dictionary<string, object> dictionary = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				string key = dictionary["eventId"] as string;
				Dictionary<string, object> dictionary2 = null;
				if (dictionary.ContainsKey("response"))
				{
					dictionary2 = (dictionary["response"] as Dictionary<string, object>);
					Jsonable.CheckForErrors(dictionary2);
				}
				object obj = AmazonIapV2Impl.eventLock;
				lock (obj)
				{
					foreach (IDelegator delegator in AmazonIapV2Impl.eventListeners[key])
					{
						if (dictionary2 != null)
						{
							delegator.ExecuteSuccess(dictionary2);
						}
						else
						{
							delegator.ExecuteSuccess();
						}
					}
				}
			}
			catch (AmazonException ex)
			{
				AmazonIapV2Impl.logger.Debug("Event call threw exception: " + ex.ToString());
			}
		}

		public abstract RequestOutput GetUserData();

		public abstract RequestOutput Purchase(SkuInput skuInput);

		public abstract RequestOutput GetProductData(SkusInput skusInput);

		public abstract RequestOutput GetPurchaseUpdates(ResetInput resetInput);

		public abstract void NotifyFulfillment(NotifyFulfillmentInput notifyFulfillmentInput);

		public abstract void UnityFireEvent(string jsonMessage);

		public abstract void AddGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate);

		public abstract void RemoveGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate);

		public abstract void AddPurchaseResponseListener(PurchaseResponseDelegate responseDelegate);

		public abstract void RemovePurchaseResponseListener(PurchaseResponseDelegate responseDelegate);

		public abstract void AddGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate);

		public abstract void RemoveGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate);

		public abstract void AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate);

		public abstract void RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate);

		private static AmazonLogger logger;

		private static readonly Dictionary<string, IDelegator> callbackDictionary = new Dictionary<string, IDelegator>();

		private static readonly object callbackLock = new object();

		private static readonly Dictionary<string, List<IDelegator>> eventListeners = new Dictionary<string, List<IDelegator>>();

		private static readonly object eventLock = new object();

		private abstract class AmazonIapV2Base : AmazonIapV2Impl
		{
			public AmazonIapV2Base()
			{
				AmazonIapV2Impl.logger = new AmazonLogger(base.GetType().Name);
			}

			protected void Start()
			{
				if (AmazonIapV2Impl.AmazonIapV2Base.startCalled)
				{
					return;
				}
				object obj = AmazonIapV2Impl.AmazonIapV2Base.startLock;
				lock (obj)
				{
					if (!AmazonIapV2Impl.AmazonIapV2Base.startCalled)
					{
						this.Init();
						this.RegisterCallback();
						this.RegisterEventListener();
						this.RegisterCrossPlatformTool();
						AmazonIapV2Impl.AmazonIapV2Base.startCalled = true;
					}
				}
			}

			protected abstract void Init();

			protected abstract void RegisterCallback();

			protected abstract void RegisterEventListener();

			protected abstract void RegisterCrossPlatformTool();

			public override void UnityFireEvent(string jsonMessage)
			{
				AmazonIapV2Impl.FireEvent(jsonMessage);
			}

			public override RequestOutput GetUserData()
			{
				this.Start();
				return RequestOutput.CreateFromJson(this.GetUserDataJson("{}"));
			}

			private string GetUserDataJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = this.NativeGetUserDataJson(jsonMessage);
				stopwatch.Stop();
				AmazonIapV2Impl.logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeGetUserDataJson(string jsonMessage);

			public override RequestOutput Purchase(SkuInput skuInput)
			{
				this.Start();
				return RequestOutput.CreateFromJson(this.PurchaseJson(skuInput.ToJson()));
			}

			private string PurchaseJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = this.NativePurchaseJson(jsonMessage);
				stopwatch.Stop();
				AmazonIapV2Impl.logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativePurchaseJson(string jsonMessage);

			public override RequestOutput GetProductData(SkusInput skusInput)
			{
				this.Start();
				return RequestOutput.CreateFromJson(this.GetProductDataJson(skusInput.ToJson()));
			}

			private string GetProductDataJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = this.NativeGetProductDataJson(jsonMessage);
				stopwatch.Stop();
				AmazonIapV2Impl.logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeGetProductDataJson(string jsonMessage);

			public override RequestOutput GetPurchaseUpdates(ResetInput resetInput)
			{
				this.Start();
				return RequestOutput.CreateFromJson(this.GetPurchaseUpdatesJson(resetInput.ToJson()));
			}

			private string GetPurchaseUpdatesJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = this.NativeGetPurchaseUpdatesJson(jsonMessage);
				stopwatch.Stop();
				AmazonIapV2Impl.logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeGetPurchaseUpdatesJson(string jsonMessage);

			public override void NotifyFulfillment(NotifyFulfillmentInput notifyFulfillmentInput)
			{
				this.Start();
				Jsonable.CheckForErrors(Json.Deserialize(this.NotifyFulfillmentJson(notifyFulfillmentInput.ToJson())) as Dictionary<string, object>);
			}

			private string NotifyFulfillmentJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = this.NativeNotifyFulfillmentJson(jsonMessage);
				stopwatch.Stop();
				AmazonIapV2Impl.logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeNotifyFulfillmentJson(string jsonMessage);

			public override void AddGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "getUserDataResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						AmazonIapV2Impl.eventListeners[key].Add(new GetUserDataResponseDelegator(responseDelegate));
					}
					else
					{
						List<IDelegator> list = new List<IDelegator>();
						list.Add(new GetUserDataResponseDelegator(responseDelegate));
						AmazonIapV2Impl.eventListeners.Add(key, list);
					}
				}
			}

			public override void RemoveGetUserDataResponseListener(GetUserDataResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "getUserDataResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						foreach (IDelegator delegator in AmazonIapV2Impl.eventListeners[key])
						{
							GetUserDataResponseDelegator getUserDataResponseDelegator = (GetUserDataResponseDelegator)delegator;
							if (getUserDataResponseDelegator.responseDelegate == responseDelegate)
							{
								AmazonIapV2Impl.eventListeners[key].Remove(getUserDataResponseDelegator);
								break;
							}
						}
					}
				}
			}

			public override void AddPurchaseResponseListener(PurchaseResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "purchaseResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						AmazonIapV2Impl.eventListeners[key].Add(new PurchaseResponseDelegator(responseDelegate));
					}
					else
					{
						List<IDelegator> list = new List<IDelegator>();
						list.Add(new PurchaseResponseDelegator(responseDelegate));
						AmazonIapV2Impl.eventListeners.Add(key, list);
					}
				}
			}

			public override void RemovePurchaseResponseListener(PurchaseResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "purchaseResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						foreach (IDelegator delegator in AmazonIapV2Impl.eventListeners[key])
						{
							PurchaseResponseDelegator purchaseResponseDelegator = (PurchaseResponseDelegator)delegator;
							if (purchaseResponseDelegator.responseDelegate == responseDelegate)
							{
								AmazonIapV2Impl.eventListeners[key].Remove(purchaseResponseDelegator);
								break;
							}
						}
					}
				}
			}

			public override void AddGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "getProductDataResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						AmazonIapV2Impl.eventListeners[key].Add(new GetProductDataResponseDelegator(responseDelegate));
					}
					else
					{
						List<IDelegator> list = new List<IDelegator>();
						list.Add(new GetProductDataResponseDelegator(responseDelegate));
						AmazonIapV2Impl.eventListeners.Add(key, list);
					}
				}
			}

			public override void RemoveGetProductDataResponseListener(GetProductDataResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "getProductDataResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						foreach (IDelegator delegator in AmazonIapV2Impl.eventListeners[key])
						{
							GetProductDataResponseDelegator getProductDataResponseDelegator = (GetProductDataResponseDelegator)delegator;
							if (getProductDataResponseDelegator.responseDelegate == responseDelegate)
							{
								AmazonIapV2Impl.eventListeners[key].Remove(getProductDataResponseDelegator);
								break;
							}
						}
					}
				}
			}

			public override void AddGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "getPurchaseUpdatesResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						AmazonIapV2Impl.eventListeners[key].Add(new GetPurchaseUpdatesResponseDelegator(responseDelegate));
					}
					else
					{
						List<IDelegator> list = new List<IDelegator>();
						list.Add(new GetPurchaseUpdatesResponseDelegator(responseDelegate));
						AmazonIapV2Impl.eventListeners.Add(key, list);
					}
				}
			}

			public override void RemoveGetPurchaseUpdatesResponseListener(GetPurchaseUpdatesResponseDelegate responseDelegate)
			{
				this.Start();
				string key = "getPurchaseUpdatesResponse";
				object eventLock = AmazonIapV2Impl.eventLock;
				lock (eventLock)
				{
					if (AmazonIapV2Impl.eventListeners.ContainsKey(key))
					{
						foreach (IDelegator delegator in AmazonIapV2Impl.eventListeners[key])
						{
							GetPurchaseUpdatesResponseDelegator getPurchaseUpdatesResponseDelegator = (GetPurchaseUpdatesResponseDelegator)delegator;
							if (getPurchaseUpdatesResponseDelegator.responseDelegate == responseDelegate)
							{
								AmazonIapV2Impl.eventListeners[key].Remove(getPurchaseUpdatesResponseDelegator);
								break;
							}
						}
					}
				}
			}

			private static readonly object startLock = new object();

			private static volatile bool startCalled = false;
		}

		private class AmazonIapV2Default : AmazonIapV2Impl.AmazonIapV2Base
		{
			protected override void Init()
			{
			}

			protected override void RegisterCallback()
			{
			}

			protected override void RegisterEventListener()
			{
			}

			protected override void RegisterCrossPlatformTool()
			{
			}

			protected override string NativeGetUserDataJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativePurchaseJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeGetProductDataJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeGetPurchaseUpdatesJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeNotifyFulfillmentJson(string jsonMessage)
			{
				return "{}";
			}
		}

		private abstract class AmazonIapV2DelegatesBase : AmazonIapV2Impl.AmazonIapV2Base
		{
			protected override void Init()
			{
				this.NativeInit();
			}

			protected override void RegisterCallback()
			{
				this.callbackDelegate = new AmazonIapV2Impl.CallbackDelegate(AmazonIapV2Impl.callback);
				this.NativeRegisterCallback(this.callbackDelegate);
			}

			protected override void RegisterEventListener()
			{
				this.eventDelegate = new AmazonIapV2Impl.CallbackDelegate(AmazonIapV2Impl.FireEvent);
				this.NativeRegisterEventListener(this.eventDelegate);
			}

			protected override void RegisterCrossPlatformTool()
			{
				this.NativeRegisterCrossPlatformTool("XAMARIN");
			}

			public override void UnityFireEvent(string jsonMessage)
			{
				throw new NotSupportedException("UnityFireEvent is not supported");
			}

			protected abstract void NativeInit();

			protected abstract void NativeRegisterCallback(AmazonIapV2Impl.CallbackDelegate callback);

			protected abstract void NativeRegisterEventListener(AmazonIapV2Impl.CallbackDelegate callback);

			protected abstract void NativeRegisterCrossPlatformTool(string crossPlatformTool);

			private const string CrossPlatformTool = "XAMARIN";

			protected AmazonIapV2Impl.CallbackDelegate callbackDelegate;

			protected AmazonIapV2Impl.CallbackDelegate eventDelegate;
		}

		protected delegate void CallbackDelegate(string jsonMessage);

		private class Builder
		{
			internal static readonly IAmazonIapV2 instance = AmazonIapV2Impl.AmazonIapV2UnityAndroid.Instance;
		}

		private class AmazonIapV2UnityAndroid : AmazonIapV2Impl.AmazonIapV2UnityBase
		{
			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeRegisterCallbackGameObject(string name);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeInit();

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeGetUserDataJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativePurchaseJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeGetProductDataJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeGetPurchaseUpdatesJson(string jsonMessage);

			[DllImport("AmazonIapV2Bridge")]
			private static extern string nativeNotifyFulfillmentJson(string jsonMessage);

			public new static AmazonIapV2Impl.AmazonIapV2UnityAndroid Instance
			{
				get
				{
					return AmazonIapV2Impl.AmazonIapV2UnityBase.getInstance<AmazonIapV2Impl.AmazonIapV2UnityAndroid>();
				}
			}

			protected override void NativeInit()
			{
				AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeInit();
			}

			protected override void RegisterCallback()
			{
				AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeRegisterCallbackGameObject(base.gameObject.name);
			}

			protected override void RegisterEventListener()
			{
				AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeRegisterCallbackGameObject(base.gameObject.name);
			}

			protected override void NativeRegisterCrossPlatformTool(string crossPlatformTool)
			{
			}

			protected override string NativeGetUserDataJson(string jsonMessage)
			{
				return AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeGetUserDataJson(jsonMessage);
			}

			protected override string NativePurchaseJson(string jsonMessage)
			{
				return AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativePurchaseJson(jsonMessage);
			}

			protected override string NativeGetProductDataJson(string jsonMessage)
			{
				return AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeGetProductDataJson(jsonMessage);
			}

			protected override string NativeGetPurchaseUpdatesJson(string jsonMessage)
			{
				return AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeGetPurchaseUpdatesJson(jsonMessage);
			}

			protected override string NativeNotifyFulfillmentJson(string jsonMessage)
			{
				return AmazonIapV2Impl.AmazonIapV2UnityAndroid.nativeNotifyFulfillmentJson(jsonMessage);
			}
		}

		private abstract class AmazonIapV2UnityBase : AmazonIapV2Impl.AmazonIapV2Base
		{
			public static T getInstance<T>() where T : AmazonIapV2Impl.AmazonIapV2UnityBase
			{
				if (AmazonIapV2Impl.AmazonIapV2UnityBase.quit)
				{
					return (T)((object)null);
				}
				if (AmazonIapV2Impl.AmazonIapV2UnityBase.instance != null)
				{
					return (T)((object)AmazonIapV2Impl.AmazonIapV2UnityBase.instance);
				}
				object obj = AmazonIapV2Impl.AmazonIapV2UnityBase.initLock;
				T result;
				lock (obj)
				{
					Type typeFromHandle = typeof(T);
					AmazonIapV2Impl.AmazonIapV2UnityBase.assertTrue(AmazonIapV2Impl.AmazonIapV2UnityBase.instance == null || (AmazonIapV2Impl.AmazonIapV2UnityBase.instance != null && AmazonIapV2Impl.AmazonIapV2UnityBase.instanceType == typeFromHandle), "Only 1 instance of 1 subtype of AmazonIapV2UnityBase can exist.");
					if (AmazonIapV2Impl.AmazonIapV2UnityBase.instance == null)
					{
						AmazonIapV2Impl.AmazonIapV2UnityBase.instanceType = typeFromHandle;
						GameObject gameObject = new GameObject();
						AmazonIapV2Impl.AmazonIapV2UnityBase.instance = gameObject.AddComponent<T>();
						gameObject.name = typeFromHandle.ToString() + "_Singleton";
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
					result = (T)((object)AmazonIapV2Impl.AmazonIapV2UnityBase.instance);
				}
				return result;
			}

			public void OnDestroy()
			{
				AmazonIapV2Impl.AmazonIapV2UnityBase.quit = true;
			}

			private static void assertTrue(bool statement, string errorMessage)
			{
				if (!statement)
				{
					throw new AmazonException("FATAL: An internal error occurred", new InvalidOperationException(errorMessage));
				}
			}

			protected override void Init()
			{
				this.NativeInit();
			}

			protected override void RegisterCrossPlatformTool()
			{
				this.NativeRegisterCrossPlatformTool("UNITY");
			}

			protected abstract void NativeInit();

			protected abstract void NativeRegisterCrossPlatformTool(string crossPlatformTool);

			private const string CrossPlatformTool = "UNITY";

			private static AmazonIapV2Impl.AmazonIapV2UnityBase instance;

			private static Type instanceType;

			private static volatile bool quit = false;

			private static object initLock = new object();
		}
	}
}
