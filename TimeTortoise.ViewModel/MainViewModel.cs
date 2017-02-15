using System.Collections.ObjectModel;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class MainViewModel : NotificationBase
	{
		private readonly IRepository _repository;

		public MainViewModel(IRepository repository)
		{
			_repository = repository;
		}

		private ObservableCollection<Activity> _activities = new ObservableCollection<Activity>();
		public ObservableCollection<Activity> Activities
		{
			get { return _activities; }
			set { SetProperty(ref _activities, value); }
		}

		public void LoadActivities()
		{
			_activities = new ObservableCollection<Activity>(_repository.LoadActivities());
		}
	}
}
