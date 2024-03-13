using System;
using System.Collections.Generic;
using System.Text;
using ArchCore.Converter;
using BestHTTP;
using BestHTTP.Forms;
using BestHTTP.SocketIO;
using UnityEngine;

namespace ArchCore.Networking.Rest
{
	public sealed class RestRequest<T, TError> : RestBaseRequest where TError : RestError, new()
	{
		public delegate void RequestSuccessDelegate(RestResponse<T> response);

		public delegate void RequestErrorDelegate(TError error);

		public delegate void RequestCompleteDelegate(RestResponse<T> response, TError error);

		public delegate void RequestUploadProgressDelegate(float progress);

		private readonly IObjectConverter converter;
		private readonly string path;
		private readonly HTTPMethods method;

		private Dictionary<string, string> headers = new Dictionary<string, string>();
		private Dictionary<string, object> queryParams = new Dictionary<string, object>();
		private RequestSuccessDelegate onSuccess;
		private RequestErrorDelegate onFail;
		private RequestCompleteDelegate onComplete;
		private RequestUploadProgressDelegate onUploadProgress;
		private object body;
		private string queryBody;
		private byte[] bytes;
		private HTTPMultiPartForm form;

		public RestRequest(IObjectConverter converter, string path, HTTPMethods method,
			Dictionary<string, object> globalQueryParams, Dictionary<string, string> globalHeaders)
		{
			this.converter = converter;
			this.path = path;
			this.method = method;

			if (globalQueryParams != null)
			{
				foreach (var globalQueryParam in globalQueryParams)
				{
					queryParams.Add(globalQueryParam.Key, globalQueryParam.Value);
				}
			}

			if (globalHeaders != null)
			{
				foreach (var header in globalHeaders)
				{
					headers.Add(header.Key, header.Value);
				}
			}
		}

		public RestRequest<T, TError> AddQueryParam(string key, object value)
		{
			queryParams[key] = value;
			return this;
		}

		public RestRequest<T, TError> AddHeader(string key, string value)
		{
			headers[key] = value;
			return this;
		}

		public RestRequest<T, TError> AddBody(object body)
		{
			this.body = body;
			return this;
		}
		
		public RestRequest<T, TError> AddBodyQuery(string body)
		{
			queryBody = body;
			return this;
		}

		public RestRequest<T, TError> AddBinaryData(byte[] bytes)
		{
			this.bytes = bytes;
			return this;
		}

		public RestRequest<T, TError> AddFormData(string fieldName, byte[] content)
		{
			if (form == null)
			{
				form = new HTTPMultiPartForm();
			}

			form.AddBinaryData(fieldName, content);
			return this;
		}

		public RestRequest<T, TError> AddFormObjectData(string fieldName, object content)
		{
			if (form == null)
			{
				form = new HTTPMultiPartForm();
			}

			form.AddBinaryData(fieldName, converter.ToRawData(content), null, "application/json");
			return this;
		}

		public RestRequest<T, TError> OnSuccess(RequestSuccessDelegate onSuccess)
		{
			this.onSuccess = onSuccess;
			return this;
		}

		public RestRequest<T, TError> OnFail(RequestErrorDelegate onFail)
		{
			this.onFail = onFail;
			return this;
		}

		public RestRequest<T, TError> OnComplete(RequestCompleteDelegate onComplete)
		{
			this.onComplete = onComplete;
			return this;
		}

		public RestRequest<T, TError> OnUploadProgress(RequestUploadProgressDelegate onUploadProgress)
		{
			this.onUploadProgress = onUploadProgress;
			return this;
		}

		public void Send()
		{
			HTTPRequest request = new HTTPRequest(createUri(path, queryParams), method,
				delegate(HTTPRequest originalRequest, HTTPResponse response)
				{
					RestResponse<T> resp = null;
					TError error = null;

					if (originalRequest.Exception != null)
					{
						Debug.Log("<REST> ERROR Uri: " + originalRequest.Uri + " Exception: " + originalRequest.Exception.Message);
						error = new TError {ExceptionMessage = originalRequest.Exception.Message};
						invokeComplete(resp ?? new RestResponse<T>(), error);
						return;
					}

					Debug.Log("<REST> RESPONSE Uri: " + originalRequest.Uri + " Response: " + response?.DataAsText);

					try
					{
						resp = processResponse(originalRequest, response);
					}
					catch (RestException restException)
					{
						Debug.Log("<REST> SERVER ERROR: " + restException.Message);
						error = (TError) restException.Error;
					}
					catch (Exception e)
					{
						Debug.Log("<REST> ERROR: " + e.Message);
						error = new TError {ExceptionMessage = e.Message};
					}

					invokeComplete(resp ?? new RestResponse<T>(), error);
				});

			foreach (var header in headers)
			{
				request.AddHeader(header.Key, header.Value);
			}

			if (onUploadProgress != null)
			{
				request.OnUploadProgress += delegate(HTTPRequest originalRequest, long uploaded, long length) { onUploadProgress(uploaded / (float) length); };
			}

			if (body != null)
			{
				request.RawData = converter.ToRawData(body);
				Debug.Log("<REST> CALLING SERVER Uri: " + request.Uri + "\nBody: " + converter.ToString(body));
			}
			else if (queryBody != null)
			{
				request.RawData = System.Text.Encoding.UTF8.GetBytes(queryBody);
				Debug.Log("<REST> CALLING SERVER Uri: " + request.Uri + "\nBody: " + converter.ToString(queryBody));
			}
			else if (bytes != null)
			{
				request.AddBinaryData("reactionVideoInBytes", bytes);
				Debug.Log("<REST> CALLING SERVER Uri: " + request.Uri + "\nBinary Data length: " + bytes.Length);
			}
			else if (form != null)
			{
				request.SetForm(form);
				Debug.Log("<REST> CALLING SERVER Uri: " + request.Uri + "\nForm Data: " + form.ToString());
			}
			else
			{
				Debug.Log("<REST> CALLING SERVER Uri: " + request.Uri);
			}

			request.Send();
		}

		private RestResponse<T> processResponse(HTTPRequest request, HTTPResponse response)
		{
			if (response == null)
			{
				throw request.Exception ?? new Exception("Unknown Exception");
			}

			if (!response.IsSuccess)
			{
				TError error = converter.ToObject<TError>(response.DataAsText);
				throw new RestException(error, response.Message);
			}

			string dataText = response.DataAsText;
			
			RestResponse<T> result = new RestResponse<T>();
			//RestResponseWrapper<T> wrapper = converter.ToObject<RestResponseWrapper<T>>(dataText);

			if(typeof(T) == typeof(string)) 
				result.Body = (T)(object)response.DataAsText;
			else
				result.Body = converter.ToObject<T>(response.DataAsText);

//            if (wrapper.error != null)
//			{
//				throw new RestException(
//					new RestError((RestErrorType) wrapper.error.errorCode, wrapper.error.message),
//					wrapper.error.message);
//			}

            result.DataAsText = dataText;
			//result.Body = wrapper.data;
			result.Headers = response.Headers;


			return result;
		}

		private void invokeComplete(RestResponse<T> resp, TError error)
		{
			if (onComplete != null)
			{
				onComplete(resp, error);
			}

			if (error == null && onSuccess != null)
			{
				onSuccess(resp);
			}

			if (error != null && onFail != null)
			{
				onFail(error);
			}
		}
	}


}