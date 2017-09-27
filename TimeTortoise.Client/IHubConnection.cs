using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace TimeTortoise.Client
{
	public interface IHubConnection
	{
		IHubProxy CreateHubProxy(string hubName);
		Task Start();
		ConnectionState State { get; }
	}
}
