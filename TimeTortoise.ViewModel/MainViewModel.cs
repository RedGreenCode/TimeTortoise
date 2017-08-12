using System;
using System.Collections.ObjectModel;
using System.Globalization;
using TimeTortoise.Client;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class MainViewModel : NotificationBase
	{
		private readonly IRepository _repository;
		private readonly IDateTime _dateTime;
		private readonly ValidationMessageViewModel _validationMessageViewModel;
		private readonly ISignalRClient _client;

		public MainViewModel(string localPath, ValidationMessageViewModel validationMessageViewModel) :
			this(new Repository(new SqliteContext(localPath)),
			new SystemDateTime(), validationMessageViewModel,
			new SignalRClient())
		{
		}

		public MainViewModel(IRepository repository, IDateTime dateTime,
			ValidationMessageViewModel validationMessageViewModel, ISignalRClient signalRClient)
		{
			_repository = repository;
			_dateTime = dateTime;
			_validationMessageViewModel = validationMessageViewModel;
			_selectedTimeSegment = null;
			_client = signalRClient;
			_client.ConnectToServer();
			LoadActivities();
		}

		private bool SelectedActivityIndexIsValid()
		{
			return _selectedActivityIndex >= 0 && _selectedActivityIndex < Activities.Count;
		}

		private bool SelectedTimeSegmentIndexIsValid()
		{
			return _selectedTimeSegmentIndex >= 0 && SelectedActivity != null &&
				   _selectedTimeSegmentIndex < SelectedActivity.NumTimeSegments;
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
					Save();
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
			if (!SelectedActivityIndexIsValid())
				throw new InvalidOperationException("Can't load time segments without first selecting an activity.");

			var startDate = DateRangeStart?.DateTime;
			if (startDate.HasValue)
				startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day);
			var endDate = DateTime.MaxValue;
			var timeSegments = _repository.LoadTimeSegments(SelectedActivity.Id, new SystemDateTime(startDate), new SystemDateTime(endDate));
			SelectedActivity.ObservableTimeSegments.Clear();
			if (timeSegments == null) return;
			foreach (var timeSegment in timeSegments)
			{
				var tsvm = new TimeSegmentViewModel(timeSegment, new ValidationMessageViewModel());
				if (StartedTimeSegment != null && StartedTimeSegment.TimeSegment.StartTime == timeSegment.StartTime)
					tsvm = StartedTimeSegment;
				SelectedActivity.ObservableTimeSegments.Add(tsvm);
			}
		}

		public void Save()
		{
			_repository.SaveChanges();
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
				ActivityId = SelectedActivity.Id,
				StartTime = new DateTime(_dateTime.Now.Ticks),
				EndTime = new DateTime(_dateTime.Now.Ticks)
			};
			var tsvm = new TimeSegmentViewModel(ts, _validationMessageViewModel);
			SelectedTimeSegmentIndex = SelectedActivity.AddTimeSegment(ts, tsvm) - 1;
			_repository.AddTimeSgment(ts);
		}

		public void DeleteActivity()
		{
			if (StartedTimeSegment != null) StartStop();
			SelectedActivity.Name = string.Empty;
			var selectedActivity = SelectedActivity;
			selectedActivity.ClearAllTimeSegments();
			Activities.Remove(selectedActivity);
			_repository.DeleteActivity(selectedActivity);
		}

		private string _startStopText = "Start";
		public string StartStopText
		{
			get => _startStopText;
			private set => SetProperty(ref _startStopText, value);
		}

		public ActivityViewModel StartedActivity { get; private set; }
		public TimeSegmentViewModel StartedTimeSegment { get; private set; }

		public TimeSegmentViewModel IdleTimeSegment { get; private set; }

		public bool CheckIdleTime()
		{
			if (_client.Messages.Count > 0)
			{
				var lastUserActivityTime = _client.Messages.Dequeue();
				var currentTime = _dateTime.Now;
				var duration = currentTime - lastUserActivityTime;
				if (duration.TotalSeconds >= 10)
				{
					if (IdleTimeSegment == null) SetIdleTimeSegment(lastUserActivityTime, _dateTime.Now);
					else
					{
						IdleTimeSegment.StartTime = lastUserActivityTime.ToString(CultureInfo.InvariantCulture);
						IdleTimeSegment.EndTime = _dateTime.Now.ToString(CultureInfo.InvariantCulture);
					}
				}

				IsIncludeExcludeEnabled = true;
				return true;
			}

			IsIncludeExcludeEnabled = false;
			return false;
		}

		public void SetIdleTimeSegment(DateTime startTime, DateTime endTime)
		{
			var ts = new TimeSegment
			{
				StartTime = startTime,
				EndTime = endTime,
				ActivityId = SelectedActivityIndex
			};
			IdleTimeSegment = new TimeSegmentViewModel(ts, _validationMessageViewModel);
		}

		public void IncludeIdleTime()
		{
			IdleTimeSegment = null;
			IsIncludeExcludeEnabled = false;
		}

		private bool _isIncludeExcludeEnabled;
		public bool IsIncludeExcludeEnabled
		{
			get => _isIncludeExcludeEnabled;
			set => SetProperty(ref _isIncludeExcludeEnabled, value);
		}

		public void ExcludeIdleTime()
		{
			// TODO: Refactor to avoid duplicating code from StartStop()
			SelectedActivity = StartedActivity;
			StartedActivity = null;
			StartStopText = "Start";
			_startedTimeSegment.EndTime = DateTime.Parse(IdleTimeSegment.StartTime);
			SelectedActivity.UpdateTimeSegment(StartedTimeSegment);
			Save();
			StartedTimeSegment = null;
			IdleTimeSegment = null;
			IsIncludeExcludeEnabled = false;
		}

		private TimeSegment _startedTimeSegment;
		public void StartStop()
		{
			if (!SelectedActivityIndexIsValid())
				throw new InvalidOperationException("Can't start timing without first selecting an activity.");

			if (StartedActivity == null) StartedActivity = SelectedActivity;
			else
			{
				SelectedActivity = StartedActivity;
				StartedActivity = null;
			}

			StartStopText = StartedActivity == null ? "Start" : "Stop";
			if (StartedActivity != null)
			{
				_startedTimeSegment = new TimeSegment
				{
					StartTime = new DateTime(_dateTime.Now.Ticks),
					EndTime = new DateTime(_dateTime.Now.Ticks)
				};
				StartedTimeSegment = new TimeSegmentViewModel(_startedTimeSegment, _validationMessageViewModel);
				SelectedActivity.AddTimeSegment(_startedTimeSegment, StartedTimeSegment);
				Save();
			}
			else
			{
				_startedTimeSegment.EndTime = new DateTime(_dateTime.Now.Ticks);
				SelectedActivity.UpdateTimeSegment(StartedTimeSegment);
				Save();
				StartedTimeSegment = null;
			}
		}

		private int _selectedTimeSegmentIndex = -1;
		public int SelectedTimeSegmentIndex
		{
			get => _selectedTimeSegmentIndex;
			set
			{
				if (SelectedActivity == null)
				{
					IsTimeSegmentDeleteEnabled = false;
					SelectedTimeSegment = null;
					return;
				}

				SetProperty(ref _selectedTimeSegmentIndex, value);

				var numTimeSegments = SelectedActivity.NumTimeSegments;
				if (_selectedTimeSegmentIndex >= 0)
				{
					if (_selectedTimeSegmentIndex >= numTimeSegments)
						throw new IndexOutOfRangeException($"Selected time segment index is {_selectedTimeSegmentIndex}, but there are only {numTimeSegments} time segments.");
				}

				if (_selectedTimeSegmentIndex < 0)
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
			private set
			{
				SetProperty(ref _selectedTimeSegment, value);
				RaisePropertyChanged("SelectedTimeSegmentStartTime");
				RaisePropertyChanged("SelectedTimeSegmentEndTime");
			}
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
			if (!SelectedTimeSegmentIndexIsValid())
				throw new InvalidOperationException("No time segment is selected for deletion.");

			var selectedTimeSegment = SelectedTimeSegment;
			if (StartedTimeSegment != null && StartedTimeSegment == selectedTimeSegment) StartStop();
			SelectedActivity.RemoveTimeSegment(_repository, selectedTimeSegment);
			SelectedTimeSegment = null;
			RaisePropertyChanged("SelectedTimeSegmentStartTime");
			RaisePropertyChanged("SelectedTimeSegmentEndTime");
		}

		private DateTimeOffset? _dateRangeStart;
		public DateTimeOffset? DateRangeStart
		{
			get => _dateRangeStart;
			set
			{
				SetProperty(ref _dateRangeStart, value);
				if (SelectedActivityIndexIsValid())
					LoadTimeSegments();
			}
		}
	}
}
