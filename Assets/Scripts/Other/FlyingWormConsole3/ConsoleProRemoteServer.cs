using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace FlyingWormConsole3
{
	public class ConsoleProRemoteServer : MonoBehaviour
	{
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			ConsoleProRemoteServer.listener.Prefixes.Add("http://*:" + this.port + "/");
			ConsoleProRemoteServer.listener.Start();
			ConsoleProRemoteServer.listener.BeginGetContext(new AsyncCallback(this.ListenerCallback), null);
		}

		private void OnEnable()
		{
			Application.logMessageReceived += this.LogCallback;
		}

		private void OnDisable()
		{
			Application.logMessageReceived -= this.LogCallback;
		}

		public void LogCallback(string logString, string stackTrace, LogType type)
		{
			if (!logString.StartsWith("CPIGNORE"))
			{
				this.QueueLog(logString, stackTrace, type);
			}
		}

		private void QueueLog(string logString, string stackTrace, LogType type)
		{
			this.logs.Add(new ConsoleProRemoteServer.QueuedLog
			{
				message = logString,
				stackTrace = stackTrace,
				type = type
			});
		}

		private void ListenerCallback(IAsyncResult result)
		{
			ConsoleProRemoteServer.HTTPContext context = new ConsoleProRemoteServer.HTTPContext(ConsoleProRemoteServer.listener.EndGetContext(result));
			this.HandleRequest(context);
			ConsoleProRemoteServer.listener.BeginGetContext(new AsyncCallback(this.ListenerCallback), null);
		}

		private void HandleRequest(ConsoleProRemoteServer.HTTPContext context)
		{
			bool flag = false;
			string command = context.Command;
			if (command != null)
			{
				if (command == "/NewLogs")
				{
					flag = true;
					if (this.logs.Count > 0)
					{
						string text = string.Empty;
						for (int i = 0; i < this.logs.Count; i++)
						{
							ConsoleProRemoteServer.QueuedLog queuedLog = this.logs[i];
							text = text + "::::" + queuedLog.type;
							text = text + "||||" + queuedLog.message;
							text = text + ">>>>" + queuedLog.stackTrace + ">>>>";
						}
						context.RespondWithString(text);
						this.logs.Clear();
					}
					else
					{
						context.RespondWithString(string.Empty);
					}
				}
			}
			if (!flag)
			{
				context.Response.StatusCode = 404;
				context.Response.StatusDescription = "Not Found";
			}
			context.Response.OutputStream.Close();
		}

		public int port = 51000;

		private static HttpListener listener = new HttpListener();

		[NonSerialized]
		public List<ConsoleProRemoteServer.QueuedLog> logs = new List<ConsoleProRemoteServer.QueuedLog>();

		public class HTTPContext
		{
			public HTTPContext(HttpListenerContext inContext)
			{
				this.context = inContext;
			}

			public string Command
			{
				get
				{
					return WWW.UnEscapeURL(this.context.Request.Url.AbsolutePath);
				}
			}

			public HttpListenerRequest Request
			{
				get
				{
					return this.context.Request;
				}
			}

			public HttpListenerResponse Response
			{
				get
				{
					return this.context.Response;
				}
			}

			public void RespondWithString(string inString)
			{
				this.Response.StatusDescription = "OK";
				this.Response.StatusCode = 200;
				if (!string.IsNullOrEmpty(inString))
				{
					this.Response.ContentType = "text/plain";
					byte[] bytes = Encoding.UTF8.GetBytes(inString);
					this.Response.ContentLength64 = (long)bytes.Length;
					this.Response.OutputStream.Write(bytes, 0, bytes.Length);
				}
			}

			public HttpListenerContext context;

			public string path;
		}

		[Serializable]
		public class QueuedLog
		{
			public string message;

			public string stackTrace;

			public LogType type;
		}
	}
}
