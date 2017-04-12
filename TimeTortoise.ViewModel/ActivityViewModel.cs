using System.Collections.ObjectModel;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class ActivityViewModel : NotificationBase<Activity>
	{
		public ActivityViewModel(Activity activity) : base(activity)
		{
		}

		public string Name
		{
			get { return This.Name; }
			set { SetProperty(This.Name, value, () => This.Name = value); }
		}

		public readonly ObservableCollection<TimeSegmentViewModel> ObservableTimeSegments = new ObservableCollection<TimeSegmentViewModel>();

		public void AddTimeSegment(TimeSegmentViewModel tsvm)
		{
			ObservableTimeSegments.Add(tsvm);
		}

		public int AddTimeSegment(TimeSegment ts, TimeSegmentViewModel tsvm)
		{
			This.TimeSegments.Add(ts);
			ObservableTimeSegments.Add(tsvm);
			return ObservableTimeSegments.Count;
		}
		public TimeSegmentViewModel GetTimeSegment(int index)
		{
			return ObservableTimeSegments[index];
		}

		public void UpdateTimeSegment(TimeSegmentViewModel tsvm)
		{
			ObservableTimeSegments[ObservableTimeSegments.Count - 1] = tsvm;
		}

		public void RemoveTimeSegment(IRepository repository, TimeSegmentViewModel tsvm)
		{
			ObservableTimeSegments.Remove(tsvm);
			repository.DeleteTimeSegment(tsvm.TimeSegment);
		}

		public int NumObservableTimeSegments
		{
			get { return ObservableTimeSegments.Count; }
		}

		public int NumTimeSegments
		{
			get { return This.TimeSegments.Count; }
		}
	}
}
