using System;
using System.Globalization;
using Windows.UI.Xaml;
using TimeTortoise.Model;
using TimeTortoise.ViewModel;

namespace TimeTortoise.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			ValidationMessage = new ValidationMessageViewModel();
			var localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
			var settingsUtility = new SettingsUtility(localPath);
			Main = new MainViewModel(settingsUtility, localPath, ValidationMessage);

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
				return;
			}

			StartedActivity.Text = Main.StartedActivity.Name ?? string.Empty;
			Main.StartedTimeSegment.EndTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
			StartedTimeSegmentStartTime.Text = Main.StartedTimeSegment.StartTime;
			StartedTimeSegmentDuration.Text = Main.StartedTimeSegment.Duration;
			Main.WriteDailySummary();

			if (Main.CheckIdleTime())
			{
				IdleTimeSegmentStartTime.Text = Main.IdleTimeSegment.StartTime;
				IdleTimeSegmentEndTime.Text = Main.IdleTimeSegment.EndTime;
				IdleTimeDuration.Text = Main.IdleTimeSegment.Duration;
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
