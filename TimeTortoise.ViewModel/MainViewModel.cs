using System;
using System.Collections.ObjectModel;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class MainViewModel : NotificationBase
	{
		private readonly IRepository _repository;
		private readonly IDateTime _dateTime;
		private readonly ValidationMessageViewModel _validationMessageViewModel;

		public MainViewModel(string localPath, ValidationMessageViewModel validationMessageViewModel) : this(new Repository(new SqliteContext(localPath)), new SystemDateTime(), validationMessageViewModel)
		{
		}

		public MainViewModel(IRepository repository, IDateTime dateTime, ValidationMessageViewModel validationMessageViewModel)
		{
			_repository = repository;
			_dateTime = dateTime;
			_validationMessageViewModel = validationMessageViewModel;
			_selectedTimeSegment = new TimeSegmentViewModel(new TimeSegment(), _validationMessageViewModel);
			LoadActivities();
		}

		public ObservableCollection<ActivityViewModel> Activities { get; private set; } = new ObservableCollection<ActivityViewModel>();

		private int _selectedActivityIndex = -1;
		public int SelectedActivityIndex
		{
			get { return _selectedActivityIndex; }
			set
			{
				SetProperty(ref _selectedActivityIndex, value);

				if (_selectedActivityIndex < 0 || _selectedActivityIndex >= Activities.Count)
				{
					IsSaveEnabled = false;
					SelectedActivity = new ActivityViewModel(new Activity());
				}
				else
				{
					IsSaveEnabled = true;
					SelectedActivity = Activities[_selectedActivityIndex];
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
			foreach (var activity in activities)
			{
				var avm = new ActivityViewModel(activity);
				foreach (var ts in activity.TimeSegments)
				{
					avm.AddTimeSegment(new TimeSegmentViewModel(ts, _validationMessageViewModel));
				}
				Activities.Add(avm);
			}
		}

		public void Save()
		{
			_repository.SaveActivity();
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

		public void AddActivity()
		{
			var activity = new Activity();
			Activities.Add(new ActivityViewModel(activity));
			_repository.AddActivity(activity);
			SelectedActivityIndex = Activities.Count - 1;
		}

		public void AddTimeSegment()
		{
			var ts = new TimeSegment();
			SelectedTimeSegmentIndex = SelectedActivity.AddTimeSegment(ts, new TimeSegmentViewModel(ts, _validationMessageViewModel)) - 1;
		}

		public void DeleteActivity()
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

		private bool _isStarted;
		private TimeSegment _currentTimeSegment;
		public void StartStop()
		{
			if (_selectedActivityIndex < 0 || _selectedActivityIndex >= Activities.Count)
				throw new InvalidOperationException("Can't start timing without a selected activity");

			_isStarted = !_isStarted;
			StartStopText = _isStarted ? "Stop" : "Start";
			if (_isStarted)
			{
				_currentTimeSegment = new TimeSegment {StartTime = new DateTime(_dateTime.Now.Ticks)};
				SelectedActivity.AddTimeSegment(_currentTimeSegment, new TimeSegmentViewModel(_currentTimeSegment, _validationMessageViewModel));
			}
			else
			{
				_currentTimeSegment.EndTime = new DateTime(_dateTime.Now.Ticks);
				SelectedActivity.UpdateTimeSegment(new TimeSegmentViewModel(_currentTimeSegment, _validationMessageViewModel));
				_repository.SaveActivity();
			}
		}

		private int _selectedTimeSegmentIndex = -1;
		public int SelectedTimeSegmentIndex
		{
			get { return _selectedTimeSegmentIndex; }
			set
			{
				SetProperty(ref _selectedTimeSegmentIndex, value);

				if (_selectedTimeSegmentIndex < 0 || _selectedTimeSegmentIndex >= Activities.Count)
				{
					IsSaveEnabled = false;
					SelectedTimeSegment = new TimeSegmentViewModel(new TimeSegment(), _validationMessageViewModel);
				}
				else
				{
					IsSaveEnabled = true;
					SelectedTimeSegment = SelectedActivity.GetTimeSegment(_selectedTimeSegmentIndex);
				}
			}
		}

		private TimeSegmentViewModel _selectedTimeSegment;
		public TimeSegmentViewModel SelectedTimeSegment
		{
			get { return _selectedTimeSegment; }
			private set { SetProperty(ref _selectedTimeSegment, value); }
		}

		public void DeleteTimeSegment()
		{
			var selectedTimeSegment = SelectedTimeSegment;
			SelectedActivity.RemoveTimeSegment(_repository, selectedTimeSegment);
		}
	}
}
