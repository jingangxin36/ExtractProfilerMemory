using System;
using System.Reflection;

public class Dynamic
{
    private const BindingFlags PublicInstanceFieldFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField;

    private const BindingFlags PrivateInstanceFieldFlag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField;

    private const BindingFlags PrivateStaticFieldFlag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField;

    private const BindingFlags PublicInstanceMethodFlag = BindingFlags.Instance | BindingFlags.Public;

    private const BindingFlags PrivateInstanceMethodFlag = BindingFlags.Instance | BindingFlags.NonPublic;

    public readonly Type InnerType;


    public object InnerObject { get; private set; }

    public Dynamic(Type innerType)
    {
        InnerType = innerType;
    }

    public Dynamic(object obj)
    {
        if (null == obj) return;
        InnerType = obj.GetType();
        InnerObject = obj;
    }

    public static void CopyFrom(object dst, object src, BindingFlags flags)
    {
        if (dst == null || src == null) return;
        var srcType = src.GetType();
        var dstType = dst.GetType();
        var dstFields = dstType.GetFields(flags);
        var dstArray = dstFields;
        foreach (var dstFieldInfo in dstArray)
        {
            var srcFieldInfo = srcType.GetField(dstFieldInfo.Name, flags);
            if (srcFieldInfo != null && dstFieldInfo.FieldType == srcFieldInfo.FieldType)
            {
                dstFieldInfo.SetValue(dst, srcFieldInfo.GetValue(src));
            }
        }
    }

    public void SetObject(object obj)
    {
        if (obj.GetType() == InnerType)
        {
            InnerObject = obj;
        }
    }

    public object PrivateStaticField(string fieldName)
    {
        return _GetFiled(fieldName, PrivateStaticFieldFlag);
    }

    public T PrivateStaticField<T>(string fieldName) where T : class
    {
        return PrivateStaticField(fieldName) as T;
    }

    public object PrivateInstanceField(string fieldName)
    {
        return _GetFiled(fieldName, PrivateInstanceFieldFlag);
    }

    public T PrivateInstanceField<T>(string fieldName) where T : class
    {
        return PrivateInstanceField(fieldName) as T;
    }

    public object PublicInstanceField(string fieldName)
    {
        return _GetFiled(fieldName, PublicInstanceFieldFlag);
    }

    public T PublicInstanceField<T>(string fieldName) where T : class
    {
        return PublicInstanceField(fieldName) as T;
    }

    public void CallPublicInstanceMethod(string methodName, params object[] args)
    {
        _InvokeMethod(methodName, PublicInstanceMethodFlag, args);
    }

    public void CallPrivateInstanceMethod(string methodName, params object[] args)
    {
        _InvokeMethod(methodName, PrivateInstanceMethodFlag, args);
    }

    private object _GetFiled(string fieldName, BindingFlags flags)
    {
        if (null == InnerType) return null;
        var field = InnerType.GetField(fieldName, flags);
        return field != null ? field.GetValue(InnerObject) : null;
    }

    private void _InvokeMethod(string methodName, BindingFlags flags, params object[] args)
    {
        if (InnerType == null) return;
        var method = InnerType.GetMethod(methodName, flags);
        if (method == null) return;
        method.Invoke(InnerObject, args);
    }
}
