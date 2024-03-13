using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ArchCore.Serialization
{
//    public class AbstractSerializableList<T> : List<T>
//    {
//        private List<Type> types = new List<Type>();
//
//        [OnSerializing]
//        private void OnSerializing(StreamingContext streamingContext)
//        {
//            types.Clear();
//
//            foreach (var item in this)
//            {
//                types.Add(item.GetType());
//            }
//        }
//
//        [OnDeserialized]
//        private void OnDeserialized(StreamingContext streamingContext)
//        {
//            Type type = Type.GetType(WrapperType);
//            MethodInfo func = GetType().GetMethod("createInstance", BindingFlags.Instance | BindingFlags.NonPublic);
//            func?.MakeGenericMethod(type).Invoke(this, new object[] {Data});
//        }
//    }

}