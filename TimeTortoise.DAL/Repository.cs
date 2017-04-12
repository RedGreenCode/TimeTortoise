using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
			return _context.Activities.Include(a => a.TimeSegments).ToList();
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

		public void DeleteTimeSegment(TimeSegment ts)
		{
			_context.Remove(ts);
			_context.SaveChanges();
		}
	}
}
