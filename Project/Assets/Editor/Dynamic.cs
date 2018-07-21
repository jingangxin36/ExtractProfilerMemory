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

    private object _obj;


    public object InnerObject
    {
        get
        {
            return _obj;
        }

    }

    public Dynamic(Type innerType)
    {
        InnerType = innerType;
    }

    public Dynamic(object obj)
    {
        if (null != obj)
        {
            InnerType = obj.GetType();
            _obj = obj;
        }
    }

    public static void ShallowCopyFrom(object dst, object src, BindingFlags flags)
    {
        if (dst != null && null != src)
        {
            Type type = dst.GetType();
            Type type2 = src.GetType();
            FieldInfo[] fields = type.GetFields(flags);
            FieldInfo[] array = fields;
            foreach (FieldInfo fieldInfo2 in array)
            {
                var fieldInfo = type2.GetField(fieldInfo2.Name, flags);
                if (fieldInfo != null && fieldInfo2.FieldType == fieldInfo.FieldType)
                {
                    fieldInfo2.SetValue(dst, fieldInfo.GetValue(src));
                }
            }
        }
    }

    public void SetObject(object obj)
    {
        if (obj.GetType() == InnerType)
        {
            _obj = obj;
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

    private object _GetFiled(string fieldName, BindingFlags flags)
    {
        if (null != InnerType)
        {
            FieldInfo field = InnerType.GetField(fieldName, flags);
            if (null != field)
            {
                return field.GetValue(_obj);
            }
            return null;
        }
        return null;
    }

    public void CallPublicInstanceMethod(string methodName, params object[] args)
    {
        _InvokeMethod(methodName, PublicInstanceMethodFlag, args);
    }

    public void CallPrivateInstanceMethod(string methodName, params object[] args)
    {
        _InvokeMethod(methodName, PrivateInstanceMethodFlag, args);
    }

    private void _InvokeMethod(string methodName, BindingFlags flags, params object[] args)
    {
        if (null != InnerType)
        {
            MethodInfo method = InnerType.GetMethod(methodName, flags);
            if (null != method)
            {
                method.Invoke(_obj, args);
            }
        }
    }
}
