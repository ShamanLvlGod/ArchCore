using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ArchCore.Networking.Rest
{
	public class RestResponseWrapper<TData>
	{
		public bool success;
		public TData data;
        
		[JsonProperty(PropertyName = "dataError")]
		public RestWrapperError error;
	}

	public class RestWrapperError
	{
		[JsonProperty(PropertyName = "error")]
		public int errorCode;
		public string message;
	}
}