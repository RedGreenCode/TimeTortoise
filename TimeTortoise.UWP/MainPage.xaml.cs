using System;
using System.Globalization;
using Windows.UI.Xaml;

using TimeTortoise.ViewModel;

namespace TimeTortoise.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			ValidationMessage = new ValidationMessageViewModel();
			Main = new MainViewModel(Windows.Storage.ApplicationData.Current.LocalFolder.Path, ValidationMessage);

			_dispatcherTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
			_dispatcherTimer.Tick += dispatcherTimer_Tick;
		}

		private void ClearStartedText()
		{
			StartedActivity.Text = string.Empty;
			StartedTimeSegmentStartTime.Text = string.Empty;
			StartedTimeSegmentDuration.Text = string.Empty;
		}

		private void dispatcherTimer_Tick(object sender, object e)
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

		private readonly DispatcherTimer _dispatcherTimer;

		public MainViewModel Main { get; set; }
		public ValidationMessageViewModel ValidationMessage { get; set; }
	}
}
