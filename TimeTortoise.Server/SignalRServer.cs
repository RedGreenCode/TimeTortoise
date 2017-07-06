using System.Threading;
using Microsoft.Owin.Hosting;
using Owin;

namespace TimeTortoise.Server
{
	public class SignalRServer
	{
		MessageHub _messageHub;

		public void StartServer()
		{
			const string url = "http://127.0.0.1:8080";

			using (WebApp.Start(url))
			{
				_messageHub = new MessageHub();
				Thread.Sleep(Timeout.Infinite);
			}
		}

		public void SendMessage(string message)
		{
			_messageHub.SendMessage(message);
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
