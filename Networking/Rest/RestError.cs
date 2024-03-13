namespace ArchCore.Networking.Rest {
	public class RestError {
		public string ExceptionMessage;
		public RestErrorType ExceptionType;

		public RestError() {
		}

		public RestError(RestErrorType type, string exceptionMessage) {
			ExceptionMessage = exceptionMessage;
			ExceptionType = type;
		}
	}

	public enum RestErrorType
	{
		Unsupported = 0,
		ExistingMail = 1,
		ExistingLogin = 2,
		SomethingWentWrong = 3,
		NotExistingUserOrInvalidPassword = 4,
		UnregisteredMail = 5,
		NotEnoughResources = 8,
		UnknownIdentifier = 9,
		MaxLevelReached = 10,
		UnavailableAction = 11
		//...
	}
}