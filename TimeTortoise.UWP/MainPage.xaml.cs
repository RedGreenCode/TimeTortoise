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
		}

		public MainViewModel Main { get; set; }
		public ValidationMessageViewModel ValidationMessage { get; set; }
	}
}
