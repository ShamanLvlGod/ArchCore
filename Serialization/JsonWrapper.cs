using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ArchCore.Serialization
{
	public class JsonWrapper
	{
		public Type WrapperType;
		public JObject Data;

		public JsonWrapper()
		{
		}

		protected JsonWrapper(Type type)
		{
			WrapperType = type;
		}
	}

	public class JsonWrapper<TBase> : JsonWrapper
	{
		private TBase item;

		[JsonIgnore]
		public TBase Item
		{
			get { return item; }
			set
			{
				item = value;
				Data = JObject.FromObject(item);
			}
		}

		public JsonWrapper()
		{
		}

		protected JsonWrapper(Type type) : base(type)
		{
		}

		protected void CreateInstance<T>(JObject parameter) where T : TBase
		{
			T instance = parameter.ToObject<T>();
			Data = JObject.FromObject(instance);
			Item = Data.ToObject<T>();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext streamingContext)
		{
			Type type = WrapperType;
			MethodInfo func = GetType().GetMethod("CreateInstance", BindingFlags.Instance | BindingFlags.NonPublic);
			func?.MakeGenericMethod(type).Invoke(this, new object[] {Data});
		}

		public override string ToString()
		{
			return $"{WrapperType}: {Item}";
		}
	}
}