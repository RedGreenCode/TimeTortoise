using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace TimeTortoise.Client
{
	public class SignalRHubConnection : IHubConnection
	{
		private readonly HubConnection _hubConnection;

		public SignalRHubConnection(string url)
		{
			_hubConnection = new HubConnection(url);
		}

		public IHubProxy CreateHubProxy(string hubName)
		{
			return _hubConnection.CreateHubProxy(hubName);
		}

		public Task Start()
		{
			return _hubConnection.Start();
		}

		public ConnectionState State => _hubConnection.State;
	}
}
