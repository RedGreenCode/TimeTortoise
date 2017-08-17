using System;
using System.Reflection;
using System.Threading;
using Microsoft.Owin.Hosting;
using Owin;

namespace TimeTortoise.Server
{
	public class SignalRServer
	{
		private MessageHub _messageHub;
		private IDisposable _webApp;

		public void StartServer()
		{
			const string url = "http://127.0.0.1:8080";

			try
			{
				_webApp = WebApp.Start(url);
				_messageHub = new MessageHub();
				Thread.Sleep(Timeout.Infinite);
			}
			catch (TargetInvocationException)
			{
				// a Time Tortoise Companion server is already running
			}
		}

		public void SendMessage(DateTime lastUserActivityTime)
		{
			_messageHub.SendMessage(lastUserActivityTime);
		}

		public void Shutdown()
		{
			_webApp?.Dispose();
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}
