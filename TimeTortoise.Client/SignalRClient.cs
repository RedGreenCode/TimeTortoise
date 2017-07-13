using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;

namespace TimeTortoise.Client
{
	public class SignalRClient
	{
		private HubConnection _hubConnection;
		private IHubProxy _hubProxy;
		private IDisposable _receiveMessageHandler;

		public Queue<DateTime> Messages { get; }

		public SignalRClient()
		{
			Messages = new Queue<DateTime>();
		}

		public void ConnectToServer()
		{
			_hubConnection = new HubConnection("http://127.0.0.1:8080");
			_hubProxy = _hubConnection.CreateHubProxy("MessageHub");	// must match hub name
			_receiveMessageHandler = _hubProxy.On<DateTime>("ReceiveMessage", ReceiveMessage);
			_hubConnection.Start();
		}

		private void ReceiveMessage(DateTime lastUserActivityTime)
		{
			Messages.Enqueue(lastUserActivityTime);
		}
	}
}
