using System;
using System.Collections.Generic;


public class InterfaceServiceLocator 
{
    private static Dictionary<Type, object> _interfaceDB = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class{
        _interfaceDB.Add(typeof(T), service);
    }

    public static T Get<T>(Type type) where T : class{
        return _interfaceDB[type] as T;
    }

}
