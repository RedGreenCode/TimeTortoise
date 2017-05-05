using System.Collections.Generic;
using System.Linq;
using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public class Repository : IRepository
	{
		private readonly Context _context;

		public Repository(Context context)
		{
			_context = context;
		}

		public List<Activity> LoadActivities()
		{
			return _context.Activities.ToList();
		}

		public List<TimeSegment> LoadTimeSegments(int activityId, IDateTime startTime, IDateTime endTime)
		{
			return _context.TimeSegments.Where(t => t.ActivityId == activityId && t.StartTime >= startTime.Value && t.EndTime <= endTime.Value).ToList();
		}

		public void AddActivity(Activity activity)
		{
			_context.Add(activity);
		}

		public void SaveActivity()
		{
			_context.SaveChanges();
		}

		public void DeleteActivity(Activity activity)
		{
			_context.Remove(activity);
			_context.SaveChanges();
		}

		public void AddTimeSgment(TimeSegment ts)
		{
			_context.Add(ts);
		}

		public void DeleteTimeSegment(TimeSegment ts)
		{
			_context.Remove(ts);
			_context.SaveChanges();
		}
	}
}
