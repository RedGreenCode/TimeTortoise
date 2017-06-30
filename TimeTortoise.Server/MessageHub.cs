using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TimeTortoise.Server
{
	[HubName("MessageHub")]
	public class MessageHub : Hub
	{
		public void SendMessage(string message)
		{
			var hubContext = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
			hubContext.Clients.All.ReceiveMessage(message);
		}
	}
}
