using System;
using System.Collections;
using TactileModules.TactileCloud;
using UnityEngine;

namespace Cloud
{
	public class Response : ICloudResponse
	{
		public Hashtable data { get; set; }

		public DateTime utcReceived { get; set; }

		public string Body { get; private set; }

		public bool Success
		{
			get
			{
				return this.ReturnCode >= ReturnCode.NoError;
			}
		}

		public ReturnCode ReturnCode
		{
			get
			{
				if (this.data == null || !this.data.ContainsKey("returnCode"))
				{
					return ReturnCode.UnexpectedError;
				}
				return (ReturnCode)((double)this.data["returnCode"]);
			}
		}

		public string ErrorInfo
		{
			get
			{
				if (this.data == null)
				{
					return "No response data available - errorInfo not available";
				}
				if (!this.data.ContainsKey("errorInfo"))
				{
					return "errorInfo was not available in response data";
				}
				return (string)this.data["errorInfo"];
			}
		}

		public bool PasswordInvalid
		{
			get
			{
				return this.ReturnCode == ReturnCode.AuthenticationFailed;
			}
		}

		public bool IsNetworkError
		{
			get
			{
				return this.ReturnCode == ReturnCode.ClientConnectionError;
			}
		}

		public bool IsRecoverableError
		{
			get
			{
				ReturnCode returnCode = this.ReturnCode;
				switch (returnCode + 15)
				{
				case ReturnCode.NoError:
				case ReturnCode.NoErrorAlreadyLatestVersion:
				case (ReturnCode)2:
				case (ReturnCode)6:
				case (ReturnCode)7:
					break;
				default:
					if (returnCode != ReturnCode.ClientConnectionError)
					{
						return false;
					}
					break;
				}
				return true;
			}
		}

		public void FillResponse(WWW request)
		{
			this.utcReceived = DateTime.UtcNow;
			if (request == null)
			{
				this.data = new Hashtable
				{
					{
						"returnCode",
						-10000.0
					},
					{
						"errorInfo",
						"Request timed out..."
					}
				};
				this.Body = "Request timed out...";
			}
			else if (request.error != null)
			{
				this.data = new Hashtable
				{
					{
						"returnCode",
						-10000.0
					},
					{
						"errorInfo",
						request.error
					}
				};
				this.Body = request.error;
			}
			else
			{
				this.data = (MiniJSON.jsonDecode(request.text) as Hashtable);
				this.Body = request.text;
				if (this.data == null)
				{
					this.data = new Hashtable
					{
						{
							"returnCode",
							-1.0
						},
						{
							"errorInfo",
							"Unexpected error..."
						}
					};
				}
			}
		}
	}
}
