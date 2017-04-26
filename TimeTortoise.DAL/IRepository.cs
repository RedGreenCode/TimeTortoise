using System.Collections.Generic;

using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public interface IRepository
	{
		List<Activity> LoadActivities();
		List<TimeSegment> LoadTimeSegments(int activityId, IDateTime startTime, IDateTime endTime);
		void AddActivity(Activity activity);
		void SaveActivity();
		void DeleteActivity(Activity activity);
		void AddTimeSgment(TimeSegment ts);
		void DeleteTimeSegment(TimeSegment ts);
	}
}
