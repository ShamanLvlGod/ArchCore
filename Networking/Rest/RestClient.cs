using System;
using System.Collections.Generic;
using ArchCore.Converter;
using ArchCore.Networking.Rest.Config;
using BestHTTP;

namespace ArchCore.Networking.Rest {
	public class RestClient {
		
		private IRestConfig config;
		private IObjectConverter converter;
		private Func<string, string> errorMessageParser;

		private readonly Dictionary<string, string> globalHeaders = new Dictionary<string, string>();

		public RestClient(IRestConfig config, IObjectConverter converter) 
		{
			this.config = config;
			this.converter = converter;
		}

		public void AddGlobalHeader(string key, string value) {
			globalHeaders[key] = value;
		}

		public void AddErrorParser<TError>(Func<string, TError> parser) {
			
		}
		
		public RestRequest<T, RestError> Get<T>(string path) {
			return Get<T, RestError>(path);
		}
		
		public RestRequest<T, RestError> Post<T>(string path, object body) {
			return Post<T, RestError>(path, body);
		}
		
		public RestRequest<T, RestError> Post<T>(string path) {
			return Post<T, RestError>(path);
		}
		
		public RestRequest<T, RestError> Put<T>(string path, object body) {
			return Put<T, RestError>(path, body);
		}
		
		public RestRequest<T, RestError> Delete<T>(string path) {
			return Delete<T, RestError>(path);
		}

		public RestRequest<T, TError> Get<T, TError>(string path) where TError : RestError, new() {
			return new RestRequest<T, TError>(converter, config.BaseUrl + path, HTTPMethods.Get, config.GlobalQueryParams, globalHeaders);
		}
		
		public RestRequest<T, TError> Post<T, TError>(string path, object body) where TError : RestError, new() {
			return new RestRequest<T, TError>(converter, config.BaseUrl + path, HTTPMethods.Post, config.GlobalQueryParams, globalHeaders).AddBody(body);
		}
		
		public RestRequest<T, TError> Post<T, TError>(string path) where TError : RestError, new() {
			return new RestRequest<T, TError>(converter, config.BaseUrl + path, HTTPMethods.Post, config.GlobalQueryParams, globalHeaders);
		}
		
		public RestRequest<T, TError> Put<T, TError>(string path, object body) where TError : RestError, new() {
			return new RestRequest<T, TError>(converter, config.BaseUrl + path, HTTPMethods.Put, config.GlobalQueryParams, globalHeaders).AddBody(body);
		}
		
		public RestRequest<T, TError> Delete<T, TError>(string path) where TError : RestError, new() {
			return new RestRequest<T, TError>(converter, config.BaseUrl + path, HTTPMethods.Delete, config.GlobalQueryParams, globalHeaders);
		}
		
		public static RestBaseSimpleRequest Get(string url) {
			return new RestBaseSimpleRequest(url, HTTPMethods.Get);
		}
		
		public static RestBaseSimpleRequest Post(string url, byte[] body) {
			return new RestBaseSimpleRequest(url, HTTPMethods.Post).AddBody(body);
		}
		
		public static RestBaseSimpleRequest Post(string url) {
			return new RestBaseSimpleRequest(url, HTTPMethods.Post);
		}
		
		public static RestBaseSimpleRequest Put(string url, byte[] body) {
			return new RestBaseSimpleRequest(url, HTTPMethods.Put).AddBody(body);
		}
		
		public static RestBaseSimpleRequest Delete(string url) {
			return new RestBaseSimpleRequest(url, HTTPMethods.Delete);
		}
	}
}