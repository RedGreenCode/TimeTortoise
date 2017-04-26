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
			_selectedTimeSegment = null;
			LoadActivities();
		}

		private bool SelectedActivityIndexIsValid()
		{
			return _selectedActivityIndex >= 0 && _selectedActivityIndex < Activities.Count;
		}

		public ObservableCollection<ActivityViewModel> Activities { get; private set; } = new ObservableCollection<ActivityViewModel>();

		private int _selectedActivityIndex = -1;
		public int SelectedActivityIndex
		{
			get => _selectedActivityIndex;
			set
			{
				SetProperty(ref _selectedActivityIndex, value);

				if (!SelectedActivityIndexIsValid())
				{
					IsSaveEnabled = false;
					IsTimeSegmentAddEnabled = false;
					SelectedActivity = null;
				}
				else
				{
					IsSaveEnabled = true;
					IsTimeSegmentAddEnabled = true;
					SelectedActivity = Activities[_selectedActivityIndex];
					LoadTimeSegments();
				}
			}
		}

		private ActivityViewModel _selectedActivity;
		public ActivityViewModel SelectedActivity
		{
			get => _selectedActivity;
			private set => SetProperty(ref _selectedActivity, value);
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

		public void LoadTimeSegments()
		{
			var timeSegments = _repository.LoadTimeSegments(SelectedActivity.Id, new SystemDateTime(DateRangeStart?.DateTime), new SystemDateTime(DateTime.MaxValue));
			SelectedActivity.ObservableTimeSegments.Clear();
			foreach (var timeSegment in timeSegments)
			{
				SelectedActivity.ObservableTimeSegments.Add(new TimeSegmentViewModel(timeSegment, new ValidationMessageViewModel()));
			}
		}

		public void Save()
		{
			_repository.SaveActivity();
		}

		private bool _isSaveEnabled;
		public bool IsSaveEnabled
		{
			get => _isSaveEnabled;
			private set => SetProperty(ref _isSaveEnabled, value);
		}

		private bool _isTimeSegmentAddEnabled;
		public bool IsTimeSegmentAddEnabled
		{
			get => _isTimeSegmentAddEnabled;
			private set => SetProperty(ref _isTimeSegmentAddEnabled, value);
		}

		private bool _isTimeSegmentDeleteEnabled;
		public bool IsTimeSegmentDeleteEnabled
		{
			get => _isTimeSegmentDeleteEnabled;
			private set => SetProperty(ref _isTimeSegmentDeleteEnabled, value);
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
			if (!SelectedActivityIndexIsValid())
				throw new InvalidOperationException("Can't add a time segment without first selecting an activity.");
			var ts = new TimeSegment
			{
				StartTime = new DateTime(_dateTime.Now.Ticks),
				EndTime = new DateTime(_dateTime.Now.Ticks)
			};
			var tsvm = new TimeSegmentViewModel(ts, _validationMessageViewModel);
			SelectedTimeSegmentIndex = SelectedActivity.AddTimeSegment(ts, tsvm) - 1;
			_repository.AddTimeSgment(ts);
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
			get => _startStopText;
			private set => SetProperty(ref _startStopText, value);
		}

		private bool _isStarted;
		private TimeSegment _currentTimeSegment;
		private TimeSegmentViewModel _currentTimeSegmentViewModel;
		public void StartStop()
		{
			if (!SelectedActivityIndexIsValid())
				throw new InvalidOperationException("Can't start timing without first selecting an activity.");

			_isStarted = !_isStarted;
			StartStopText = _isStarted ? "Stop" : "Start";
			if (_isStarted)
			{
				_currentTimeSegment = new TimeSegment
				{
					StartTime = new DateTime(_dateTime.Now.Ticks),
					EndTime = new DateTime(_dateTime.Now.Ticks)
				};
				_currentTimeSegmentViewModel = new TimeSegmentViewModel(_currentTimeSegment, _validationMessageViewModel);
				SelectedActivity.AddTimeSegment(_currentTimeSegment, _currentTimeSegmentViewModel);
			}
			else
			{
				_currentTimeSegment.EndTime = new DateTime(_dateTime.Now.Ticks);
				SelectedActivity.UpdateTimeSegment(_currentTimeSegmentViewModel);
				_repository.SaveActivity();
			}
		}

		private int _selectedTimeSegmentIndex = -1;
		public int SelectedTimeSegmentIndex
		{
			get => _selectedTimeSegmentIndex;
			set
			{
				SetProperty(ref _selectedTimeSegmentIndex, value);

				if (_selectedTimeSegmentIndex < 0 || SelectedActivity == null || _selectedTimeSegmentIndex >= SelectedActivity.NumTimeSegments)
				{
					IsTimeSegmentDeleteEnabled = false;
					SelectedTimeSegment = null;
				}
				else
				{
					IsTimeSegmentDeleteEnabled = true;
					SelectedTimeSegment = SelectedActivity.GetTimeSegment(_selectedTimeSegmentIndex);
					SelectedTimeSegmentStartTime = SelectedTimeSegment.StartTime;
					SelectedTimeSegmentEndTime = SelectedTimeSegment.EndTime;
				}
			}
		}

		private TimeSegmentViewModel _selectedTimeSegment;
		public TimeSegmentViewModel SelectedTimeSegment
		{
			get => _selectedTimeSegment;
			private set => SetProperty(ref _selectedTimeSegment, value);
		}

		public string SelectedTimeSegmentStartTime
		{
			get => _selectedTimeSegment == null ? string.Empty : _selectedTimeSegment.StartTime;
			set
			{
				if (_selectedTimeSegment == null) return;
				SelectedTimeSegment.StartTime = value;
				RaisePropertyChanged("SelectedTimeSegmentStartTime");
			}
		}

		public string SelectedTimeSegmentEndTime
		{
			get => _selectedTimeSegment == null ? string.Empty : _selectedTimeSegment.EndTime;
			set
			{
				if (_selectedTimeSegment == null) return;
				SelectedTimeSegment.EndTime = value;
				RaisePropertyChanged("SelectedTimeSegmentEndTime");
			}
		}

		public void DeleteTimeSegment()
		{
			var selectedTimeSegment = SelectedTimeSegment;
			SelectedActivity.RemoveTimeSegment(_repository, selectedTimeSegment);
		}

		private DateTimeOffset? _dateRangeStart;
		public DateTimeOffset? DateRangeStart
		{
			get => _dateRangeStart;
			set
			{
				SetProperty(ref _dateRangeStart, value);
				LoadTimeSegments();
			}
		}
	}
}
