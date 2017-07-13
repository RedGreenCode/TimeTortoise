using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using TimeTortoise.Server;

namespace TimeTortoise.WPF
{
	/// <summary>
	/// This window has no visible UI components. It's just here as a container.
	/// </summary>
	public partial class MainWindow
	{
		private readonly SignalRServer _server;
		private readonly IdleTime _idleTime;

		public MainWindow()
		{
			InitializeComponent();
			_server = new SignalRServer();
			_idleTime = new IdleTime();

			Task.Run(() => StartServer());
			var dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Start();
		}

		private void DispatcherTimer_Tick(object sender, object e)
		{
			_server.SendMessage(_idleTime.LastUserActivityTime);
		}

		private void StartServer()
		{
			_server.StartServer();
		}
	}
}
