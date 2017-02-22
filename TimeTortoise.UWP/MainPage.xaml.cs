using TimeTortoise.ViewModel;

namespace TimeTortoise.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			Main = new MainViewModel();
		}

		public MainViewModel Main { get; set; }
	}
}
