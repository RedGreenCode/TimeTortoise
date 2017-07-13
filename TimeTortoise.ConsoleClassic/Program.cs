using System;
using System.Threading;
using System.Threading.Tasks;
using TimeTortoise.Server;

namespace TimeTortoise.ConsoleClassic
{
	public class Program
	{
		private static SignalRServer server;

		private void StartServer()
		{
			server = new SignalRServer();
			server.StartServer();
		}

		public static void Main(string[] args)
		{
			try
			{
				var p = new Program();
				Task.Run(() => p.StartServer());

				while (true)
				{
					Thread.Sleep(1000);
					server.SendMessage(DateTime.Now);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
