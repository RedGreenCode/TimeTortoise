﻿using System;
using System.Collections.ObjectModel;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class MainViewModel : NotificationBase
	{
		private readonly IRepository _repository;
		private readonly IDateTime _dateTime;

		public MainViewModel(string localPath) : this(new Repository(new SqliteContext(localPath)), new SystemDateTime())
		{
		}

		public MainViewModel(IRepository repository, IDateTime dateTime)
		{
			_repository = repository;
			_dateTime = dateTime;
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
			foreach (var activity in activities)
			{
				var avm = new ActivityViewModel(activity);
				foreach (var ts in activity.TimeSegments)
				{
					avm.TimeSegments.Add(ts);
				}
				Activities.Add(avm);
			}
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

		private bool _isStarted;
		private TimeSegment _currentTimeSegment;
		public void StartStop()
		{
			_isStarted = !_isStarted;
			StartStopText = _isStarted ? "Stop" : "Start";
			if (_isStarted)
			{
				_currentTimeSegment = new TimeSegment {StartTime = new DateTime(_dateTime.Now.Ticks)};
				SelectedActivity.TimeSegments.Add(_currentTimeSegment);
				SelectedActivity.AddTimeSegment(_currentTimeSegment);
			}
			else
			{
				_currentTimeSegment.EndTime = new DateTime(_dateTime.Now.Ticks);
			}
		}
	}
}