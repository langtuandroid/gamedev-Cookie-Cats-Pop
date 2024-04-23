using System;
using System.Reflection;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager
{
	public static class FeatureHandlerInvokers
	{
		public static FeatureInstanceCustomData NewFeatureInstanceCustomData(IFeatureTypeHandler featureTypeHandler, FeatureData featureData)
		{
			MethodInfo method = featureTypeHandler.GetType().GetMethod("NewFeatureInstanceCustomData");
			FeatureInstanceCustomData result;
			try
			{
				object obj = method.Invoke(featureTypeHandler, new object[]
				{
					featureData
				});
				result = (FeatureInstanceCustomData)obj;
			}
			catch (Exception ex)
			{
				result = null;
			}
			return result;
		}

		public static FeatureTypeCustomData NewFeatureTypeCustomData(IFeatureTypeHandler featureTypeHandler)
		{
			MethodInfo method = featureTypeHandler.GetType().GetMethod("NewFeatureTypeCustomData");
			FeatureTypeCustomData result;
			try
			{
				object obj = method.Invoke(featureTypeHandler, null);
				result = (FeatureTypeCustomData)obj;
			}
			catch (Exception ex)
			{
				result = null;
			}
			return result;
		}

		public static void MergeFeatureInstanceStates(IFeatureTypeHandler featureTypeHandler, ref FeatureInstanceCustomData toMerge, FeatureInstanceCustomData current, FeatureInstanceCustomData cloud)
		{
			Type typeOfFeatureHandlerGeneric = FeatureHandlerInvokers.GetTypeOfFeatureHandlerGeneric(featureTypeHandler, typeof(FeatureInstanceCustomData));
			MethodInfo methodInfo = featureTypeHandler.GetType().GetMethod("MergeFeatureInstanceStates").MakeGenericMethod(new Type[]
			{
				typeOfFeatureHandlerGeneric
			});
			try
			{
				methodInfo.Invoke(featureTypeHandler, new object[]
				{
					toMerge,
					current,
					cloud
				});
			}
			catch (Exception ex)
			{
			}
		}

		public static void MergeFeatureTypeState(IFeatureTypeHandler featureTypeHandler, ref FeatureTypeCustomData toMerge, FeatureTypeCustomData current, FeatureTypeCustomData cloud)
		{
			Type typeOfFeatureHandlerGeneric = FeatureHandlerInvokers.GetTypeOfFeatureHandlerGeneric(featureTypeHandler, typeof(FeatureTypeCustomData));
			MethodInfo methodInfo = featureTypeHandler.GetType().GetMethod("MergeFeatureTypeState").MakeGenericMethod(new Type[]
			{
				typeOfFeatureHandlerGeneric
			});
			try
			{
				methodInfo.Invoke(featureTypeHandler, new object[]
				{
					toMerge,
					current,
					cloud
				});
			}
			catch (Exception ex)
			{
			}
		}

		public static Type GetTypeOfFeatureHandlerGeneric(IFeatureTypeHandler featureTypeHandler, Type type)
		{
			Type type2 = featureTypeHandler.GetType();
			Type type3 = null;
			Type[] interfaces = type2.GetInterfaces();
			foreach (Type type4 in interfaces)
			{
				if (typeof(IFeatureTypeHandler).IsAssignableFrom(type4) && type4 != typeof(IFeatureTypeHandler) && type4.GetGenericArguments().Length >= 3)
				{
					type3 = type4;
					break;
				}
			}
			Type[] genericArguments = type3.GetGenericArguments();
			foreach (Type type5 in genericArguments)
			{
				if (type.IsAssignableFrom(type5))
				{
					return type5;
				}
			}
			throw new Exception(string.Format("FeatureHandler {0} does not implement type {1}", featureTypeHandler, type));
		}
	}
}
