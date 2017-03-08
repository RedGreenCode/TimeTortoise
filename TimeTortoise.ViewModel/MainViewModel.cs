using System;
using System.Collections.ObjectModel;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class MainViewModel : NotificationBase
	{
		private readonly IRepository _repository;

		public MainViewModel() : this(new Repository(new SqliteContext()))
		{
		}

		public MainViewModel(IRepository repository)
		{
			_repository = repository;
			LoadActivities();
		}

		public ObservableCollection<ActivityViewModel> Activities { get; private set; } = new ObservableCollection<ActivityViewModel>();

		private int _selectedIndex = -1;
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set
			{
				SetProperty(ref _selectedIndex, value);

				if (_selectedIndex < 0 || _selectedIndex >= Activities.Count)
				{
					IsSaveEnabled = false;
					SelectedActivity = new ActivityViewModel(new Activity());
				}
				else
				{
					IsSaveEnabled = true;
					SelectedActivity = Activities[_selectedIndex];
				}
			}
		}

		private ActivityViewModel _selectedActivity = new ActivityViewModel(new Activity());
		public ActivityViewModel SelectedActivity
		{
			get { return _selectedActivity; }
			private set { SetProperty(ref _selectedActivity, value); }
		}

		public void LoadActivities()
		{
			var activities = _repository.LoadActivities();
			Activities = new ObservableCollection<ActivityViewModel>();
			foreach (var activity in activities) Activities.Add(new ActivityViewModel(activity));
		}

		public void Save()
		{
			if (_selectedIndex < 0 || _selectedIndex >= Activities.Count)
			{
				throw new InvalidOperationException("Tried to save an invalid selection at index " + _selectedIndex);
			}
			_repository.SaveActivity(Activities[_selectedIndex]);
		}

		private bool _isSaveEnabled;
		public bool IsSaveEnabled
		{
			get { return _isSaveEnabled; }
			private set
			{
				SetProperty(ref _isSaveEnabled, value);
			}
		}

		public void Add()
		{
			Activities.Add(new ActivityViewModel(new Activity()));
			SelectedIndex = Activities.Count - 1;
		}

		public void Delete()
		{
			var selectedActivity = SelectedActivity;
			Activities.Remove(selectedActivity);
			_repository.DeleteActivity(selectedActivity);
		}

		private string _startStopText = "Start";
		public string StartStopText
		{
			get { return _startStopText; }
			private set
			{
				SetProperty(ref _startStopText, value);
			}
		}

		public void StartStop()
		{
			StartStopText = _startStopText == "Start" ? "Stop" : "Start";
		}
	}
}
