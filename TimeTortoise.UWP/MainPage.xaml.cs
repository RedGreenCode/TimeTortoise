using TimeTortoise.ViewModel;

namespace TimeTortoise.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			Main = new MainViewModel(Windows.Storage.ApplicationData.Current.LocalFolder.Path);
		}

		public MainViewModel Main { get; set; }
	}
}
