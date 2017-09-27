using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;

namespace TimeTortoise.Client
{
	public class SignalRClient : ISignalRClient
	{
		private readonly IHubConnection _hubConnection;
		private IHubProxy _hubProxy;
		private DateTime _lastConnectionCheckTime = DateTime.MinValue;

		private Stack<DateTime> Messages { get; }

		public SignalRClient(string url) : this(new SignalRHubConnection(url))
		{
		}

		public SignalRClient(IHubConnection hubConnection)
		{
			Messages = new Stack<DateTime>();
			_hubConnection = hubConnection;
		}

		public DateTime GetNewestMessage()
		{
			if (_hubConnection.State != ConnectionState.Connected)
			{
				var ts = DateTime.Now - _lastConnectionCheckTime;
				if (ts.TotalSeconds > 10) ConnectToServer();
				_lastConnectionCheckTime = DateTime.Now;
			}
			if (Messages.Count == 0) return DateTime.MinValue;
			var message = Messages.Pop();
			Messages.Clear();
			return message;
		}

		public void ConnectToServer()
		{
			_hubProxy = _hubConnection.CreateHubProxy("MessageHub");	// must match hub name
			_hubProxy.On<DateTime>("ReceiveMessage", ReceiveMessage);
			_hubConnection.Start();
		}

		public void ReceiveMessage(DateTime lastUserActivityTime)
		{
			Messages.Push(lastUserActivityTime);
		}
	}
}
