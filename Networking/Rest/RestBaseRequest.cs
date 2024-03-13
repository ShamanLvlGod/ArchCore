using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP;

namespace ArchCore.Networking.Rest {
    public abstract class RestBaseRequest {
		
		protected Uri createUri(string path, Dictionary<string, object> queryParams) {
			StringBuilder query = new StringBuilder();
			foreach (var key in queryParams.Keys) {
				query.Append(string.Format("{0}={1}&", key, queryParams[key]));
			}

			// Remove last &
			if (query.Length > 0) {
				query.Remove(query.Length - 1, 1);
			}
			return new Uri(path + (query.Length > 0 ? "?" + query : ""));
		}
    }
}