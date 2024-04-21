using System;
using UnityEngine;

public class DeviceUtilsAndroid
{
    static DeviceUtilsAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (new AndroidJavaClass("android.util.DisplayMetrics"))
        {
            DeviceUtilsAndroid.metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics", new object[0]);
        }
        using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getWindowManager", new object[0]))
                {
                    using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getDefaultDisplay", new object[0]))
                    {
                        androidJavaObject2.Call("getMetrics", new object[]
                        {
                            DeviceUtilsAndroid.metricsInstance
                        });
                    }
                }
            }
        }
#endif
    }

    public static float getScreenInches()
    {
        int num = DeviceUtilsAndroid.metricsInstance.Get<int>("heightPixels");
        int num2 = DeviceUtilsAndroid.metricsInstance.Get<int>("widthPixels");
        float num3 = DeviceUtilsAndroid.metricsInstance.Get<float>("xdpi");
        float num4 = DeviceUtilsAndroid.metricsInstance.Get<float>("ydpi");
        float num5 = (float)num2 / num3;
        float num6 = (float)num / num4;
        return Mathf.Sqrt(num5 * num5 + num6 * num6);
    }

    public static float getDensity()
    {
        //return 1.7f;
#if UNITY_ANDROID && !UNITY_EDITOR
        return DeviceUtilsAndroid.metricsInstance.Get<float>("density");
#endif
#if UNITY_EDITOR
        return 1.7f;
#endif

    }

    public static int getRotation()
    {
        int result = 0;
        using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getWindowManager", new object[0]))
                {
                    using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getDefaultDisplay", new object[0]))
                    {
                        result = androidJavaObject2.Call<int>("getRotation", new object[0]);
                    }
                }
            }
        }
        return result;
    }

    public static int getAccelerometerRotation()
    {
        return DeviceUtilsAndroid.getSystemSetting("accelerometer_rotation", 0);
    }

    public static int getUserRotation(int defaultValue)
    {
        return DeviceUtilsAndroid.getSystemSetting("user_rotation", defaultValue);
    }

    public static int getSystemSetting(string settingName, int defaultValue = 0)
    {
        int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.provider.Settings$System"))
        {
            using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]))
                    {
                        result = androidJavaClass.CallStatic<int>("getInt", new object[]
                        {
                            androidJavaObject,
                            settingName,
                            defaultValue
                        });
                    }
                }
            }
        }

#endif
        return result;
    }

    private static AndroidJavaObject metricsInstance;
}
