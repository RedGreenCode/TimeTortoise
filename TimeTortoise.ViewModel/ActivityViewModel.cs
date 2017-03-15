using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public ObservableCollection<TimeSegment> TimeSegments = new ObservableCollection<TimeSegment>();

		internal void AddTimeSegment(TimeSegment ts)
		{
			This.TimeSegments.Add(ts);
		}
	}
}
