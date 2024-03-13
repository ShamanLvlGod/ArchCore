using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ArchCore.Converter
{
	public class NewtonsoftJsonConverter : IObjectConverter
	{

		private readonly JsonSerializerSettings settings;

		public NewtonsoftJsonConverter()
		{
			settings = new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			settings.Converters.Add(new StringEnumConverter());
		}

		public T ToObject<T>(string data)
		{
			return JsonConvert.DeserializeObject<T>(data, settings);
		}

		public string ToString(object obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.None, settings);
		}

		public byte[] ToRawData(object obj)
		{
			string stringData = ToString(obj);
			return System.Text.Encoding.UTF8.GetBytes(stringData);
		}
	}
}