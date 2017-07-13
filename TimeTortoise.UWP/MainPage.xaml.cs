using System;
using System.Globalization;
using Windows.UI.Xaml;

using TimeTortoise.Client;
using TimeTortoise.ViewModel;

namespace TimeTortoise.UWP
{
	public sealed partial class MainPage
	{
		private SignalRClient _client;

		public MainPage()
		{
			InitializeComponent();
			ValidationMessage = new ValidationMessageViewModel();
			Main = new MainViewModel(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ValidationMessage);

			_dispatcherTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
			_dispatcherTimer.Tick += DispatcherTimer_Tick;
		}

		private void ClearStartedText()
		{
			StartedActivity.Text = string.Empty;
			StartedTimeSegmentStartTime.Text = string.Empty;
			StartedTimeSegmentDuration.Text = string.Empty;
		}

		private void DispatcherTimer_Tick(object sender, object e)
		{
			if (Main.StartedActivity == null)
			{
				ClearStartedText();
				_client = null;
				return;
			}

			if (_client == null)
			{
				_client = new SignalRClient();
				_client.ConnectToServer();
			}

			StartedActivity.Text = Main.StartedActivity.Name ?? string.Empty;
			Main.StartedTimeSegment.EndTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
			StartedTimeSegmentStartTime.Text = Main.StartedTimeSegment.StartTime;
			StartedTimeSegmentDuration.Text = Main.StartedTimeSegment.Duration;

			if (_client.Messages.Count > 0)
			{
				var lastUserActivityTime = _client.Messages.Dequeue();
				var currentTime = DateTime.Now;
				var duration = currentTime - lastUserActivityTime;
				if (duration.TotalSeconds >= 10)
				{
					if (Main.IdleTimeSegment == null) Main.SetIdleTimeSegment(lastUserActivityTime, DateTime.Now);
					else
					{
						Main.IdleTimeSegment.StartTime = lastUserActivityTime.ToString(CultureInfo.InvariantCulture);
						Main.IdleTimeSegment.EndTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
					}
					IdleTimeSegmentStartTime.Text = Main.IdleTimeSegment.StartTime;
					IdleTimeSegmentEndTime.Text = Main.IdleTimeSegment.EndTime;
					IdleTimeDuration.Text = Main.IdleTimeSegment.Duration;
					Main.IsIncludeExcludeEnabled = true;
				}
			}
		}

		private void StartStop()
		{
			if (Main.StartedTimeSegment != null)
			{
				_dispatcherTimer.Stop();
				ClearStartedText();
			}
			else _dispatcherTimer.Start();
			Main.StartStop();
		}

		private void ClearIdleTimeText()
		{
			IdleTimeSegmentStartTime.Text = string.Empty;
			IdleTimeSegmentEndTime.Text = string.Empty;
			IdleTimeDuration.Text = string.Empty;
		}

		private void IncludeIdleTime()
		{
			Main.IncludeIdleTime();
			ClearIdleTimeText();
		}

		private void ExcludeIdleTime()
		{
			Main.ExcludeIdleTime();
			ClearIdleTimeText();
		}

		private readonly DispatcherTimer _dispatcherTimer;

		public MainViewModel Main { get; set; }
		public ValidationMessageViewModel ValidationMessage { get; set; }
	}
}
