using System;
using System.ComponentModel;

namespace RomanceTour.Models
{
	public enum ResultType
	{
		SUCCESS,
		NOT_FOUND,
		ALREADY_EXISTS,
		LOGIN_REQUIRED,
		ACCESS_DENIED,
		INVALID_ACCESS,
		SYSTEM_ERROR
	}
	public class Response
	{
		public ResultType Result { get; set; }
		public object Model { get; set; }
		public int ErrorCode => Error is Win32Exception w ? w.ErrorCode : 0;
		public string ErrorMessage => Error != null ? Error.Message : string.Empty;
		public string StackTrace => Error != null ? Error.StackTrace : string.Empty;
		public Exception Error { private get; set; }
	}
}