using System;
using System.Globalization;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class TimeSegmentViewModel : NotificationBase<TimeSegment>
	{
		public TimeSegmentViewModel(TimeSegment timeSegment) : base(timeSegment)
		{

		}

		public string StartTime => This.StartTime.ToString(CultureInfo.CurrentUICulture);

		public string EndTime => 
			This.EndTime == DateTime.MinValue ?
			string.Empty :
			This.EndTime.ToString(CultureInfo.CurrentUICulture);
	}
}
