using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cloud;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class CloudRequestsSequence
	{
		public CloudRequestsSequence(params ICloudRequest[] requests)
		{
			foreach (ICloudRequest item in requests)
			{
				this.queue.Enqueue(item);
			}
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Complete;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ICloudRequest, Response> RequestSuccess;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ICloudRequest, Response> RequestFailed;



		public IEnumerator Execute()
		{
			this.invalidRequests = new List<RequestLog>();
			while (this.queue.Count > 0)
			{
				ICloudRequest request = this.queue.Dequeue();
				yield return request.Execute();
				Response response = request.GetResponse();
				bool isValid = request.IsValid(response);
				if (isValid)
				{
					this.RequestSuccess(request, response);
				}
				else
				{
					this.LogInvalidRequest(new RequestLog
					{
						Request = request,
						Response = response
					});
					this.RequestFailed(request, response);
				}
			}
			this.Complete();
			yield break;
		}

		private void LogInvalidRequest(RequestLog invalidRequest)
		{
			this.invalidRequests.Add(invalidRequest);
		}

		public bool Success
		{
			get
			{
				return this.invalidRequests.Count == 0;
			}
		}

		public List<RequestLog> InvalidRequests
		{
			get
			{
				return this.invalidRequests;
			}
		}

		private readonly Queue<ICloudRequest> queue = new Queue<ICloudRequest>();

		private List<RequestLog> invalidRequests;
	}
}
