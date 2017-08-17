using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Xunit;

namespace TimeTortoise.Server.Tests
{
	public class ServerTests
	{
		[Fact]
		public void SignalRServer_StartsAndShutsDownWithoutError()
		{
			// This section is not a test. It's just to indicate to static analysis tools that this class and method are used.
			var startup = new Startup();
			startup.Configuration(new AppBuilder());

			// Start and stop the server
			var server = new SignalRServer();
			Task.Run(() => server.StartServer());
			server.Shutdown();
		}
	}
}
