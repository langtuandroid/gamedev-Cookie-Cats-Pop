using System;
using UnityEngine;

public class AmazonJavaWrapper : IDisposable
{
    public AmazonJavaWrapper()
    {
    }

    public AmazonJavaWrapper(AndroidJavaObject o)
    {
        this.setAndroidJavaObject(o);
    }

    public AndroidJavaObject getJavaObject()
    {
        return this.jo;
    }

    public void setAndroidJavaObject(AndroidJavaObject o)
    {
        this.jo = o;
    }

    public IntPtr GetRawObject()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return this.jo.GetRawObject();
        }
        return (IntPtr)0;
    }

    public IntPtr GetRawClass()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return this.jo.GetRawClass();
        }
        return (IntPtr)0;
    }

    public void Set<FieldType>(string fieldName, FieldType type)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            this.jo.Set<FieldType>(fieldName, type);
        }
    }

    public FieldType Get<FieldType>(string fieldName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return this.jo.Get<FieldType>(fieldName);
        }
        return default(FieldType);
    }

    public void SetStatic<FieldType>(string fieldName, FieldType type)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            this.jo.SetStatic<FieldType>(fieldName, type);
        }
    }

    public FieldType GetStatic<FieldType>(string fieldName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return this.jo.GetStatic<FieldType>(fieldName);
        }
        return default(FieldType);
    }

    public void CallStatic(string method, params object[] args)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJNI.PushLocalFrame(args.Length + 1);
            this.jo.CallStatic(method, args);
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        }
    }

    public void Call(string method, params object[] args)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJNI.PushLocalFrame(args.Length + 1);
            this.jo.Call(method, args);
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        }
    }

    public ReturnType CallStatic<ReturnType>(string method, params object[] args)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJNI.PushLocalFrame(args.Length + 1);
            ReturnType result = this.jo.CallStatic<ReturnType>(method, args);
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
            return result;
        }
        return default(ReturnType);
    }

    public ReturnType Call<ReturnType>(string method, params object[] args)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJNI.PushLocalFrame(args.Length + 1);
            ReturnType result = this.jo.Call<ReturnType>(method, args);
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
            return result;
        }
        return default(ReturnType);
    }

    public void Dispose()
    {
        if (this.jo != null)
        {
            this.jo.Dispose();
        }
    }

    private AndroidJavaObject jo;
}
