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

		private void dispatcherTimer_Tick(object sender, object e)
		{
			Main.SelectedTimeSegmentEndTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
		}

		private void StartStop()
		{
			if (_dispatcherTimer.IsEnabled) _dispatcherTimer.Stop();
			else _dispatcherTimer.Start();
			Main.StartStop();
		}

		private readonly DispatcherTimer _dispatcherTimer;

		public MainViewModel Main { get; set; }
		public ValidationMessageViewModel ValidationMessage { get; set; }
	}
}
