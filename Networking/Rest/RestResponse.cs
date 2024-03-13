using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArchCore.Networking.Rest {
    public class RestResponse<TData> {
        public TData Body;
        public Dictionary<string, List<string>> Headers;
        public string DataAsText;

        public RestResponse() {
            Body = default(TData);
            Headers = new Dictionary<string, List<string>>();
        }

        public bool HasHeader(string headerName) {
            var values = GetHeaderValues(headerName);
            if (values == null)
                return false;

            return true;
        }

        public List<string> GetHeaderValues(string name) {
            if (Headers == null)
                return null;

            name = name.ToLower();

            List<string> values;
            if (!Headers.TryGetValue(name, out values) || values.Count == 0) {
                return null;
            }

            return values;
        }

        public string GetFirstHeaderValue(string name) {
            if (Headers == null)
                return null;

            name = name.ToLower();

            List<string> values;
            if (!Headers.TryGetValue(name, out values) || values.Count == 0) {
                return null;
            }

            return values[0];
        }

        public bool HasHeaderWithValue(string headerName, string value) {
            var values = GetHeaderValues(headerName);
            if (values == null)
                return false;

            for (int i = 0; i < values.Count; ++i) {
                if (string.Compare(values[i], value, StringComparison.OrdinalIgnoreCase) == 0) {
                    return true;
                }
            }

            return false;
        }
    }

    
}