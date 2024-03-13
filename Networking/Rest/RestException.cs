using System;

namespace ArchCore.Networking.Rest {
	public class RestException : Exception {
		public RestError Error;

		public RestException(RestError error, string message) : base(message) {
			Error = error;
		}
	}
}